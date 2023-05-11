#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EFT;
using EFT.Quests;
using EFT.Interactive;
using EFT.InventoryLogic;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
using GamePanelHUDCore.Utils.Zone;
using GamePanelHUDCompass.Patches;

namespace GamePanelHUDCompass
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCompass", "kmyuhkyuk-GamePanelHUDCompass", "2.6.4")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class GamePanelHUDCompassPlugin : BaseUnityPlugin, IUpdate
    {
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

        private readonly CompassData CompassInfos = new CompassData();

        private readonly CompassFireData CFData = new CompassFireData();

        private readonly CompassStaticData CSData = new CompassStaticData();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassData, SettingsData> CompassHUD = new GamePanelHUDCorePlugin.HUDClass<CompassData, SettingsData>();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassFireData, SettingsData> CompassFireHUD = new GamePanelHUDCorePlugin.HUDClass<CompassFireData, SettingsData>();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassStaticData, SettingsData> CompassStaticHUD = new GamePanelHUDCorePlugin.HUDClass<CompassStaticData, SettingsData>();

        internal static float NorthDirection;

        internal static Vector3 NorthVector;

        private bool CompassHUDSw;

        private bool CompassFireHUDSw;

        private bool CompassStaticHUDSw;

        private bool CompassStaticCacheBool;

        private Transform Cam;

        private RectTransform ScreenRect;

        private readonly SettingsData SetData = new SettingsData();

        private readonly ReflectionData RefData = new ReflectionData();

        internal static readonly List<List<string>> Airdrops = new List<List<string>>();

        internal static GameObject FirePrefab { get; private set; }

        internal static GameObject StaticPrefab { get; private set; }

        internal static Action<CompassFireInfo> ShowFire;

        internal static Action<string> DestroyFire;

        internal static Action<CompassStaticInfo> ShowStatic;

        internal static Action<string> DestroyStatic;

        private static readonly bool Is350Up = GamePanelHUDCorePlugin.HUDCoreClass.GameVersion > new Version("0.13.0.21734");

        private void Start()
        {
            Logger.LogInfo("Loaded: KmYuHkYuk-GamePanelHUDCompass");

            ModUpdateCheck.DrawCheck(this);

            const string mainSettings = "主设置 Main Settings";
            const string questSettings = "任务显示设置 Quest Display Settings";
            const string positionScaleSettings = "位置大小设置 Position Scale Settings";
            const string colorSettings = "颜色设置 Color Settings";
            const string fontStylesSettings = "字体样式设置 Font Styles Settings";
            const string speedSettings = "动画速度设置 Animation Speed Settings";
            const string rateSettings = "率设置  Rate Settings";
            const string otherSettings = "其他设置 Other Settings";

            SetData.KeyCompassHUDSw = Config.Bind<bool>(mainSettings, "罗盘指示栏显示 Compass HUD display", true);
            SetData.KeyAngleHUDSw = Config.Bind<bool>(mainSettings, "罗盘角度显示 Compass Angle HUD display", true);
            SetData.KeyCompassFireHUDSw = Config.Bind<bool>(mainSettings, "罗盘开火指示栏显示 Compass Fire HUD display", true);
            SetData.KeyCompassFireDirectionHUDSw = Config.Bind<bool>(mainSettings, "罗盘开火方向指示栏显示 Compass Fire Direction HUD display", true);
            SetData.KeyCompassFireSilenced = Config.Bind<bool>(mainSettings, "罗盘开火隐藏消音 Compass Fire Hide Silenced", true);
            SetData.KeyCompassFireDeadDestroy = Config.Bind<bool>(mainSettings, "罗盘开火死亡销毁 Compass Fire Dead Destroy", true);
            SetData.KeyCompassStaticHUDSw = Config.Bind<bool>(mainSettings, "罗盘静态指示栏显示 Compass Static HUD display", true);
            SetData.KeyCompassStaticAirdrop = Config.Bind<bool>(mainSettings, "罗盘静态空投显示 Compass Static Airdrop display", true);
            SetData.KeyCompassStaticExfiltration = Config.Bind<bool>(mainSettings, "罗盘静态撤离点显示 Compass Static Exfiltration display", true);
            SetData.KeyCompassStaticQuest = Config.Bind<bool>(mainSettings, "罗盘静态任务显示 Compass Static Quest display", true);
            SetData.KeyCompassStaticInfoHUDSw = Config.Bind<bool>(mainSettings, "罗盘静态信息显示 Compass Static Info HUD display", true);
            SetData.KeyCompassStaticDistanceHUDSw = Config.Bind<bool>(mainSettings, "罗盘静态距离显示 Compass Static Distance HUD display", true);
            SetData.KeyCompassStaticHideRequirements = Config.Bind<bool>(mainSettings, "罗盘静态隐藏需求 Compass Static Hide Requirements", false);
            SetData.KeyCompassStaticHideOptional = Config.Bind<bool>(mainSettings, "罗盘静态隐藏可选项 Compass Static Hide Optional", false);
            SetData.KeyCompassStaticHideSearchedAirdrop = Config.Bind<bool>(mainSettings, "罗盘静态隐藏搜索过空投 Compass Static Hide Already Searched Airdrop", true);
            SetData.KeyAutoSizeDelta = Config.Bind<bool>(mainSettings, "自动高度 Auto Size Delta", true);

            SetData.KeyConditionFindItem = Config.Bind<bool>(questSettings, "FindItem", true);
            SetData.KeyConditionLeaveItemAtLocation = Config.Bind<bool>(questSettings, "LeaveItemAtLocation", true);
            SetData.KeyConditionPlaceBeacon = Config.Bind<bool>(questSettings, "PlaceBeacon", true);
            SetData.KeyConditionVisitPlace = Config.Bind<bool>(questSettings, "VisitPlace", true);
            SetData.KeyConditionInZone = Config.Bind<bool>(questSettings, "InZone", true);

            SetData.KeyAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "指示栏位置 Anchored Position", new Vector2(0, 0));
            SetData.KeySizeDelta = Config.Bind<Vector2>(positionScaleSettings, "指示栏高度 Size Delta", new Vector2(600, 90));
            SetData.KeyLocalScale = Config.Bind<Vector2>(positionScaleSettings, "指示栏大小 Local Scale", new Vector2(1, 1));
            SetData.KeyCompassFireSizeDelta = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火高度 Compass Fire Size Delta", new Vector2(25, 25));
            SetData.KeyCompassFireOutlineSizeDelta = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火轮廓高度 Compass Fire Outline Size Delta", new Vector2(26, 26));
            SetData.KeyCompassFireDirectionAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火方向位置 Compass Fire Direction Anchored Position", new Vector2(15, -63));
            SetData.KeyCompassFireDirectionScale = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火方向大小 Compass Fire Direction Local Scale", new Vector2(1, 1));
            SetData.KeyCompassStaticInfoAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "罗盘静态信息位置 Compass Static Info Anchored Position", new Vector2(0, -15));
            SetData.KeyCompassStaticInfoScale = Config.Bind<Vector2>(positionScaleSettings, "罗盘静态信息大小 Compass Static Info Local Scale", new Vector2(1, 1));

            SetData.KeyCompassFireActiveSpeed = Config.Bind<float>(speedSettings, "罗盘开火激活速度 Compass Fire Active Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SetData.KeyCompassFireWaitSpeed = Config.Bind<float>(speedSettings, "罗盘开火等待速度 Compass Fire Wait Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SetData.KeyCompassFireToSmallSpeed = Config.Bind<float>(speedSettings, "罗盘开火变小速度 Compass Fire To Small Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SetData.KeyCompassFireSmallWaitSpeed = Config.Bind<float>(speedSettings, "罗盘开火变小等待速度 Compass Fire Small Wait Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));

            SetData.KeyCompassFireHeight = Config.Bind<float>(positionScaleSettings, "罗盘开火高度 Compass Fire Height", 8);
            SetData.KeyCompassStaticHeight = Config.Bind<float>(positionScaleSettings, "罗盘静态高度 Compass Static Height", 5);

            SetData.KeyCompassFireDistance = Config.Bind<float>(otherSettings, "罗盘开火最大距离 Compass Fire Max Distance", 50, new ConfigDescription("Fire distance <= How many meters display", new AcceptableValueRange<float>(0, 1000)));
            SetData.KeyCompassStaticCenterPointRange = Config.Bind<int>(otherSettings, "罗盘静态中心点范围 Compass Static Center Point Range", 20);

            SetData.KeyAutoSizeDeltaRate = Config.Bind<int>(rateSettings, "自动高度比率 Auto Size Delta Rate", 30, new ConfigDescription("Screen percentage", new AcceptableValueRange<int>(0, 100)));

            SetData.KeyArrowColor = Config.Bind<Color>(colorSettings, "指针 Arrow", new Color(1f, 1f, 1f));
            SetData.KeyAzimuthsColor = Config.Bind<Color>(colorSettings, "刻度 Azimuths", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SetData.KeyAzimuthsAngleColor = Config.Bind<Color>(colorSettings, "刻度角度 Azimuths Angle", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SetData.KeyDirectionColor = Config.Bind<Color>(colorSettings, "方向 Direction", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SetData.KeyAngleColor = Config.Bind<Color>(colorSettings, "角度 Angle", new Color(0.8901961f, 0.8901961f, 0.8392157f));

            SetData.KeyCompassFireColor = Config.Bind<Color>(colorSettings, "罗盘开火 Compass Fire", new Color(1f, 0f, 0f));
            SetData.KeyCompassFireOutlineColor = Config.Bind<Color>(colorSettings, "罗盘开火轮廓 Compass Fire Outline", new Color(0.5f, 0f, 0f));
            SetData.KeyCompassFireBossColor = Config.Bind<Color>(colorSettings, "罗盘开火 Compass Boss Fire", new Color(1f, 0.5f, 0f));
            SetData.KeyCompassFireBossOutlineColor = Config.Bind<Color>(colorSettings, "罗盘开火轮廓 Compass Boss Fire Outline", new Color(1f, 0.3f, 0f));
            SetData.KeyCompassFireFollowerColor = Config.Bind<Color>(colorSettings, "罗盘开火 Compass Follower Fire", new Color(0f, 1f, 1f));
            SetData.KeyCompassFireFollowerOutlineColor = Config.Bind<Color>(colorSettings, "罗盘开火轮廓 Compass Follower Outline", new Color(0f, 0.7f, 1f));
            SetData.KeyCompassStaticNameColor = Config.Bind<Color>(colorSettings, "罗盘静态名字 Compass Static Name", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SetData.KeyCompassStaticDescriptionColor = Config.Bind<Color>(colorSettings, "罗盘静态说明 Compass Static Description", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SetData.KeyCompassStaticNecessaryColor = Config.Bind<Color>(colorSettings, "罗盘静态可选项 Compass Static Optional", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SetData.KeyCompassStaticRequirementsColor = Config.Bind<Color>(colorSettings, "罗盘静态需求 Compass Static Requirements", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SetData.KeyCompassStaticDistanceColor = Config.Bind<Color>(colorSettings, "罗盘静态距离 Compass Static Distance", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SetData.KeyCompassStaticMetersColor = Config.Bind<Color>(colorSettings, "罗盘静态米 Compass Static Meters", new Color(0.8901961f, 0.8901961f, 0.8392157f));

            SetData.KeyAzimuthsAngleStyles = Config.Bind<FontStyles>(fontStylesSettings, "刻度角度 Azimuths Angle", FontStyles.Normal);
            SetData.KeyDirectionStyles = Config.Bind<FontStyles>(fontStylesSettings, "方向 Direction", FontStyles.Bold);
            SetData.KeyAngleStyles = Config.Bind<FontStyles>(fontStylesSettings, "角度 Angle", FontStyles.Bold);
            SetData.KeyCompassFireDirectionStyles = Config.Bind<FontStyles>(fontStylesSettings, "罗盘开火方向 Compass Fire Direction", FontStyles.Normal);
            SetData.KeyCompassStaticNameStyles = Config.Bind<FontStyles>(fontStylesSettings, "罗盘静态名字 Compass Static Name", FontStyles.Bold);
            SetData.KeyCompassStaticDescriptionStyles = Config.Bind<FontStyles>(fontStylesSettings, "罗盘静态说明 Compass Static Description", FontStyles.Normal);
            SetData.KeyCompassStaticDistanceStyles = Config.Bind<FontStyles>(fontStylesSettings, "罗盘静态距离 Compass Static Distance", FontStyles.Bold);

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            RefData.RefEquipment = RefHelp.FieldRef<InventoryClass, object>.Create("Equipment");
            RefData.RefQuestRaidItems = RefHelp.FieldRef<InventoryClass, object>.Create("QuestRaidItems");
            RefData.RefSlots = RefHelp.FieldRef<object, Slot[]>.Create(RefData.RefEquipment.FieldType, "Slots");
            RefData.RefGrids = RefHelp.FieldRef<object, object[]>.Create(RefHelp.GetEftType(x => x.GetMethod("TryGetLastForbiddenItem", BindingFlags.DeclaredOnly | flags) != null), "Grids");

            RefData.RefItems = RefHelp.PropertyRef<object, IEnumerable<Item>>.Create(RefData.RefGrids.FieldType.GetElementType(), "Items");

            new LevelSettingsPatch().Enable();
            new PlayerShotPatch().Enable();
            new PlayerDeadPatch().Enable();
            new OnConditionValueChangedPatch().Enable();

            if (Is350Up)
            {
                new AirdropBoxPatch().Enable();
            }

            GamePanelHUDCorePlugin.HUDCoreClass.WorldStart += (GameWorld) => CompassStaticCacheBool = true;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            GamePanelHUDCorePlugin.HUDCoreClass.AssetData<GameObject> prefabs = GamePanelHUDCorePlugin.HUDCoreClass.LoadHUD("gamepanelcompasshud.bundle", "GamePanelCompassHUD");

            FirePrefab = prefabs.Asset["Fire"];

            StaticPrefab = prefabs.Asset["Point"];

            ScreenRect = GamePanelHUDCorePlugin.HUDCoreClass.GamePanelHUDPublic.GetComponent<RectTransform>();
        }

        public void IUpdate()
        {
            CompassPlugin();
        }

        void CompassPlugin()
        {
            CompassHUDSw = HUDCore.AllHUDSw && Cam != null && HUDCore.HasPlayer && SetData.KeyCompassHUDSw.Value;
            CompassFireHUDSw = CompassHUDSw && SetData.KeyCompassFireHUDSw.Value;
            CompassStaticHUDSw = CompassHUDSw && SetData.KeyCompassStaticHUDSw.Value;

            if (SetData.KeyAutoSizeDelta.Value)
            {
                CompassInfos.SizeDelta = new Vector2(ScreenRect.sizeDelta.x * ((float)SetData.KeyAutoSizeDeltaRate.Value / 100), SetData.KeySizeDelta.Value.y);
            }
            else
            {
                CompassInfos.SizeDelta = SetData.KeySizeDelta.Value;
            }

            CompassHUD.Set(CompassInfos, SetData, CompassHUDSw);
            CompassFireHUD.Set(CFData, SetData, CompassFireHUDSw);
            CompassStaticHUD.Set(CSData, SetData, CompassStaticHUDSw);

            if (HUDCore.HasPlayer)
            {
                Cam = HUDCore.YourPlayer.CameraPosition;

                CompassInfos.Angle = GetAngle(Cam.eulerAngles, NorthDirection);

                CFData.CopyFrom(CompassInfos);

                CFData.NorthVector = NorthVector;

                CFData.PlayerPosition = Cam.position;

                CFData.PlayerRight = Cam.right;

                CSData.CopyFrom(CFData);

                CSData.YourProfileId = HUDCore.YourPlayer.ProfileId;

                CSData.Airdrops = Airdrops;

                //Performance Optimization
                if (Time.frameCount % 20 == 0)
                {
                    HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                    Slot[] slots = RefData.RefSlots.GetValue(RefData.RefEquipment.GetValue(HUDCore.YourPlayer.Profile.Inventory));

                    foreach (Slot slot in new[] { slots[6], slots[7], slots[8], slots[10] })
                    {
                        Item gear = slot.ContainedItem;

                        if (gear == null)
                            continue;

                        foreach (object grid in RefData.RefGrids.GetValue(gear))
                        {
                            foreach (Item item in RefData.RefItems.GetValue(grid))
                            {
                                hashSet.Add(item.TemplateId);
                            }
                        }
                    }

                    foreach (object grid in RefData.RefGrids.GetValue(RefData.RefQuestRaidItems.GetValue(HUDCore.YourPlayer.Profile.Inventory)))
                    {
                        foreach (Item item in RefData.RefItems.GetValue(grid))
                        {
                            hashSet.Add(item.TemplateId);
                        }
                    }

                    CSData.EquipmentAndQuestRaidItems = hashSet;
                }

                if (CompassStaticCacheBool)
                {
                    ShowQuest(HUDCore.YourPlayer, HUDCore.TheWorld, Is350Up, ShowStatic);

                    CSData.ExfiltrationPoints = ShowExfiltration(HUDCore.YourPlayer, HUDCore.TheWorld, ShowStatic);

                    CompassStaticCacheBool = false;
                }
            }
            else
            {
                Airdrops.Clear();
                CSData.EquipmentAndQuestRaidItems = null;
                CSData.ExfiltrationPoints = null;
            }
        }

        void ShowQuest(Player player, GameWorld world, bool is231Up, Action<CompassStaticInfo> showStatic)
        {
            if (player is HideoutPlayer)
                return;

            object questData = Traverse.Create(player).Field("_questController").GetValue<object>();

            object quests = Traverse.Create(questData).Field("Quests").GetValue<object>();

            IList questsList = Traverse.Create(quests).Field("list_0").GetValue<IList>();

            List<LootItem> lootItemsList = Traverse.Create(Traverse.Create(world).Field("LootItems").GetValue<object>()).Field("list_0").GetValue<List<LootItem>>();

            Tuple<string, LootItem>[] questItems = lootItemsList.Where(x => x.Item.QuestItem).Select(x => new Tuple<string, LootItem>(x.TemplateId, x)).ToArray();

            foreach (object item in questsList)
            {
                if (Traverse.Create(item).Property("QuestStatus").GetValue<EQuestStatus>() != EQuestStatus.Started)
                    continue;

                object template = Traverse.Create(item).Property("Template").GetValue<object>();

                if (is231Up && (player.Profile.Side == EPlayerSide.Savage ? 1 : 0) != Traverse.Create(template).Field("PlayerGroup").GetValue<int>() || !is231Up && player.Profile.Side == EPlayerSide.Savage)
                    continue;

                string nameKey = Traverse.Create(template).Property("NameLocaleKey").GetValue<string>();

                string traderId = Traverse.Create(template).Field("TraderId").GetValue<string>();

                object availableForFinishConditions = Traverse.Create(item).Property("AvailableForFinishConditions").GetValue<object>();

                IList availableForFinishConditionsList = Traverse.Create(availableForFinishConditions).Field("list_0").GetValue<IList>();

                foreach (object condition in availableForFinishConditionsList)
                {
                    if (condition is ConditionLeaveItemAtLocation location)
                    {
                        string zoneId = location.zoneId;

                        if (ZoneHelp.TryGetValues(zoneId, out IEnumerable<PlaceItemTrigger> triggers))
                        {
                            foreach (var trigger in triggers)
                            {
                                CompassStaticInfo staticInfo = new CompassStaticInfo()
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
                    }
                    else if (condition is ConditionPlaceBeacon beacon)
                    {
                        string zoneId = beacon.zoneId;

                        if (ZoneHelp.TryGetValues(zoneId, out IEnumerable<PlaceItemTrigger> triggers))
                        {
                            foreach (var trigger in triggers)
                            {
                                CompassStaticInfo staticInfo = new CompassStaticInfo()
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
                    }
                    else if (condition is ConditionFindItem findItem)
                    {
                        string[] itemIds = findItem.target;

                        foreach (string itemId in itemIds)
                        {
                            foreach (var questItem in questItems)
                            {
                                if (questItem.Item1.Equals(itemId, StringComparison.OrdinalIgnoreCase))
                                {
                                    CompassStaticInfo staticInfo = new CompassStaticInfo()
                                    {
                                        Id = findItem.id,
                                        Where = questItem.Item2.transform.position,
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
                    }
                    else if (condition is ConditionCounterCreator counterCreator)
                    {
                        object counter = Traverse.Create(counterCreator).Field("counter").GetValue<object>();

                        object conditions = Traverse.Create(counter).Property("conditions").GetValue<object>();

                        IList conditionsList = Traverse.Create(conditions).Field("list_0").GetValue<IList>();

                        foreach (object condition2 in conditionsList)
                        {
                            if (condition2 is ConditionVisitPlace place)
                            {
                                string zoneId = place.target;

                                if (ZoneHelp.TryGetValues(zoneId, out IEnumerable<ExperienceTrigger> triggers))
                                {
                                    foreach (var trigger in triggers)
                                    {
                                        CompassStaticInfo staticInfo = new CompassStaticInfo()
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
                            }
                            else if (condition2 is ConditionInZone inZone)
                            {
                                string[] zoneIds = inZone.zoneIds;

                                foreach (string zoneId in zoneIds)
                                {
                                    if (ZoneHelp.TryGetValues(zoneId, out IEnumerable<ExperienceTrigger> triggers))
                                    {
                                        foreach (var trigger in triggers)
                                        {
                                            CompassStaticInfo staticInfo = new CompassStaticInfo()
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
                            }
                        }
                    }
                }
            }
        }

        ExfiltrationData[] ShowExfiltration(Player player, GameWorld world, Action<CompassStaticInfo> showStatic)
        {
            if (player is HideoutPlayer)
                return null;
   
            object exfiltrationController = Traverse.Create(world).Property("ExfiltrationController").GetValue<object>();

            ExfiltrationPoint[] exfiltrationPoints;
            if (player.Profile.Side != EPlayerSide.Savage)
            {
                exfiltrationPoints = Traverse.Create(exfiltrationController).Method("EligiblePoints", new[] { typeof(Profile) }).GetValue<ExfiltrationPoint[]>(player.Profile);
                
            }
            else
            {
                exfiltrationPoints = Traverse.Create(exfiltrationController).Property("ScavExfiltrationPoints").GetValue<ScavExfiltrationPoint[]>().Where(x => x.EligibleIds.Contains(player.ProfileId)).ToArray<ExfiltrationPoint>();
            }

            List<ExfiltrationData> exfiltrationList = new List<ExfiltrationData>();

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

                showStatic(staticInfo);

                Switch[] switchs = Array.Empty<Switch>();

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

                        showStatic(staticInfo2);
                    }
                }

                exfiltrationList.Add(new ExfiltrationData(point, switchs));
            }

            return exfiltrationList.ToArray();
        }

        float GetAngle(Vector3 eulerAngles, float northDirection)
        {
            float num = eulerAngles.y - northDirection;

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
            public ExfiltrationData[] ExfiltrationPoints;

            public HashSet<string> EquipmentAndQuestRaidItems;

            public List<List<string>> Airdrops;

            public string YourProfileId;

            public bool HasEquipmentAndQuestRaidItems => EquipmentAndQuestRaidItems != null;

            public void ExfiltrationGetStatus(int index, out bool notPresent, out bool requirements)
            {
                ExfiltrationData point = ExfiltrationPoints[index];

                EExfiltrationStatus status = point.Exfiltration.Status;

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
            public ExfiltrationPoint Exfiltration;

            public Switch[] Switchs;

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

        public class ReflectionData
        {
            public RefHelp.FieldRef<InventoryClass, object> RefEquipment;
            public RefHelp.FieldRef<InventoryClass, object> RefQuestRaidItems;
            public RefHelp.FieldRef<object, Slot[]> RefSlots;
            public RefHelp.FieldRef<object, object[]> RefGrids;

            public RefHelp.PropertyRef<object, IEnumerable<Item>> RefItems;
        }

        public class SettingsData
        {
            public ConfigEntry<bool> KeyCompassHUDSw;
            public ConfigEntry<bool> KeyAngleHUDSw;
            public ConfigEntry<bool> KeyCompassFireHUDSw;
            public ConfigEntry<bool> KeyCompassFireDirectionHUDSw;
            public ConfigEntry<bool> KeyCompassFireSilenced;
            public ConfigEntry<bool> KeyCompassFireDeadDestroy;
            public ConfigEntry<bool> KeyCompassStaticHUDSw;
            public ConfigEntry<bool> KeyCompassStaticAirdrop;
            public ConfigEntry<bool> KeyCompassStaticExfiltration;
            public ConfigEntry<bool> KeyCompassStaticQuest;
            public ConfigEntry<bool> KeyCompassStaticInfoHUDSw;
            public ConfigEntry<bool> KeyCompassStaticDistanceHUDSw;
            public ConfigEntry<bool> KeyCompassStaticHideRequirements;
            public ConfigEntry<bool> KeyCompassStaticHideOptional;
            public ConfigEntry<bool> KeyCompassStaticHideSearchedAirdrop;
            public ConfigEntry<bool> KeyAutoSizeDelta;

            public ConfigEntry<bool> KeyConditionFindItem;
            public ConfigEntry<bool> KeyConditionLeaveItemAtLocation;
            public ConfigEntry<bool> KeyConditionPlaceBeacon;
            public ConfigEntry<bool> KeyConditionVisitPlace;
            public ConfigEntry<bool> KeyConditionInZone;

            public ConfigEntry<Vector2> KeyAnchoredPosition;
            public ConfigEntry<Vector2> KeySizeDelta;
            public ConfigEntry<Vector2> KeyLocalScale;
            public ConfigEntry<Vector2> KeyCompassFireSizeDelta;
            public ConfigEntry<Vector2> KeyCompassFireOutlineSizeDelta;
            public ConfigEntry<Vector2> KeyCompassFireDirectionAnchoredPosition;
            public ConfigEntry<Vector2> KeyCompassFireDirectionScale;
            public ConfigEntry<Vector2> KeyCompassStaticInfoAnchoredPosition;
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
