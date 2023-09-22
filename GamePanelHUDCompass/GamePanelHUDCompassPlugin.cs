#if !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using EFT;
using EFT.Interactive;
using EFT.Quests;
using EFTApi;
using GamePanelHUDCore;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Utils;
using HarmonyLib;
using TMPro;
using UnityEngine;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDCompass
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCompass", "kmyuhkyuk-GamePanelHUDCompass", "2.7.5")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "2.7.5")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "localized/compass")]
    public partial class GamePanelHUDCompassPlugin : BaseUnityPlugin, IUpdate
    {
        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

        private readonly CompassData _compassData = new CompassData();

        private readonly CompassFireData _compassFireData = new CompassFireData();

        private readonly CompassStaticData _compassStaticData = new CompassStaticData();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassData, SettingsData> CompassHUD =
            new GamePanelHUDCorePlugin.HUDClass<CompassData, SettingsData>();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassFireData, SettingsData> CompassFireHUD =
            new GamePanelHUDCorePlugin.HUDClass<CompassFireData, SettingsData>();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassStaticData, SettingsData> CompassStaticHUD =
            new GamePanelHUDCorePlugin.HUDClass<CompassStaticData, SettingsData>();

        private bool _compassHUDSw;

        private bool _compassFireHUDSw;

        private bool _compassStaticHUDSw;

        private static bool _compassStaticCacheBool;

        private Transform _camTransform;

        private RectTransform _screenRect;

        private readonly SettingsData _setData;

        private static readonly List<List<string>> Airdrops = new List<List<string>>();

        internal static GameObject FirePrefab { get; private set; }

        internal static GameObject StaticPrefab { get; private set; }

        internal static Action<CompassFireInfo> ShowFire;

        internal static Action<string> DestroyFire;

        internal static Action<CompassStaticInfo> ShowStatic;

        internal static Action<string> DestroyStatic;

        public GamePanelHUDCompassPlugin()
        {
            _setData = new SettingsData(Config);
        }

        private void Start()
        {
            HUDCore.WorldStart += OnHUDCoreOnWorldStart;

            _PlayerHelper.FirearmControllerHelper.InitiateShot.Add(this, nameof(InitiateShot));
            _PlayerHelper.OnDead.Add(this, nameof(OnDead));
            _AirdropHelper.AirdropBoxHelper.OnBoxLand?.Add(this, nameof(OnBoxLand));
            _QuestHelper.OnConditionValueChanged.Add(this, nameof(OnConditionValueChanged));

            HUDCore.UpdateManger.Register(this);
        }

        private static void OnHUDCoreOnWorldStart(GameWorld __instance)
        {
            _compassStaticCacheBool = true;
        }

        private void Awake()
        {
            var prefabs = HUDCore.LoadHUD("gamepanelcompasshud.bundle", "GamePanelCompassHUD");

            FirePrefab = prefabs.Asset["Fire"];

            StaticPrefab = prefabs.Asset["Point"];

            _screenRect = HUDCore.GamePanelHUDPublic.GetComponent<RectTransform>();
        }

        public void CustomUpdate()
        {
            CompassPlugin();
        }

        private void CompassPlugin()
        {
            _compassHUDSw = HUDCore.AllHUDSw && _camTransform != null && HUDCore.HasPlayer &&
                            _setData.KeyCompassHUDSw.Value;
            _compassFireHUDSw = _compassHUDSw && _setData.KeyCompassFireHUDSw.Value;
            _compassStaticHUDSw = _compassHUDSw && _setData.KeyCompassStaticHUDSw.Value;

            _compassData.SizeDelta = _setData.KeyAutoSizeDelta.Value
                ? new Vector2(_screenRect.sizeDelta.x * ((float)_setData.KeyAutoSizeDeltaRate.Value / 100),
                    _setData.KeySizeDelta.Value.y)
                : _setData.KeySizeDelta.Value;

            CompassHUD.Set(_compassData, _setData, _compassHUDSw);
            CompassFireHUD.Set(_compassFireData, _setData, _compassFireHUDSw);
            CompassStaticHUD.Set(_compassStaticData, _setData, _compassStaticHUDSw);

            if (HUDCore.HasPlayer)
            {
                _camTransform = HUDCore.YourPlayer.CameraPosition;

                var levelSettings = EFTGlobal.LevelSettings;

                _compassData.Angle = GetAngle(_camTransform.eulerAngles, levelSettings.NorthDirection);

                _compassFireData.CopyFrom(_compassData);

                _compassFireData.NorthVector = levelSettings.NorthVector;

                _compassFireData.PlayerPosition = _camTransform.position;

                _compassFireData.PlayerRight = _camTransform.right;

                _compassStaticData.CopyFrom(_compassFireData);

                _compassStaticData.YourProfileId = HUDCore.YourPlayer.ProfileId;

                _compassStaticData.Airdrops = Airdrops;

                //Performance Optimization
                if (Time.frameCount % 20 == 0)
                {
                    var hashSet = _PlayerHelper.InventoryHelper.EquipmentHash;

                    hashSet.UnionWith(_PlayerHelper.InventoryHelper.QuestRaidItemsHash);

                    _compassStaticData.EquipmentAndQuestRaidItems = hashSet;
                }

                if (_compassStaticCacheBool)
                {
                    ShowQuest(HUDCore.YourPlayer, HUDCore.TheWorld, EFTVersion.AkiVersion > Version.Parse("2.3.1"),
                        ShowStatic);

                    _compassStaticData.ExfiltrationPoints =
                        ShowExfiltration(HUDCore.YourPlayer, HUDCore.TheWorld, ShowStatic);

                    _compassStaticCacheBool = false;
                }
            }
            else
            {
                _compassStaticData.EquipmentAndQuestRaidItems = null;
                _compassStaticData.ExfiltrationPoints = null;

                if (Airdrops.Count > 0)
                {
                    Airdrops.Clear();
                }
            }
        }

        private static void ShowQuest(Player player, GameWorld world, bool is231Up,
            Action<CompassStaticInfo> showStatic)
        {
            if (player is HideoutPlayer)
                return;

            var questData = Traverse.Create(player).Field("_questController").GetValue<object>();

            var quests = Traverse.Create(questData).Field("Quests").GetValue<object>();

            var questsList = Traverse.Create(quests).Field("list_0").GetValue<IList>();

            var lootItemsList = Traverse.Create(world).Field("LootItems").Field("list_0").GetValue<List<LootItem>>();

            (string Id, LootItem Item)[] questItems =
                lootItemsList.Where(x => x.Item.QuestItem).Select(x => (x.TemplateId, x)).ToArray();

            foreach (var item in questsList)
            {
                if (Traverse.Create(item).Property("QuestStatus").GetValue<EQuestStatus>() != EQuestStatus.Started)
                    continue;

                var template = Traverse.Create(item).Property("Template").GetValue<object>();

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
                                    var staticInfo = new CompassStaticInfo
                                    {
                                        Id = location.id,
                                        Where = trigger.transform.position,
                                        ZoneId = zoneId,
                                        Target = location.target,
                                        NameKey = nameKey,
                                        DescriptionKey = location.id,
                                        TraderId = traderId,
                                        IsNotNecessary = !location.IsNecessary,
                                        InfoType = CompassStaticInfo.Type.ConditionLeaveItemAtLocation
                                    };

                                    showStatic(staticInfo);
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
                                    var staticInfo = new CompassStaticInfo
                                    {
                                        Id = beacon.id,
                                        Where = trigger.transform.position,
                                        ZoneId = zoneId,
                                        Target = beacon.target,
                                        NameKey = nameKey,
                                        DescriptionKey = beacon.id,
                                        TraderId = traderId,
                                        IsNotNecessary = !beacon.IsNecessary,
                                        InfoType = CompassStaticInfo.Type.ConditionPlaceBeacon
                                    };

                                    showStatic(staticInfo);
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
                                        var staticInfo = new CompassStaticInfo
                                        {
                                            Id = findItem.id,
                                            Where = questItem.Item.transform.position,
                                            Target = new[] { itemId },
                                            NameKey = nameKey,
                                            DescriptionKey = findItem.id,
                                            TraderId = traderId,
                                            IsNotNecessary = !findItem.IsNecessary,
                                            InfoType = CompassStaticInfo.Type.ConditionFindItem
                                        };

                                        showStatic(staticInfo);
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
                                                var staticInfo = new CompassStaticInfo
                                                {
                                                    Id = counterCreator.id,
                                                    Where = trigger.transform.position,
                                                    ZoneId = zoneId,
                                                    NameKey = nameKey,
                                                    DescriptionKey = counterCreator.id,
                                                    TraderId = traderId,
                                                    IsNotNecessary = !counterCreator.IsNecessary,
                                                    InfoType = CompassStaticInfo.Type.ConditionVisitPlace
                                                };

                                                showStatic(staticInfo);
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
                                                    var staticInfo = new CompassStaticInfo
                                                    {
                                                        Id = counterCreator.id,
                                                        Where = trigger.transform.position,
                                                        ZoneId = zoneId,
                                                        NameKey = nameKey,
                                                        DescriptionKey = counterCreator.id,
                                                        TraderId = traderId,
                                                        IsNotNecessary = !counterCreator.IsNecessary,
                                                        InfoType = CompassStaticInfo.Type.ConditionInZone
                                                    };

                                                    showStatic(staticInfo);
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

        private static ExfiltrationData[] ShowExfiltration(Player player, GameWorld world,
            Action<CompassStaticInfo> showStatic)
        {
            if (player is HideoutPlayer)
                return null;

            var exfiltrationController = Traverse.Create(world).Property("ExfiltrationController").GetValue<object>();

            var exfiltrationPoints = player.Profile.Side != EPlayerSide.Savage
                ? Traverse.Create(exfiltrationController).Method("EligiblePoints", new[] { typeof(Profile) })
                    .GetValue<ExfiltrationPoint[]>(player.Profile)
                : Traverse.Create(exfiltrationController).Property("ScavExfiltrationPoints")
                    .GetValue<ScavExfiltrationPoint[]>().Where(x => x.EligibleIds.Contains(player.ProfileId))
                    .ToArray<ExfiltrationPoint>();

            var exfiltrationList = new List<ExfiltrationData>();

            for (var i = 0; i < exfiltrationPoints.Length; i++)
            {
                var point = exfiltrationPoints[i];

                var staticInfo = new CompassStaticInfo
                {
                    Id = $"EXFIL{i}",
                    Where = point.transform.position,
                    ExIndex = i,
                    NameKey = point.Settings.Name,
                    DescriptionKey = "EXFIL",
                    InfoType = CompassStaticInfo.Type.Exfiltration
                };

                showStatic(staticInfo);

                var switchs = Array.Empty<Switch>();

                if (point.Status == EExfiltrationStatus.UncompleteRequirements)
                {
                    switchs = Traverse.Create(point).Field("list_1").GetValue<List<Switch>>().ToArray();

                    for (var j = 0; j < switchs.Length; j++)
                    {
                        var @switch = switchs[j];

                        var staticInfo2 = new CompassStaticInfo
                        {
                            Id = @switch.Id,
                            Where = @switch.transform.position,
                            ExIndex = i,
                            ExIndex2 = j,
                            NameKey = point.Settings.Name,
                            DescriptionKey = @switch.ExtractionZoneTip,
                            InfoType = CompassStaticInfo.Type.Switch
                        };

                        showStatic(staticInfo2);
                    }
                }

                exfiltrationList.Add(new ExfiltrationData(point, switchs));
            }

            return exfiltrationList.ToArray();
        }

        private static float GetAngle(Vector3 eulerAngles, float northDirection)
        {
            var num = eulerAngles.y - northDirection;

            if (num >= 0)
                return num;
            else
                return num + 360;
        }

        public class CompassData
        {
            public float Angle;

            public Vector2 SizeDelta;

            public float CompassX => -(Angle / 15 * 120);

            public void CopyFrom(CompassData data)
            {
                Angle = data.Angle;
                SizeDelta = data.SizeDelta;
            }
        }

        public class CompassFireData : CompassData
        {
            public Vector3 NorthVector;

            public Vector3 PlayerPosition;

            public Vector3 PlayerRight;

            public float GetToAngle(Vector3 lhs)
            {
                var num = Vector3.SignedAngle(lhs, NorthVector, Vector3.up);

                if (num >= 0)
                    return num;
                else
                    return num + 360;
            }

            public void CopyFrom(CompassFireData data)
            {
                Angle = data.Angle;
                SizeDelta = data.SizeDelta;

                NorthVector = data.NorthVector;
                PlayerPosition = data.PlayerPosition;
                PlayerRight = data.PlayerRight;
            }
        }

        public class CompassStaticData : CompassFireData
        {
            public ExfiltrationData[] ExfiltrationPoints;

            public HashSet<string> EquipmentAndQuestRaidItems;

            public List<List<string>> Airdrops;

            public string YourProfileId;

            public bool HasEquipmentAndQuestRaidItems => EquipmentAndQuestRaidItems != null;

            public void ExfiltrationGetStatus(int index, out bool notPresent, out bool requirements)
            {
                var point = ExfiltrationPoints[index];

                var status = point.Exfiltration.Status;

                notPresent = status == EExfiltrationStatus.NotPresent;
                requirements = status == EExfiltrationStatus.UncompleteRequirements;
            }

            public void ExfiltrationGetSwitch(int index, int index2, out bool open)
            {
                open = ExfiltrationPoints[index].Switchs[index2].DoorState == EDoorState.Open;
            }

            public void CopyFrom(CompassStaticData data)
            {
                Angle = data.Angle;
                SizeDelta = data.SizeDelta;

                NorthVector = data.NorthVector;
                PlayerPosition = data.PlayerPosition;
                PlayerRight = data.PlayerRight;

                ExfiltrationPoints = data.ExfiltrationPoints;
            }
        }

        public class ExfiltrationData
        {
            public readonly ExfiltrationPoint Exfiltration;

            public readonly Switch[] Switchs;

            public ExfiltrationData(ExfiltrationPoint point, Switch[] switchs)
            {
                Exfiltration = point;
                Switchs = switchs;
            }
        }

        public struct CompassFireInfo
        {
            public string Who;

            public Vector3 Where;

            public float Distance;

            public WildSpawnType Role;

            public bool IsSilenced;
        }

        public struct CompassStaticInfo
        {
            public string Id;

            public Vector3 Where;

            public string ZoneId;

            public string[] Target;

            public string NameKey;

            public string DescriptionKey;

            public string TraderId;

            public int ExIndex;

            public int ExIndex2;

            public bool IsNotNecessary;

            public Type InfoType;

            public enum Type
            {
                Airdrop,
                Exfiltration,
                Switch,
                ConditionLeaveItemAtLocation,
                ConditionPlaceBeacon,
                ConditionFindItem,
                ConditionVisitPlace,
                ConditionInZone
            }
        }

        public class SettingsData
        {
            public readonly ConfigEntry<bool> KeyCompassHUDSw;
            public readonly ConfigEntry<bool> KeyAngleHUDSw;
            public readonly ConfigEntry<bool> KeyCompassFireHUDSw;
            public readonly ConfigEntry<bool> KeyCompassFireDirectionHUDSw;
            public readonly ConfigEntry<bool> KeyCompassFireSilenced;
            public readonly ConfigEntry<bool> KeyCompassFireDeadDestroy;
            public readonly ConfigEntry<bool> KeyCompassStaticHUDSw;
            public readonly ConfigEntry<bool> KeyCompassStaticAirdrop;
            public readonly ConfigEntry<bool> KeyCompassStaticExfiltration;
            public readonly ConfigEntry<bool> KeyCompassStaticQuest;
            public readonly ConfigEntry<bool> KeyCompassStaticInfoHUDSw;
            public readonly ConfigEntry<bool> KeyCompassStaticDistanceHUDSw;
            public readonly ConfigEntry<bool> KeyCompassStaticHideRequirements;
            public readonly ConfigEntry<bool> KeyCompassStaticHideOptional;
            public readonly ConfigEntry<bool> KeyCompassStaticHideSearchedAirdrop;
            public readonly ConfigEntry<bool> KeyAutoSizeDelta;

            public readonly ConfigEntry<bool> KeyConditionFindItem;
            public readonly ConfigEntry<bool> KeyConditionLeaveItemAtLocation;
            public readonly ConfigEntry<bool> KeyConditionPlaceBeacon;
            public readonly ConfigEntry<bool> KeyConditionVisitPlace;
            public readonly ConfigEntry<bool> KeyConditionInZone;

            public readonly ConfigEntry<Vector2> KeyAnchoredPosition;
            public readonly ConfigEntry<Vector2> KeySizeDelta;
            public readonly ConfigEntry<Vector2> KeyLocalScale;
            public readonly ConfigEntry<Vector2> KeyCompassFireSizeDelta;
            public readonly ConfigEntry<Vector2> KeyCompassFireOutlineSizeDelta;
            public readonly ConfigEntry<Vector2> KeyCompassFireDirectionAnchoredPosition;
            public readonly ConfigEntry<Vector2> KeyCompassFireDirectionScale;
            public readonly ConfigEntry<Vector2> KeyCompassStaticInfoAnchoredPosition;
            public readonly ConfigEntry<Vector2> KeyCompassStaticInfoScale;

            public readonly ConfigEntry<float> KeyCompassFireHeight;
            public readonly ConfigEntry<float> KeyCompassFireDistance;
            public readonly ConfigEntry<float> KeyCompassFireActiveSpeed;
            public readonly ConfigEntry<float> KeyCompassFireWaitSpeed;
            public readonly ConfigEntry<float> KeyCompassFireToSmallSpeed;
            public readonly ConfigEntry<float> KeyCompassFireSmallWaitSpeed;
            public readonly ConfigEntry<float> KeyCompassStaticHeight;

            public readonly ConfigEntry<int> KeyAutoSizeDeltaRate;
            public readonly ConfigEntry<int> KeyCompassStaticCenterPointRange;

            public readonly ConfigEntry<Color> KeyArrowColor;
            public readonly ConfigEntry<Color> KeyAzimuthsColor;
            public readonly ConfigEntry<Color> KeyAzimuthsAngleColor;
            public readonly ConfigEntry<Color> KeyDirectionColor;
            public readonly ConfigEntry<Color> KeyAngleColor;
            public readonly ConfigEntry<Color> KeyCompassFireColor;
            public readonly ConfigEntry<Color> KeyCompassFireOutlineColor;
            public readonly ConfigEntry<Color> KeyCompassFireBossColor;
            public readonly ConfigEntry<Color> KeyCompassFireBossOutlineColor;
            public readonly ConfigEntry<Color> KeyCompassFireFollowerColor;
            public readonly ConfigEntry<Color> KeyCompassFireFollowerOutlineColor;
            public readonly ConfigEntry<Color> KeyCompassStaticNameColor;
            public readonly ConfigEntry<Color> KeyCompassStaticDescriptionColor;
            public readonly ConfigEntry<Color> KeyCompassStaticNecessaryColor;
            public readonly ConfigEntry<Color> KeyCompassStaticRequirementsColor;
            public readonly ConfigEntry<Color> KeyCompassStaticDistanceColor;
            public readonly ConfigEntry<Color> KeyCompassStaticMetersColor;

            public readonly ConfigEntry<FontStyles> KeyAzimuthsAngleStyles;
            public readonly ConfigEntry<FontStyles> KeyDirectionStyles;
            public readonly ConfigEntry<FontStyles> KeyAngleStyles;
            public readonly ConfigEntry<FontStyles> KeyCompassFireDirectionStyles;
            public readonly ConfigEntry<FontStyles> KeyCompassStaticNameStyles;
            public readonly ConfigEntry<FontStyles> KeyCompassStaticDescriptionStyles;
            public readonly ConfigEntry<FontStyles> KeyCompassStaticDistanceStyles;

            public SettingsData(ConfigFile configFile)
            {
                const string mainSettings = "Main Settings";
                const string questSettings = "Quest Display Settings";
                const string positionScaleSettings = "Position Scale Settings";
                const string colorSettings = "Color Settings";
                const string fontStylesSettings = "Font Styles Settings";
                const string speedSettings = "Animation Speed Settings";
                const string rateSettings = "Rate Settings";
                const string otherSettings = "Other Settings";

                KeyCompassHUDSw = configFile.Bind<bool>(mainSettings, "Compass HUD display", true);
                KeyAngleHUDSw = configFile.Bind<bool>(mainSettings, "Compass Angle HUD display", true);
                KeyCompassFireHUDSw = configFile.Bind<bool>(mainSettings, "Compass Fire HUD display", true);
                KeyCompassFireDirectionHUDSw =
                    configFile.Bind<bool>(mainSettings, "Compass Fire Direction HUD display", true);
                KeyCompassFireSilenced =
                    configFile.Bind<bool>(mainSettings, "Compass Fire Hide Silenced", true);
                KeyCompassFireDeadDestroy =
                    configFile.Bind<bool>(mainSettings, "Compass Fire Dead Destroy", true);
                KeyCompassStaticHUDSw =
                    configFile.Bind<bool>(mainSettings, "Compass Static HUD display", true);
                KeyCompassStaticAirdrop =
                    configFile.Bind<bool>(mainSettings, "Compass Static Airdrop display", true);
                KeyCompassStaticExfiltration =
                    configFile.Bind<bool>(mainSettings, "Compass Static Exfiltration display", true);
                KeyCompassStaticQuest =
                    configFile.Bind<bool>(mainSettings, "Compass Static Quest display", true);
                KeyCompassStaticInfoHUDSw =
                    configFile.Bind<bool>(mainSettings, "Compass Static Info HUD display", true);
                KeyCompassStaticDistanceHUDSw =
                    configFile.Bind<bool>(mainSettings, "Compass Static Distance HUD display", true);
                KeyCompassStaticHideRequirements =
                    configFile.Bind<bool>(mainSettings, "Compass Static Hide Requirements", false);
                KeyCompassStaticHideOptional =
                    configFile.Bind<bool>(mainSettings, "Compass Static Hide Optional", false);
                KeyCompassStaticHideSearchedAirdrop = configFile.Bind<bool>(mainSettings,
                    "Compass Static Hide Already Searched Airdrop", true);
                KeyAutoSizeDelta = configFile.Bind<bool>(mainSettings, "Auto Size Delta", true);

                KeyConditionFindItem = configFile.Bind<bool>(questSettings, "FindItem", true);
                KeyConditionLeaveItemAtLocation = configFile.Bind<bool>(questSettings, "LeaveItemAtLocation", true);
                KeyConditionPlaceBeacon = configFile.Bind<bool>(questSettings, "PlaceBeacon", true);
                KeyConditionVisitPlace = configFile.Bind<bool>(questSettings, "VisitPlace", true);
                KeyConditionInZone = configFile.Bind<bool>(questSettings, "InZone", true);

                KeyAnchoredPosition =
                    configFile.Bind<Vector2>(positionScaleSettings, "Anchored Position", Vector2.zero);
                KeySizeDelta =
                    configFile.Bind<Vector2>(positionScaleSettings, "Size Delta", new Vector2(600, 90));
                KeyLocalScale =
                    configFile.Bind<Vector2>(positionScaleSettings, "Local Scale", new Vector2(1, 1));
                KeyCompassFireSizeDelta = configFile.Bind<Vector2>(positionScaleSettings,
                    "Compass Fire Size Delta", new Vector2(25, 25));
                KeyCompassFireOutlineSizeDelta = configFile.Bind<Vector2>(positionScaleSettings,
                    "Compass Fire Outline Size Delta", new Vector2(26, 26));
                KeyCompassFireDirectionAnchoredPosition = configFile.Bind<Vector2>(positionScaleSettings,
                    "Compass Fire Direction Anchored Position", new Vector2(15, -63));
                KeyCompassFireDirectionScale = configFile.Bind<Vector2>(positionScaleSettings,
                    "Compass Fire Direction Local Scale", new Vector2(1, 1));
                KeyCompassStaticInfoAnchoredPosition = configFile.Bind<Vector2>(positionScaleSettings,
                    "Compass Static Info Anchored Position", new Vector2(0, -15));
                KeyCompassStaticInfoScale = configFile.Bind<Vector2>(positionScaleSettings,
                    "Compass Static Info Local Scale", new Vector2(1, 1));

                KeyCompassFireActiveSpeed = configFile.Bind<float>(speedSettings, "Compass Fire Active Speed",
                    1, new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));
                KeyCompassFireWaitSpeed = configFile.Bind<float>(speedSettings, "Compass Fire Wait Speed", 1,
                    new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));
                KeyCompassFireToSmallSpeed = configFile.Bind<float>(speedSettings,
                    "Compass Fire To Small Speed", 1,
                    new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));
                KeyCompassFireSmallWaitSpeed = configFile.Bind<float>(speedSettings,
                    "Compass Fire Small Wait Speed", 1,
                    new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));

                KeyCompassFireHeight = configFile.Bind<float>(positionScaleSettings, "Compass Fire Height", 8);
                KeyCompassStaticHeight =
                    configFile.Bind<float>(positionScaleSettings, "Compass Static Height", 5);

                KeyCompassFireDistance = configFile.Bind<float>(otherSettings, "Compass Fire Max Distance",
                    50,
                    new ConfigDescription("Fire distance <= How many meters display",
                        new AcceptableValueRange<float>(0, 1000)));
                KeyCompassStaticCenterPointRange =
                    configFile.Bind<int>(otherSettings, "Compass Static Center Point Range", 20);

                KeyAutoSizeDeltaRate = configFile.Bind<int>(rateSettings, "Auto Size Delta Rate", 30,
                    new ConfigDescription("Screen percentage", new AcceptableValueRange<int>(0, 100)));

                KeyArrowColor = configFile.Bind<Color>(colorSettings, "Arrow", new Color(1f, 1f, 1f));
                KeyAzimuthsColor = configFile.Bind<Color>(colorSettings, "Azimuths",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyAzimuthsAngleColor = configFile.Bind<Color>(colorSettings, "Azimuths Angle",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyDirectionColor = configFile.Bind<Color>(colorSettings, "Direction",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyAngleColor =
                    configFile.Bind<Color>(colorSettings, "Angle", new Color(0.8901961f, 0.8901961f, 0.8392157f));

                KeyCompassFireColor =
                    configFile.Bind<Color>(colorSettings, "Compass Fire", new Color(1f, 0f, 0f));
                KeyCompassFireOutlineColor =
                    configFile.Bind<Color>(colorSettings, "Compass Fire Outline", new Color(0.5f, 0f, 0f));
                KeyCompassFireBossColor =
                    configFile.Bind<Color>(colorSettings, "Compass Boss Fire", new Color(1f, 0.5f, 0f));
                KeyCompassFireBossOutlineColor = configFile.Bind<Color>(colorSettings,
                    "Compass Boss Fire Outline", new Color(1f, 0.3f, 0f));
                KeyCompassFireFollowerColor =
                    configFile.Bind<Color>(colorSettings, "Compass Follower Fire", new Color(0f, 1f, 1f));
                KeyCompassFireFollowerOutlineColor = configFile.Bind<Color>(colorSettings,
                    "Compass Follower Outline", new Color(0f, 0.7f, 1f));
                KeyCompassStaticNameColor = configFile.Bind<Color>(colorSettings, "Compass Static Name",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyCompassStaticDescriptionColor = configFile.Bind<Color>(colorSettings,
                    "Compass Static Description", new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyCompassStaticNecessaryColor = configFile.Bind<Color>(colorSettings,
                    "Compass Static Optional", new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyCompassStaticRequirementsColor = configFile.Bind<Color>(colorSettings,
                    "Compass Static Requirements", new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyCompassStaticDistanceColor = configFile.Bind<Color>(colorSettings, "Compass Static Distance",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f));
                KeyCompassStaticMetersColor = configFile.Bind<Color>(colorSettings, "Compass Static Meters",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f));

                KeyAzimuthsAngleStyles =
                    configFile.Bind<FontStyles>(fontStylesSettings, "Azimuths Angle", FontStyles.Normal);
                KeyDirectionStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Direction", FontStyles.Bold);
                KeyAngleStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Angle", FontStyles.Bold);
                KeyCompassFireDirectionStyles = configFile.Bind<FontStyles>(fontStylesSettings,
                    "Compass Fire Direction", FontStyles.Normal);
                KeyCompassStaticNameStyles =
                    configFile.Bind<FontStyles>(fontStylesSettings, "Compass Static Name", FontStyles.Bold);
                KeyCompassStaticDescriptionStyles = configFile.Bind<FontStyles>(fontStylesSettings,
                    "Compass Static Description", FontStyles.Normal);
                KeyCompassStaticDistanceStyles = configFile.Bind<FontStyles>(fontStylesSettings,
                    "Compass Static Distance", FontStyles.Bold);
            }
        }
    }
}
#endif