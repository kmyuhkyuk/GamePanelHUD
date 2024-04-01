using System;
using System.Collections.Generic;
using System.Linq;
using EFT;
using EFT.Interactive;
using UnityEngine;
#if !UNITY_EDITOR
using EFT.Quests;
using EFTApi;
using EFTUtils;
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
            HUDCoreModel.Instance.WorldStart += OnWorldStart;

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
                    var hashSet = _InventoryHelper.EquipmentHash;

                    hashSet.UnionWith(_InventoryHelper.QuestRaidItemsHash);

                    compassStaticHUDModel.CompassStatic.EquipmentAndQuestRaidItems = hashSet;
                }
            }
            else
            {
                compassStaticHUDModel.CompassStatic.EquipmentAndQuestRaidItems = null;
                compassStaticHUDModel.AirdropCount = 0;
            }
        }

        private static void OnWorldStart(GameWorld __instance)
        {
            var hudCoreModel = HUDCoreModel.Instance;
            var compassStaticHUDModel = CompassStaticHUDModel.Instance;

            ShowQuest(hudCoreModel.YourPlayer, __instance, hudCoreModel.TheGame,
                compassStaticHUDModel.ShowStatic);
            ShowExfiltration(hudCoreModel.YourPlayer, compassStaticHUDModel.ShowStatic);
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static void ShowQuest(Player player, GameWorld world, AbstractGame game, Action<StaticModel> callback)
        {
            if (player is HideoutPlayer)
                return;

            var reflectionModel = ReflectionModel.Instance;

            var questData = _PlayerHelper.RefQuestController.GetValue(player);

            var quests = reflectionModel.RefQuests.GetValue(questData);

            var questsList = reflectionModel.RefQuestsList.GetValue(quests);

            var lootItems = _GameWorldHelper.RefLootItems.GetValue(world);

            var lootItemsList = reflectionModel.RefLootList.GetValue(lootItems);

            (string Id, LootItem Item)[] questItems =
                lootItemsList.Where(x => x.Item.QuestItem).Select(x => (x.TemplateId, x)).ToArray();

            var is231Up = EFTVersion.AkiVersion > EFTVersion.Parse("2.3.1");

            foreach (var item in questsList)
            {
                if (reflectionModel.RefQuestStatus.GetValue(item) != EQuestStatus.Started)
                    continue;

                var template = reflectionModel.RefTemplate.GetValue(item);

                var locationId = reflectionModel.RefLocationId.GetValue(template);

                if (locationId != game.LocationObjectId && locationId != "any")
                    continue;

                switch (is231Up)
                {
                    case true when (player.Profile.Side == EPlayerSide.Savage ? 1 : 0) !=
                                   reflectionModel.RefPlayerGroup.GetValue(template):
                    case false when player.Profile.Side == EPlayerSide.Savage:
                        continue;
                }

                var nameKey = reflectionModel.RefNameLocaleKey.GetValue(template);

                var traderId = reflectionModel.RefTraderId.GetValue(template);

                var availableForFinishConditions = reflectionModel.RefAvailableForFinishConditions.GetValue(item);

                var availableForFinishConditionsList =
                    reflectionModel.RefAvailableForFinishConditionsList.GetValue(availableForFinishConditions);

                foreach (var condition in availableForFinishConditionsList)
                {
                    switch (condition)
                    {
                        case ConditionLeaveItemAtLocation location:
                        {
                            var zoneId = location.zoneId;

                            if (_ZoneHelper.TryGetValues(zoneId,
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

                                    callback(staticModel);
                                }
                            }

                            break;
                        }
                        case ConditionPlaceBeacon beacon:
                        {
                            var zoneId = beacon.zoneId;

                            if (_ZoneHelper.TryGetValues(zoneId,
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

                                    callback(staticModel);
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
                                    if (!questItem.Id.Equals(itemId, StringComparison.OrdinalIgnoreCase))
                                        continue;

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

                                    callback(staticModel);
                                }
                            }

                            break;
                        }
                        case ConditionCounterCreator counterCreator:
                        {
                            var counter = reflectionModel.RefCounter.GetValue(counterCreator);

                            var conditions = reflectionModel.RefConditions.GetValue(counter);

                            var conditionsList = reflectionModel.RefConditionsList.GetValue(conditions);

                            foreach (var condition2 in conditionsList)
                            {
                                switch (condition2)
                                {
                                    case ConditionVisitPlace place:
                                    {
                                        var zoneId = place.target;

                                        if (_ZoneHelper.TryGetValues(zoneId,
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

                                                callback(staticModel);
                                            }
                                        }

                                        break;
                                    }
                                    case ConditionInZone inZone:
                                    {
                                        var zoneIds = inZone.zoneIds;

                                        foreach (var zoneId in zoneIds)
                                        {
                                            if (!_ZoneHelper.TryGetValues(zoneId,
                                                    out IEnumerable<ExperienceTrigger> triggers))
                                                continue;

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

                                                callback(staticModel);
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

        // ReSharper disable once SuggestBaseTypeForParameter
        private static void ShowExfiltration(Player player, Action<StaticModel> callback)
        {
            if (player is HideoutPlayer)
                return;

            var exfiltrationPoints = player.Profile.Side != EPlayerSide.Savage
                ? _ExfiltrationControllerHelper.EligiblePoints(_ExfiltrationControllerHelper.ExfiltrationController,
                    player.Profile)
                : _ExfiltrationControllerHelper.ScavExfiltrationPoints
                    .Where(x => x.EligibleIds.Contains(player.ProfileId)).ToArray<ExfiltrationPoint>();

            for (var i = 0; i < exfiltrationPoints.Length; i++)
            {
                var point = exfiltrationPoints[i];

                var exfiltrationRequirements = new Func<bool>[]
                {
                    () => point.Status == EExfiltrationStatus.NotPresent,
                    () => point.Status == EExfiltrationStatus.UncompleteRequirements
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

                callback(staticModel);

                if (point.Status != EExfiltrationStatus.UncompleteRequirements)
                    continue;

                foreach (var @switch in _ExfiltrationPointHelper.RefSwitchs.GetValue(point))
                {
                    var switchStaticModel = new StaticModel
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

                    callback(switchStaticModel);
                }
            }
        }

#endif
    }
}