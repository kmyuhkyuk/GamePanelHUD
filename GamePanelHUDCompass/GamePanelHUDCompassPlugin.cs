#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EFT;
using EFT.Quests;
using EFT.Interactive;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
using GamePanelHUDCore.Utils.Zone;
using GamePanelHUDCompass.Patches;

namespace GamePanelHUDCompass
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCompass", "kmyuhkyuk-GamePanelHUDCompass", "2.5.3")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class GamePanelHUDCompassPlugin : BaseUnityPlugin, IUpdate
    {
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore
        {
            get
            {
                return GamePanelHUDCorePlugin.HUDCore;
            }
        }

        private readonly CompassData CompassInfos = new CompassData();

        private readonly CompassFireData CompassFireDatas = new CompassFireData();

        private readonly CompassStaticData CompassStaticDatas = new CompassStaticData();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassData, SettingsData> CompassHUD = new GamePanelHUDCorePlugin.HUDClass<CompassData, SettingsData>();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassFireData, SettingsData> CompassFireHUD = new GamePanelHUDCorePlugin.HUDClass<CompassFireData, SettingsData>();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassStaticData, SettingsData> CompassStaticHUD = new GamePanelHUDCorePlugin.HUDClass<CompassStaticData, SettingsData>();

        internal static float NorthDirection;

        internal static Vector3 NorthVector;

        private bool CompassHUDSW;

        private bool CompassFireHUDSW;

        private bool CompassStaticHUDSW;

        private bool CompassQuestCacheBool = true;

        private bool CompassExitCacheBool;

        private Transform Cam;

        private RectTransform ScreenRect;

        private readonly SettingsData SettingsDatas = new SettingsData();

        private ExfiltrationData[] ExfiltrationPoints;

        internal static int AirdropCount;

        internal static GameObject FirePrefab { get; private set; }

        internal static GameObject StaticPrefab { get; private set; }

        internal static Action<CompassFireInfo> ShowFire;

        internal static Action<int> DestroyFire;

        internal static Action<CompassStaticInfo> ShowStatic;

        internal static Action<string> DestroyStatic;

        private static bool Is231Up = GamePanelHUDCorePlugin.HUDCoreClass.GameVersion > new Version("0.12.12.17349");

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDCompass");

            ModUpdateCheck.DrawNeedUpdate(Config, Info.Metadata.Version);

            const string mainSettings = "主设置 Main Settings";
            const string positionScaleSettings = "位置大小设置 Position Scale Settings";
            const string colorSettings = "颜色设置 Color Settings";
            const string fontStylesSettings = "字体样式设置 Font Styles Settings";
            const string speedSettings = "动画速度设置 Animation Speed Settings";
            const string rateSettings = "率设置  Rate Settings";
            const string otherSettings = "其他设置 Other Settings";

            SettingsDatas.KeyCompassHUDSW = Config.Bind<bool>(mainSettings, "罗盘指示栏显示 Compass HUD display", true);
            SettingsDatas.KeyAngleHUDSW = Config.Bind<bool>(mainSettings, "罗盘角度显示 Compass Angle HUD display", true);
            SettingsDatas.KeyCompassFireHUDSW = Config.Bind<bool>(mainSettings, "罗盘开火指示栏显示 Compass Fire HUD display", true);
            SettingsDatas.KeyCompassFireDirectionHUDSW = Config.Bind<bool>(mainSettings, "罗盘开火方向指示栏显示 Compass Fire Direction HUD display", true);
            SettingsDatas.KeyCompassFireSilenced = Config.Bind<bool>(mainSettings, "罗盘开火隐藏消音 Compass Fire Hide Silenced", true);
            SettingsDatas.KeyCompassFireDeadDestroy = Config.Bind<bool>(mainSettings, "罗盘开火死亡销毁 Compass Fire Dead Destroy", true);
            SettingsDatas.KeyCompassStaticHUDSW = Config.Bind<bool>(mainSettings, "罗盘静态指示栏显示 Compass Static HUD display", true);
            SettingsDatas.KeyCompassStaticAirdrop = Config.Bind<bool>(mainSettings, "罗盘静态空投显示 Compass Static Airdrop display", true);
            SettingsDatas.KeyCompassStaticExfiltration = Config.Bind<bool>(mainSettings, "罗盘静态撤离点显示 Compass Static Exfiltration display", true);
            SettingsDatas.KeyCompassStaticQuest = Config.Bind<bool>(mainSettings, "罗盘静态任务显示 Compass Static Quest display", true);
            SettingsDatas.KeyCompassStaticInfoHUDSW = Config.Bind<bool>(mainSettings, "罗盘静态信息显示 Compass Static Info HUD display", true);
            SettingsDatas.KeyCompassStaticDistanceHUDSW = Config.Bind<bool>(mainSettings, "罗盘静态距离显示 Compass Static Distance HUD display", true);
            SettingsDatas.KeyCompassStaticHideRequirements = Config.Bind<bool>(mainSettings, "罗盘静态隐藏需求 Compass Static Hide Requirements", false);
            SettingsDatas.KeyCompassStaticHideOptional = Config.Bind<bool>(mainSettings, "罗盘静态隐藏可选项 Compass Static Hide Optional", false);
            SettingsDatas.KeyAutoSizeDelta = Config.Bind<bool>(mainSettings, "自动高度 Auto Size Delta", true);

            SettingsDatas.KeyAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "指示栏位置 Anchored Position", new Vector2(0, 0));
            SettingsDatas.KeySizeDelta = Config.Bind<Vector2>(positionScaleSettings, "指示栏高度 Size Delta", new Vector2(600, 90));
            SettingsDatas.KeyLocalScale = Config.Bind<Vector2>(positionScaleSettings, "指示栏大小 Local Scale", new Vector2(1, 1));
            SettingsDatas.KeyCompassFireSizeDelta = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火高度 Compass Fire Size Delta", new Vector2(25, 25));
            SettingsDatas.KeyCompassFireOutlineSizeDelta = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火轮廓高度 Compass Fire Outline Size Delta", new Vector2(26, 26));
            SettingsDatas.KeyCompassFireDirectionAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火方向位置 Compass Fire Direction Anchored Position", new Vector2(15, -63));
            SettingsDatas.KeyCompassFireDirectionScale = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火方向大小 Compass Fire Direction Local Scale", new Vector2(1, 1));
            SettingsDatas.KeyCompasStaticInfoAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "罗盘静态信息位置 Compass Static Info Anchored Position", new Vector2(0, -15));
            SettingsDatas.KeyCompassStaticInfoScale = Config.Bind<Vector2>(positionScaleSettings, "罗盘静态信息大小 Compass Static Info Local Scale", new Vector2(1, 1));

            SettingsDatas.KeyCompassFireActiveSpeed = Config.Bind<float>(speedSettings, "罗盘开火激活速度 Compass Fire Active Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SettingsDatas.KeyCompassFireWaitSpeed = Config.Bind<float>(speedSettings, "罗盘开火等待速度 Compass Fire Wait Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SettingsDatas.KeyCompassFireToSmallSpeed = Config.Bind<float>(speedSettings, "罗盘开火变小速度 Compass Fire To Small Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SettingsDatas.KeyCompassFireSmallWaitSpeed = Config.Bind<float>(speedSettings, "罗盘开火变小等待速度 Compass Fire Small Wait Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));

            SettingsDatas.KeyCompassFireHeight = Config.Bind<float>(positionScaleSettings, "罗盘开火高度 Compass Fire Height", 8);
            SettingsDatas.KeyCompassStaticHeight = Config.Bind<float>(positionScaleSettings, "罗盘静态高度 Compass Static Height", 5);

            SettingsDatas.KeyCompassFireDistance = Config.Bind<float>(otherSettings, "罗盘开火最大距离 Compass Fire Max Distance", 50, new ConfigDescription("Fire distance <= How many meters display", new AcceptableValueRange<float>(0, 1000)));
            SettingsDatas.KeyCompassStaticCenterPointRange = Config.Bind<int>(otherSettings, "罗盘静态中心点范围 Compass Static Center Point Range", 20);

            SettingsDatas.KeyAutoSizeDeltaRate = Config.Bind<int>(rateSettings, "自动高度比率 Auto Size Delta Rate", 30, new ConfigDescription("Screen percentage", new AcceptableValueRange<int>(0, 100)));

            SettingsDatas.KeyArrowColor = Config.Bind<Color>(colorSettings, "指针 Arrow", new Color(1f, 1f, 1f));
            SettingsDatas.KeyAzimuthsColor = Config.Bind<Color>(colorSettings, "刻度 Azimuths", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyAzimuthsAngleColor = Config.Bind<Color>(colorSettings, "刻度角度 Azimuths Angle", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyDirectionColor = Config.Bind<Color>(colorSettings, "方向 Direction", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyAngleColor = Config.Bind<Color>(colorSettings, "角度 Angle", new Color(0.8901961f, 0.8901961f, 0.8392157f));

            SettingsDatas.KeyCompassFireColor = Config.Bind<Color>(colorSettings, "罗盘开火 Compass Fire", new Color(1f, 0f, 0f));
            SettingsDatas.KeyCompassFireOutlineColor = Config.Bind<Color>(colorSettings, "罗盘开火轮廓 Compass Fire Outline", new Color(0.5f, 0f, 0f));
            SettingsDatas.KeyCompassFireBossColor = Config.Bind<Color>(colorSettings, "罗盘开火 Compass Boss Fire", new Color(1f, 0.5f, 0f));
            SettingsDatas.KeyCompassFireBossOutlineColor = Config.Bind<Color>(colorSettings, "罗盘开火轮廓 Compass Boss Fire Outline", new Color(1f, 0.3f, 0f));
            SettingsDatas.KeyCompassFireFollowerColor = Config.Bind<Color>(colorSettings, "罗盘开火 Compass Follower Fire", new Color(0f, 1f, 1f));
            SettingsDatas.KeyCompassFireFollowerOutlineColor = Config.Bind<Color>(colorSettings, "罗盘开火轮廓 Compass Follower Outline", new Color(0f, 0.7f, 1f));
            SettingsDatas.KeyCompassStaticNameColor = Config.Bind<Color>(colorSettings, "罗盘静态名字 Compass Static Name", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyCompassStaticDescriptionColor = Config.Bind<Color>(colorSettings, "罗盘静态说明 Compass Static Description", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyCompassStaticNecessaryColor = Config.Bind<Color>(colorSettings, "罗盘静态可选项 Compass Static Optional", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyCompassStaticRequirementsColor = Config.Bind<Color>(colorSettings, "罗盘静态需求 Compass Static Requirements", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyCompassStaticDistanceColor = Config.Bind<Color>(colorSettings, "罗盘静态距离 Compass Static Distance", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyCompassStaticMetersColor = Config.Bind<Color>(colorSettings, "罗盘静态米 Compass Static Meters", new Color(0.8901961f, 0.8901961f, 0.8392157f));

            SettingsDatas.KeyAzimuthsAngleStyles = Config.Bind<FontStyles>(fontStylesSettings, "刻度角度 Azimuths Angle", FontStyles.Normal);
            SettingsDatas.KeyDirectionStyles = Config.Bind<FontStyles>(fontStylesSettings, "方向 Direction", FontStyles.Bold);
            SettingsDatas.KeyAngleStyles = Config.Bind<FontStyles>(fontStylesSettings, "角度 Angle", FontStyles.Bold);
            SettingsDatas.KeyCompassFireDirectionStyles = Config.Bind<FontStyles>(fontStylesSettings, "罗盘开火方向 Compass Fire Direction", FontStyles.Normal);
            SettingsDatas.KeyCompassStaticNameStyles = Config.Bind<FontStyles>(fontStylesSettings, "罗盘静态名字 Compass Static Name", FontStyles.Bold);
            SettingsDatas.KeyCompassStaticDescriptionStyles = Config.Bind<FontStyles>(fontStylesSettings, "罗盘静态说明 Compass Static Description", FontStyles.Normal);
            SettingsDatas.KeyCompassStaticDistanceStyles = Config.Bind<FontStyles>(fontStylesSettings, "罗盘静态距离 Compass Static Distance", FontStyles.Bold);

            new LevelSettingsPatch().Enable();
            new PlayerShotPatch().Enable();
            new PlayerDeadPatch().Enable();
            new OnConditionValueChangedPatch().Enable();

            if (Is231Up)
            {
                new AirdropBoxPatch().Enable();
            }

            GamePanelHUDCorePlugin.HUDCoreClass.WorldStart += (GameWorld) => CompassExitCacheBool = true;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            GamePanelHUDCorePlugin.HUDCoreClass.AssetData<GameObject> prefabs = GamePanelHUDCorePlugin.HUDCoreClass.LoadHUD("gamepanlcompasshud.bundle", "GamePanlCompassHUD");

            FirePrefab = prefabs.Asset["Fire"];

            StaticPrefab = prefabs.Asset["Point"];

            ScreenRect = GamePanelHUDCorePlugin.HUDCoreClass.GamePanlHUDPublic.GetComponent<RectTransform>();
        }

        public void IUpdate()
        {
            CompassPlugin();
        }

        void CompassPlugin()
        {
            CompassHUDSW = HUDCore.AllHUDSW && Cam != null && HUDCore.HasPlayer && SettingsDatas.KeyCompassHUDSW.Value;
            CompassFireHUDSW = CompassHUDSW && SettingsDatas.KeyCompassFireHUDSW.Value;
            CompassStaticHUDSW = CompassHUDSW && SettingsDatas.KeyCompassStaticHUDSW.Value;

            if (SettingsDatas.KeyAutoSizeDelta.Value)
            {
                CompassInfos.SizeDelta = new Vector2(ScreenRect.sizeDelta.x * ((float)SettingsDatas.KeyAutoSizeDeltaRate.Value / 100), SettingsDatas.KeySizeDelta.Value.y);
            }
            else
            {
                CompassInfos.SizeDelta = SettingsDatas.KeySizeDelta.Value;
            }

            CompassHUD.Set(CompassInfos, SettingsDatas, CompassHUDSW);
            CompassFireHUD.Set(CompassFireDatas, SettingsDatas, CompassFireHUDSW);
            CompassStaticHUD.Set(CompassStaticDatas, SettingsDatas, CompassStaticHUDSW);

            if (HUDCore.HasPlayer)
            {
                Cam = HUDCore.YourPlayer.CameraPosition;

                CompassInfos.Angle = GetAngle(Cam.eulerAngles, NorthDirection);

                CompassFireDatas.CopyFrom(CompassInfos);

                CompassFireDatas.NorthVector = NorthVector;

                CompassFireDatas.PlayerPosition = Cam.position;

                CompassFireDatas.PlayerRight = Cam.right;

                CompassStaticDatas.CopyFrom(CompassFireDatas);

                CompassStaticDatas.AllPlayerItems = HUDCore.YourPlayer.Profile.Inventory.AllPlayerItems.Select(x => x.TemplateId).ToHashSet();

                if (CompassQuestCacheBool)
                {
                    ShowQuest(HUDCore.YourPlayer, HUDCore.TheWorld, Is231Up, ShowStatic);

                    CompassQuestCacheBool = false;
                }

                if (CompassExitCacheBool)
                {
                    ExfiltrationPoints = ShowExfiltration(HUDCore.YourPlayer, HUDCore.TheWorld, ShowStatic);

                    CompassExitCacheBool = false;
                }

                if (ExfiltrationPoints != null)
                {
                    CompassStaticDatas.ExfiltrationPoints = ExfiltrationPoints.Select(x => new ExfiltrationUIData()
                    {
                        NotPresent = x.Exfiltration.Status == EExfiltrationStatus.NotPresent,
                        UncompleteRequirements = x.Exfiltration.Status == EExfiltrationStatus.UncompleteRequirements,
                        Swtichs = x.Swtichs.Select(j => j.DoorState == EDoorState.Open).ToArray()
                    }).ToArray();
                }
            }
            else
            {
                ExfiltrationPoints = null;
                AirdropCount = 0;
                CompassQuestCacheBool = true;
            }
        }

        void ShowQuest(Player player, GameWorld world, bool is231up, Action<CompassStaticInfo> showstatic)
        {
            if (player is HideoutPlayer)
                return;

            object questData = Traverse.Create(player).Field("_questController").GetValue<object>();

            object quests = Traverse.Create(questData).Field("Quests").GetValue<object>();

            IList questsList = Traverse.Create(quests).Field("list_0").GetValue<IList>();

            List<LootItem> lootItemsList = Traverse.Create(Traverse.Create(world).Field("LootItems").GetValue<object>()).Field("list_0").GetValue<List<LootItem>>();

            Dictionary<string, LootItem> questItemsDictionary = lootItemsList.Where(x => x.Item.QuestItem).ToDictionary(x => x.TemplateId, x => x);

            foreach (object item in questsList)
            {
                if (Traverse.Create(item).Property("QuestStatus").GetValue<EQuestStatus>() != EQuestStatus.Started)
                    continue;

                object template = Traverse.Create(item).Property("Template").GetValue<object>();

                if (is231up && (player.Profile.Side == EPlayerSide.Savage ? 1 : 0) != Traverse.Create(template).Field("PlayerGroup").GetValue<int>() || !is231up && player.Profile.Side == EPlayerSide.Savage)
                    continue;

                string name = Traverse.Create(template).Property("NameLocaleKey").GetValue<string>();

                string traderId = Traverse.Create(template).Field("TraderId").GetValue<string>();

                object availableForFinishConditions = Traverse.Create(item).Property("AvailableForFinishConditions").GetValue<object>();

                IList availableForFinishConditionsList = Traverse.Create(availableForFinishConditions).Field("list_0").GetValue<IList>();

                foreach (object condition in availableForFinishConditionsList)
                {
                    if (condition is ConditionLeaveItemAtLocation)
                    {
                        ConditionLeaveItemAtLocation nowCondition = (ConditionLeaveItemAtLocation)condition;
                        string zoneId = nowCondition.zoneId;

                        if (ZoneHelp.TryGetValues(zoneId, out IEnumerable<PlaceItemTrigger> triggers))
                        {
                            foreach (var trigger in triggers)
                            {
                                CompassStaticInfo staticInfo = new CompassStaticInfo()
                                {
                                    Id = nowCondition.id,
                                    Where = trigger.transform.position,
                                    ZoneId = zoneId,
                                    Target = nowCondition.target,
                                    NameKey = name,
                                    DescriptionKey = nowCondition.id,
                                    TraderId = traderId,
                                    IsNotNecessary = !nowCondition.IsNecessary,
                                    InfoType = CompassStaticInfo.Type.ConditionLeaveItemAtLocation
                                };

                                showstatic(staticInfo);
                            }
                        }
                    }
                    else if (condition is ConditionPlaceBeacon)
                    {
                        ConditionPlaceBeacon nowCondition = (ConditionPlaceBeacon)condition;
                        string zoneId = nowCondition.zoneId;

                        if (ZoneHelp.TryGetValues(zoneId, out IEnumerable<PlaceItemTrigger> triggers))
                        {
                            foreach (var trigger in triggers)
                            {
                                CompassStaticInfo staticInfo = new CompassStaticInfo()
                                {
                                    Id = nowCondition.id,
                                    Where = trigger.transform.position,
                                    ZoneId = zoneId,
                                    Target = nowCondition.target,
                                    NameKey = name,
                                    DescriptionKey = nowCondition.id,
                                    TraderId = traderId,
                                    IsNotNecessary = !nowCondition.IsNecessary,
                                    InfoType = CompassStaticInfo.Type.ConditionPlaceBeacon
                                };

                                showstatic(staticInfo);
                            }
                        }
                    }
                    else if (condition is ConditionFindItem)
                    {
                        ConditionFindItem nowCondition = (ConditionFindItem)condition;
                        string[] itemIds = nowCondition.target;

                        foreach (string itemid in itemIds)
                        {
                            if (questItemsDictionary.TryGetValue(itemid, out LootItem lootitem))
                            {
                                CompassStaticInfo staticInfo = new CompassStaticInfo()
                                {
                                    Id = nowCondition.id,
                                    Where = lootitem.transform.position,
                                    NameKey = name,
                                    DescriptionKey = nowCondition.id,
                                    TraderId = traderId,
                                    IsNotNecessary = !nowCondition.IsNecessary,
                                    InfoType = CompassStaticInfo.Type.ConditionFindItem
                                };

                                showstatic(staticInfo);
                            }
                        }
                    }
                    else if (condition is ConditionCounterCreator)
                    {
                        object counter = Traverse.Create(condition).Field("counter").GetValue<object>();

                        ConditionCounterCreator beforeCondition = (ConditionCounterCreator)condition;

                        object conditions = Traverse.Create(counter).Property("conditions").GetValue<object>();

                        IList conditionsList = Traverse.Create(conditions).Field("list_0").GetValue<IList>();

                        foreach (object ccondition in conditionsList)
                        {
                            if (ccondition is ConditionVisitPlace)
                            {
                                ConditionVisitPlace nowCondition = (ConditionVisitPlace)ccondition;
                                string zoneId = nowCondition.target;

                                if (ZoneHelp.TryGetValues(zoneId, out IEnumerable<ExperienceTrigger> triggers))
                                {
                                    foreach (var trigger in triggers)
                                    {
                                        CompassStaticInfo staticInfo = new CompassStaticInfo()
                                        {
                                            Id = beforeCondition.id,
                                            Where = trigger.transform.position,
                                            ZoneId = zoneId,
                                            NameKey = name,
                                            DescriptionKey = beforeCondition.id,
                                            TraderId = traderId,
                                            IsNotNecessary = !beforeCondition.IsNecessary,
                                            InfoType = CompassStaticInfo.Type.ConditionVisitPlace
                                        };

                                        showstatic(staticInfo);
                                    }
                                }
                            }
                            else if (ccondition is ConditionInZone)
                            {
                                ConditionInZone nowCondition = (ConditionInZone)ccondition;
                                string[] zoneIds = nowCondition.zoneIds;

                                foreach (string zoneid in zoneIds)
                                {
                                    if (ZoneHelp.TryGetValues(zoneid, out IEnumerable<ExperienceTrigger> triggers))
                                    {
                                        foreach (var trigger in triggers)
                                        {
                                            CompassStaticInfo staticInfo = new CompassStaticInfo()
                                            {
                                                Id = beforeCondition.id,
                                                Where = trigger.transform.position,
                                                ZoneId = zoneid,
                                                NameKey = name,
                                                DescriptionKey = beforeCondition.id,
                                                TraderId = traderId,
                                                IsNotNecessary = !beforeCondition.IsNecessary,
                                                InfoType = CompassStaticInfo.Type.ConditionInZone
                                            };

                                            showstatic(staticInfo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        ExfiltrationData[] ShowExfiltration(Player player, GameWorld world, Action<CompassStaticInfo> showstatic)
        {
            if (player is HideoutPlayer)
                return null;
   
            object exfiltrationController = Traverse.Create(world).Property("ExfiltrationController").GetValue<object>();

            ExfiltrationPoint[] exfiltrationPoints;
            if (player.Profile.Side != EPlayerSide.Savage)
            {
                exfiltrationPoints = Traverse.Create(exfiltrationController).Method("EligiblePoints", new Type[] { typeof(Profile) }).GetValue<ExfiltrationPoint[]>(player.Profile);
                
            }
            else
            {
                exfiltrationPoints = Traverse.Create(exfiltrationController).Property("ScavExfiltrationPoints").GetValue<ScavExfiltrationPoint[]>().Where(x => x.EligibleIds.Contains(player.ProfileId)).ToArray();
            }

            List<ExfiltrationData> exfiltrationDatas = new List<ExfiltrationData>();

            for (int i = 0; i < exfiltrationPoints.Length; i++)
            {
                ExfiltrationPoint point = exfiltrationPoints[i];

                CompassStaticInfo staticInfo = new CompassStaticInfo()
                {
                    Id = string.Concat("EXFIL", i),
                    Where = point.transform.position,
                    ExIndex = i,
                    NameKey = point.Settings.Name,
                    DescriptionKey = "EXFIL",
                    InfoType = CompassStaticInfo.Type.Exfiltration
                };

                showstatic(staticInfo);

                Switch[] switchs = new Switch[0];

                if (point.Status == EExfiltrationStatus.UncompleteRequirements)
                {
                    switchs = Traverse.Create(point).Field("list_1").GetValue<List<Switch>>().ToArray();

                    for (int j = 0; j < switchs.Length; j++)
                    {
                        Switch @switch = switchs[j];

                        CompassStaticInfo staticInfo2 = new CompassStaticInfo()
                        {
                            Id = @switch.Id,
                            Where = @switch.transform.position,
                            ExIndex = i,
                            ExIndex2 = j,
                            NameKey = point.Settings.Name,
                            DescriptionKey = @switch.ExtractionZoneTip,
                            InfoType = CompassStaticInfo.Type.Switch
                        };

                        showstatic(staticInfo2);
                    }
                }

                exfiltrationDatas.Add(new ExfiltrationData(point, switchs));
            }

            return exfiltrationDatas.ToArray();
        }

        float GetAngle(Vector3 eulerangles, float northdirection)
        {
            float num = eulerangles.y - northdirection;

            if (num >= 0)
                return num;
            else
                return num + 360;
        }

        public class CompassData
        {
            public float Angle;

            public Vector2 SizeDelta;

            public float CompassX
            {
                get
                {
                    return -(Angle / 15 * 120);
                }
            }

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
                float num = Vector3.SignedAngle(lhs, NorthVector, Vector3.up);

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
            public HashSet<string> AllPlayerItems;

            public ExfiltrationUIData[] ExfiltrationPoints;

            public void CopyFrom(CompassStaticData data)
            {
                Angle = data.Angle;
                SizeDelta = data.SizeDelta;

                NorthVector = data.NorthVector;
                PlayerPosition = data.PlayerPosition;
                PlayerRight = data.PlayerRight;

                AllPlayerItems = data.AllPlayerItems;
                ExfiltrationPoints = data.ExfiltrationPoints;
            }
        }

        public class ExfiltrationUIData
        {
            public bool NotPresent;

            public bool UncompleteRequirements;

            public bool[] Swtichs;
        }

        public class ExfiltrationData
        {
            public ExfiltrationPoint Exfiltration;

            public Switch[] Swtichs;

            public ExfiltrationData(ExfiltrationPoint point, Switch[] swtichs)
            {
                Exfiltration = point;
                Swtichs = swtichs;
            }
        }

        public struct CompassFireInfo
        {
            public int Who;

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
            public ConfigEntry<bool> KeyCompassHUDSW;
            public ConfigEntry<bool> KeyAngleHUDSW;
            public ConfigEntry<bool> KeyCompassFireHUDSW;
            public ConfigEntry<bool> KeyCompassFireDirectionHUDSW;
            public ConfigEntry<bool> KeyCompassFireSilenced;
            public ConfigEntry<bool> KeyCompassFireDeadDestroy;
            public ConfigEntry<bool> KeyCompassStaticHUDSW;
            public ConfigEntry<bool> KeyCompassStaticAirdrop;
            public ConfigEntry<bool> KeyCompassStaticExfiltration;
            public ConfigEntry<bool> KeyCompassStaticQuest;
            public ConfigEntry<bool> KeyCompassStaticInfoHUDSW;
            public ConfigEntry<bool> KeyCompassStaticDistanceHUDSW;
            public ConfigEntry<bool> KeyCompassStaticHideRequirements;
            public ConfigEntry<bool> KeyCompassStaticHideOptional;
            public ConfigEntry<bool> KeyAutoSizeDelta;

            public ConfigEntry<Vector2> KeyAnchoredPosition;
            public ConfigEntry<Vector2> KeySizeDelta;
            public ConfigEntry<Vector2> KeyLocalScale;
            public ConfigEntry<Vector2> KeyCompassFireSizeDelta;
            public ConfigEntry<Vector2> KeyCompassFireOutlineSizeDelta;
            public ConfigEntry<Vector2> KeyCompassFireDirectionAnchoredPosition;
            public ConfigEntry<Vector2> KeyCompassFireDirectionScale;
            public ConfigEntry<Vector2> KeyCompasStaticInfoAnchoredPosition;
            public ConfigEntry<Vector2> KeyCompassStaticInfoScale;

            public ConfigEntry<float> KeyCompassFireHeight;
            public ConfigEntry<float> KeyCompassFireDistance;
            public ConfigEntry<float> KeyCompassFireActiveSpeed;
            public ConfigEntry<float> KeyCompassFireWaitSpeed;
            public ConfigEntry<float> KeyCompassFireToSmallSpeed;
            public ConfigEntry<float> KeyCompassFireSmallWaitSpeed;
            public ConfigEntry<float> KeyCompassStaticHeight;

            public ConfigEntry<int> KeyAutoSizeDeltaRate;
            public ConfigEntry<int> KeyCompassStaticCenterPointRange;

            public ConfigEntry<Color> KeyArrowColor;
            public ConfigEntry<Color> KeyAzimuthsColor;
            public ConfigEntry<Color> KeyAzimuthsAngleColor;
            public ConfigEntry<Color> KeyDirectionColor;
            public ConfigEntry<Color> KeyAngleColor;
            public ConfigEntry<Color> KeyCompassFireColor;
            public ConfigEntry<Color> KeyCompassFireOutlineColor;
            public ConfigEntry<Color> KeyCompassFireBossColor;
            public ConfigEntry<Color> KeyCompassFireBossOutlineColor;
            public ConfigEntry<Color> KeyCompassFireFollowerColor;
            public ConfigEntry<Color> KeyCompassFireFollowerOutlineColor;
            public ConfigEntry<Color> KeyCompassStaticNameColor;
            public ConfigEntry<Color> KeyCompassStaticDescriptionColor;
            public ConfigEntry<Color> KeyCompassStaticNecessaryColor;
            public ConfigEntry<Color> KeyCompassStaticRequirementsColor;
            public ConfigEntry<Color> KeyCompassStaticDistanceColor;
            public ConfigEntry<Color> KeyCompassStaticMetersColor;

            public ConfigEntry<FontStyles> KeyAzimuthsAngleStyles;
            public ConfigEntry<FontStyles> KeyDirectionStyles;
            public ConfigEntry<FontStyles> KeyAngleStyles;
            public ConfigEntry<FontStyles> KeyCompassFireDirectionStyles;
            public ConfigEntry<FontStyles> KeyCompassStaticNameStyles;
            public ConfigEntry<FontStyles> KeyCompassStaticDescriptionStyles;
            public ConfigEntry<FontStyles> KeyCompassStaticDistanceStyles;
        }
    }
}
#endif
