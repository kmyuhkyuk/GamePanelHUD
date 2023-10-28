using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EFT;
using EFT.Interactive;
using UnityEngine;
#if !UNITY_EDITOR
using EFT.Quests;
using EFTApi;
using EFTUtils;
using HarmonyLib;
using static EFTApi.EFTHelpers;
using GamePanelHUDCore.Models;
using GamePanelHUDCompass.Models;
using SettingsModel = GamePanelHUDCompass.Models.SettingsModel;

#endif

namespace GamePanelHUDCompass.Controllers
{
    public class CompassStaticHUDController : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var hudCoreModel = HUDCoreModel.Instance;
            var compassHUDModel = CompassHUDModel.Instance;
            var compassStaticHUDModel = CompassStaticHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            compassStaticHUDModel.CompassStaticHUDSw =
                compassHUDModel.CompassHUDSw && settingsModel.KeyCompassStaticHUDSw.Value;

            if (hudCoreModel.HasPlayer)
            {
                compassStaticHUDModel.CompassStatic.YourProfileId = hudCoreModel.YourPlayer.ProfileId;

                compassStaticHUDModel.CompassStatic.TriggerZones = hudCoreModel.YourPlayer.TriggerZones;

                //Performance Optimization
                if (Time.frameCount % 20 == 0)
                {
                    var hashSet = _PlayerHelper.InventoryHelper.EquipmentHash;

                    hashSet.UnionWith(_PlayerHelper.InventoryHelper.QuestRaidItemsHash);

                    compassStaticHUDModel.CompassStatic.EquipmentAndQuestRaidItems = hashSet;
                }

                if (compassStaticHUDModel.CompassStaticCacheBool)
                {
                    ShowQuest(hudCoreModel.YourPlayer, hudCoreModel.TheWorld, hudCoreModel.TheGame,
                        compassStaticHUDModel.ShowStatic);

                    ShowExfiltration(hudCoreModel.YourPlayer, hudCoreModel.TheWorld, compassStaticHUDModel.ShowStatic);

                    compassStaticHUDModel.CompassStaticCacheBool = false;
                }
            }
            else
            {
                compassStaticHUDModel.CompassStatic.EquipmentAndQuestRaidItems = null;
                compassStaticHUDModel.AirdropCount = 0;
            }
        }

        private static void ShowQuest(Player player, GameWorld world, AbstractGame game, Action<StaticModel> showStatic)
        {
            if (player is HideoutPlayer)
                return;

            var questData = Traverse.Create(player).Field("_questController").GetValue<object>();

            var quests = Traverse.Create(questData).Field("Quests").GetValue<object>();

            var questsList = Traverse.Create(quests).Field("list_0").GetValue<IList>();

            var lootItemsList = Traverse.Create(world).Field("LootItems").Field("list_0").GetValue<List<LootItem>>();

            (string Id, LootItem Item)[] questItems =
                lootItemsList.Where(x => x.Item.QuestItem).Select(x => (x.TemplateId, x)).ToArray();

            var is231Up = EFTVersion.AkiVersion > Version.Parse("2.3.1");

            foreach (var item in questsList)
            {
                if (Traverse.Create(item).Property("QuestStatus").GetValue<EQuestStatus>() != EQuestStatus.Started)
                    continue;

                var template = Traverse.Create(item).Property("Template").GetValue<object>();

                var locationId = Traverse.Create(template).Field("LocationId").GetValue<string>();

                if (locationId != game.LocationObjectId && locationId != "any")
                    continue;

                switch (is231Up)
                {
                    case true when (player.Profile.Side == EPlayerSide.Savage ? 1 : 0) !=
                                   Traverse.Create(template).Field("PlayerGroup").GetValue<int>():
                    case false when player.Profile.Side == EPlayerSide.Savage:
                        continue;
                }

                var nameKey = Traverse.Create(template).Property("NameLocaleKey").GetValue<string>();

                var traderId = Traverse.Create(template).Field("TraderId").GetValue<string>();

                var availableForFinishConditions =
                    Traverse.Create(item).Property("AvailableForFinishConditions").GetValue<object>();

                var availableForFinishConditionsList =
                    Traverse.Create(availableForFinishConditions).Field("list_0").GetValue<IList>();

                foreach (var condition in availableForFinishConditionsList)
                {
                    switch (condition)
                    {
                        case ConditionLeaveItemAtLocation location:
                        {
                            var zoneId = location.zoneId;

                            if (_GameWorldHelper.ZoneHelper.TryGetValues(zoneId,
                                    out IEnumerable<PlaceItemTrigger> triggers))
                            {
                                foreach (var trigger in triggers)
                                {
                                    var staticModel = new StaticModel
                                    {
                                        Id = location.id,
                                        Where = trigger.transform.position,
                                        ZoneId = zoneId,
                                        Target = location.target,
                                        NameKey = nameKey,
                                        DescriptionKey = location.id,
                                        TraderId = traderId,
                                        IsNotNecessary = !location.IsNecessary,
                                        InfoType = StaticModel.Type.ConditionLeaveItemAtLocation
                                    };

                                    showStatic(staticModel);
                                }
                            }

                            break;
                        }
                        case ConditionPlaceBeacon beacon:
                        {
                            var zoneId = beacon.zoneId;

                            if (_GameWorldHelper.ZoneHelper.TryGetValues(zoneId,
                                    out IEnumerable<PlaceItemTrigger> triggers))
                            {
                                foreach (var trigger in triggers)
                                {
                                    var staticModel = new StaticModel
                                    {
                                        Id = beacon.id,
                                        Where = trigger.transform.position,
                                        ZoneId = zoneId,
                                        Target = beacon.target,
                                        NameKey = nameKey,
                                        DescriptionKey = beacon.id,
                                        TraderId = traderId,
                                        IsNotNecessary = !beacon.IsNecessary,
                                        InfoType = StaticModel.Type.ConditionPlaceBeacon
                                    };

                                    showStatic(staticModel);
                                }
                            }

                            break;
                        }
                        case ConditionFindItem findItem:
                        {
                            var itemIds = findItem.target;

                            foreach (var itemId in itemIds)
                            {
                                foreach (var questItem in questItems)
                                {
                                    if (questItem.Id.Equals(itemId, StringComparison.OrdinalIgnoreCase))
                                    {
                                        var staticModel = new StaticModel
                                        {
                                            Id = findItem.id,
                                            Where = questItem.Item.transform.position,
                                            Target = new[] { itemId },
                                            NameKey = nameKey,
                                            DescriptionKey = findItem.id,
                                            TraderId = traderId,
                                            IsNotNecessary = !findItem.IsNecessary,
                                            InfoType = StaticModel.Type.ConditionFindItem
                                        };

                                        showStatic(staticModel);
                                    }
                                }
                            }

                            break;
                        }
                        case ConditionCounterCreator counterCreator:
                        {
                            var counter = Traverse.Create(counterCreator).Field("counter").GetValue<object>();

                            var conditions = Traverse.Create(counter).Property("conditions").GetValue<object>();

                            var conditionsList = Traverse.Create(conditions).Field("list_0").GetValue<IList>();

                            foreach (var condition2 in conditionsList)
                            {
                                switch (condition2)
                                {
                                    case ConditionVisitPlace place:
                                    {
                                        var zoneId = place.target;

                                        if (_GameWorldHelper.ZoneHelper.TryGetValues(zoneId,
                                                out IEnumerable<ExperienceTrigger> triggers))
                                        {
                                            foreach (var trigger in triggers)
                                            {
                                                var staticModel = new StaticModel
                                                {
                                                    Id = counterCreator.id,
                                                    Where = trigger.transform.position,
                                                    ZoneId = zoneId,
                                                    NameKey = nameKey,
                                                    DescriptionKey = counterCreator.id,
                                                    TraderId = traderId,
                                                    IsNotNecessary = !counterCreator.IsNecessary,
                                                    InfoType = StaticModel.Type.ConditionVisitPlace
                                                };

                                                showStatic(staticModel);
                                            }
                                        }

                                        break;
                                    }
                                    case ConditionInZone inZone:
                                    {
                                        var zoneIds = inZone.zoneIds;

                                        foreach (var zoneId in zoneIds)
                                        {
                                            if (_GameWorldHelper.ZoneHelper.TryGetValues(zoneId,
                                                    out IEnumerable<ExperienceTrigger> triggers))
                                            {
                                                foreach (var trigger in triggers)
                                                {
                                                    var staticModel = new StaticModel
                                                    {
                                                        Id = counterCreator.id,
                                                        Where = trigger.transform.position,
                                                        ZoneId = zoneId,
                                                        NameKey = nameKey,
                                                        DescriptionKey = counterCreator.id,
                                                        TraderId = traderId,
                                                        IsNotNecessary = !counterCreator.IsNecessary,
                                                        InfoType = StaticModel.Type.ConditionInZone
                                                    };

                                                    showStatic(staticModel);
                                                }
                                            }
                                        }

                                        break;
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }

        private static void ShowExfiltration(Player player, GameWorld world,
            Action<StaticModel> showStatic)
        {
            if (player is HideoutPlayer)
                return;

            var exfiltrationController = Traverse.Create(world).Property("ExfiltrationController").GetValue<object>();

            var exfiltrationPoints = player.Profile.Side != EPlayerSide.Savage
                ? Traverse.Create(exfiltrationController).Method("EligiblePoints", new[] { typeof(Profile) })
                    .GetValue<ExfiltrationPoint[]>(player.Profile)
                : Traverse.Create(exfiltrationController).Property("ScavExfiltrationPoints")
                    .GetValue<ScavExfiltrationPoint[]>().Where(x => x.EligibleIds.Contains(player.ProfileId))
                    .ToArray<ExfiltrationPoint>();

            for (var i = 0; i < exfiltrationPoints.Length; i++)
            {
                var point = exfiltrationPoints[i];

                var exfiltrationRequirements = new Func<bool>[]
                {
                    () => point.Status == EExfiltrationStatus.NotPresent,
                    () => point.Status == EExfiltrationStatus.UncompleteRequirements,
                };

                var staticModel = new StaticModel
                {
                    Id = $"EXFIL{i}",
                    Where = point.transform.position,
                    NameKey = point.Settings.Name,
                    DescriptionKey = "EXFIL",
                    InfoType = StaticModel.Type.Exfiltration,
                    Requirements = exfiltrationRequirements
                };

                showStatic(staticModel);

                if (point.Status == EExfiltrationStatus.UncompleteRequirements)
                {
                    var switchs = EFTVersion.AkiVersion > Version.Parse("3.6.1")
                        ? Traverse.Create(point).Field("_switches").GetValue<List<Switch>>().ToArray()
                        : Traverse.Create(point).Field("list_1").GetValue<List<Switch>>().ToArray();

                    foreach (var @switch in switchs)
                    {
                        var staticModel2 = new StaticModel
                        {
                            Id = @switch.Id,
                            Where = @switch.transform.position,
                            NameKey = point.Settings.Name,
                            DescriptionKey = @switch.ExtractionZoneTip,
                            InfoType = StaticModel.Type.Switch,
                            Requirements = exfiltrationRequirements.Concat(new Func<bool>[]
                            {
                                () => @switch.DoorState == EDoorState.Open
                            }).ToArray()
                        };

                        showStatic(staticModel2);
                    }
                }
            }
        }

#endif
    }
}