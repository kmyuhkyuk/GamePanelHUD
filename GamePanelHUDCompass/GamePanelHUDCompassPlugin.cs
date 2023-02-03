#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using System;
using HarmonyLib;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EFT;
using EFT.Quests;
using EFT.Interactive;
using GamePanelHUDCore;
using GamePanelHUDCompass.Patches;
using GamePanelHUDCore.Utils;
using System.Security.Policy;
using EFT.InventoryLogic;
using System.Linq;

namespace GamePanelHUDCompass
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCompass", "kmyuhkyuk-GamePanelHUDCompass", "2.4.1")]
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

        private readonly CompassInfo CompassInfos = new CompassInfo();

        private readonly TriggerInfo Triggers = new TriggerInfo();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassInfo, SettingsData> CompassHUD = new GamePanelHUDCorePlugin.HUDClass<CompassInfo, SettingsData>();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassInfo, SettingsData> CompassFireHUD = new GamePanelHUDCorePlugin.HUDClass<CompassInfo, SettingsData>();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassInfo, SettingsData> CompassStaticHUD = new GamePanelHUDCorePlugin.HUDClass<CompassInfo, SettingsData>();

        internal static float NorthDirection;

        internal static Vector3 NorthVector;

        private bool CompassHUDSW;

        private bool CompassFireHUDSW;

        private bool CompassStaticCache = true;

        private Transform Cam;

        private readonly SettingsData SettingsDatas = new SettingsData();

        private readonly ReflectionData ReflectionDatas = new ReflectionData();

        private List<CompassStaticInfo> Test = new List<CompassStaticInfo>(); 

        internal static GameObject FirePrefab { get; private set; }

        internal static GameObject StaticPrefab { get; private set; }

        internal static Action<CompassFireInfo> ShowFire;

        internal static Action<int> DestroyFire;

        internal static Action<TriggerWithId> AddTrigger;

        internal static Action<CompassStaticInfo> ShowStatic;

        private bool Is231Up = GamePanelHUDCorePlugin.HUDCoreClass.GameVersion > new Version("0.12.12.17349");

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDCompass");

            ModUpdateCheck.DrawNeedUpdate(Config, Info.Metadata.Version);

            const string mainSettings = "主设置 Main Settings";
            const string positionScaleSettings = "位置大小设置 Position Scale Settings";
            const string colorSettings = "颜色设置 Color Settings";
            const string fontStylesSettings = "字体样式设置 Font Styles Settings";
            const string speedSettings = "动画速度设置 Animation Speed Settings";
            const string otherSettings = "其他设置 Other Settings";

            SettingsDatas.KeyCompassHUDSW = Config.Bind<bool>(mainSettings, "罗盘指示栏显示 Compass HUD display", true);
            SettingsDatas.KeyAngleHUDSW = Config.Bind<bool>(mainSettings, "罗盘角度显示 Compass Angle HUD display", true);

            SettingsDatas.KeyCompassFireHUDSW = Config.Bind<bool>(mainSettings, "罗盘开火指示栏显示 Compass Fire HUD display", true);
            SettingsDatas.KeyCompassFireDirectionHUDSW = Config.Bind<bool>(mainSettings, "罗盘开火指示栏显示 Compass Fire Direction HUD display", true);
            SettingsDatas.KeyCompassFireSilenced = Config.Bind<bool>(mainSettings, "罗盘开火隐藏消音 Compass Fire Hide Silenced", true);
            SettingsDatas.KeyCompassFireDeadDestroy = Config.Bind<bool>(mainSettings, "罗盘开火死亡销毁 Compass Fire Dead Destroy", true);

            SettingsDatas.KeyAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "指示栏位置 Anchored Position", new Vector2(0, 0));
            SettingsDatas.KeySizeDelta = Config.Bind<Vector2>(positionScaleSettings, "指示栏高度 Size Delta", new Vector2(600, 90));
            SettingsDatas.KeyLocalScale = Config.Bind<Vector2>(positionScaleSettings, "指示栏大小 Local Scale", new Vector2(1, 1));

            SettingsDatas.KeyCompassFireSizeDelta = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火高度 Compass Fire Size Delta", new Vector2(25, 25));
            SettingsDatas.KeyCompassFireOutlineSizeDelta = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火轮廓高度 Compass Fire Outline Size Delta", new Vector2(26, 26));
            SettingsDatas.KeyCompassFireDirectionAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火方向位置 Compass Fire Direction Anchored Position", new Vector2(15, -63));
            SettingsDatas.KeyCompassFireDirectionScale = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火方向大小 Compass Fire Direction Local Scale", new Vector2(1, 1));

            SettingsDatas.KeyCompassFireActiveSpeed = Config.Bind<float>(speedSettings, "罗盘开火激活速度 Compass Fire Active Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SettingsDatas.KeyCompassFireWaitSpeed = Config.Bind<float>(speedSettings, "罗盘开火等待速度 Compass Fire Wait Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SettingsDatas.KeyCompassFireToSmallSpeed = Config.Bind<float>(speedSettings, "罗盘开火变小速度 Compass Fire To Small Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SettingsDatas.KeyCompassFireSmallWaitSpeed = Config.Bind<float>(speedSettings, "罗盘开火变小等待速度 Compass Fire Small Wait Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));

            SettingsDatas.KeyCompassFireHeight = Config.Bind<float>(positionScaleSettings, "罗盘开火高度 Compass Fire Height", 8);

            SettingsDatas.KeyAngleOffset = Config.Bind<float>(otherSettings, "角度偏移 Angle Offset", 0);
            SettingsDatas.KeyCompassFireDistance = Config.Bind<float>(otherSettings, "罗盘开火最大距离 Compass Fire Max Distance", 50, new ConfigDescription("Fire distance <= How many meters display", new AcceptableValueRange<float>(0, 1000)));

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

            SettingsDatas.KeyAzimuthsAngleStyles = Config.Bind<FontStyles>(fontStylesSettings, "刻度角度 Azimuths Angle", FontStyles.Normal);
            SettingsDatas.KeyDirectionStyles = Config.Bind<FontStyles>(fontStylesSettings, "方向 Direction", FontStyles.Bold);
            SettingsDatas.KeyAngleStyles = Config.Bind<FontStyles>(fontStylesSettings, "角度 Angle", FontStyles.Bold);

            SettingsDatas.KeyCompassFireDirectionStyles = Config.Bind<FontStyles>(fontStylesSettings, "罗盘开火方向 Compass Fire Direction", FontStyles.Normal);

            ReflectionDatas.RefQuestController = RefHelp.FieldRef<Player, object>.Create("_questController");
            ReflectionDatas.RefQuests = RefHelp.FieldRef<object, object>.Create(ReflectionDatas.RefQuestController.FieldType, "Quests");
            ReflectionDatas.RefQuestsList = RefHelp.FieldRef<object, object>.Create(ReflectionDatas.RefQuests.FieldType, "list_0");
            ReflectionDatas.RefConditionCounterCreatorType = RefHelp.FieldRef<ConditionCounterCreator, int>.Create("type");
            ReflectionDatas.RefConditionCounterCreatorCounter = RefHelp.FieldRef<ConditionCounterCreator, object>.Create("counter");
            ReflectionDatas.RefLootItems = RefHelp.FieldRef<GameWorld, object>.Create("LootItems");
            ReflectionDatas.RefLootItemsList = RefHelp.FieldRef<object, List<LootItem>>.Create(ReflectionDatas.RefLootItems.FieldType, "list_0");

            Type questsType = ReflectionDatas.RefQuestsList.FieldType;
            Type questType = questsType.GetGenericArguments()[0];

            ReflectionDatas.RefQuestStatus = RefHelp.PropertyRef<object, EQuestStatus>.Create(questType, "QuestStatus");
            ReflectionDatas.RefAvailableForFinishConditions = RefHelp.PropertyRef<object, object>.Create(questType, "AvailableForFinishConditions");
            ReflectionDatas.RefConditions = RefHelp.PropertyRef<object, object>.Create(ReflectionDatas.RefConditionCounterCreatorCounter.FieldType, "conditions");

            ReflectionDatas.RefConditionsList = RefHelp.FieldRef<object, object>.Create(ReflectionDatas.RefConditions.PropertyType, "list_0");

            ReflectionDatas.RefAvailableForFinishConditionsList = RefHelp.FieldRef<object, object>.Create(ReflectionDatas.RefAvailableForFinishConditions.PropertyType, "list_0");

            ReflectionDatas.RefTemplate = RefHelp.PropertyRef<object, object>.Create(questType, "Template");

            Type templateType = ReflectionDatas.RefTemplate.PropertyType;

            ReflectionDatas.RefLocationId = RefHelp.FieldRef<object, string>.Create(templateType, "LocationId");
            ReflectionDatas.RefTraderId = RefHelp.FieldRef<object, string>.Create(templateType, "TraderId");

            ReflectionDatas.RefNameLocaleKey = RefHelp.PropertyRef<object, string>.Create(templateType, "NameLocaleKey");

            if (Is231Up)
            {
                ReflectionDatas.RefPlayerGroup = RefHelp.FieldRef<object, int>.Create(templateType, "PlayerGroup");
            }

            AddTrigger = AddPoint;

            new LevelSettingsPatch().Enable();
            new InitiateShotPatch().Enable();
            new OnBeenKilledByAggressorPatch().Enable();
            new TriggerWithIdPatch().Enable();
            new ExperienceTriggerPatch().Enable();
            new AirdropSynchronizableObjectPatch().Enable();

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            BundleHelp.AssetData<GameObject> prefabs = GamePanelHUDCorePlugin.HUDCoreClass.LoadHUD("gamepanlcompasshud.bundle", "gamepanlcompasshud");

            FirePrefab = prefabs.Asset["fire"];
        }

        public void IUpdate()
        {
            CompassPlugin();
        }

        void CompassPlugin()
        {
            CompassHUDSW = HUDCore.AllHUDSW && Cam != null && HUDCore.HasPlayer && SettingsDatas.KeyCompassHUDSW.Value;
            CompassFireHUDSW = CompassHUDSW && SettingsDatas.KeyCompassFireHUDSW.Value;

            CompassHUD.Set(CompassInfos, SettingsDatas, CompassHUDSW);
            CompassFireHUD.Set(CompassInfos, SettingsDatas, CompassFireHUDSW);

            if (HUDCore.HasPlayer)
            {
                Cam = HUDCore.YourPlayer.CameraPosition;

                CompassInfos.NorthVector = NorthVector;

                CompassInfos.Angle = GetAngle(Cam.eulerAngles, NorthDirection, SettingsDatas.KeyAngleOffset.Value);

                CompassInfos.PlayerPosition = Cam.position;

                CompassInfos.PlayerRight = Cam.right;

                if (CompassStaticCache)
                {
                    ShowQuest(HUDCore.YourPlayer, HUDCore.TheWorld, Is231Up, ShowStatic);

                    CompassStaticCache = false;
                }
            }
            else
            {
                Triggers.Clear();

                CompassStaticCache = true;
            }
        }

        void ShowQuest(Player player, GameWorld theworld, bool is231up, Action<CompassStaticInfo> showstatic)
        {
            object questData = ReflectionDatas.RefQuestController.GetValue(player);

            object quests = ReflectionDatas.RefQuests.GetValue(questData);

            IList questsList = ReflectionDatas.RefQuestsList.GetValue(quests) as IList;

            object lootItems = ReflectionDatas.RefLootItems.GetValue(theworld);

            Dictionary<string, LootItem> lootItemsList = ReflectionDatas.RefLootItemsList.GetValue(lootItems).ToDictionary(x => x.ItemId, x => x);

            foreach (object item in questsList)
            {
                if (ReflectionDatas.RefQuestStatus.GetValue(item) == EQuestStatus.Started)
                {
                    object template = ReflectionDatas.RefTemplate.GetValue(item);

                    if (player.Location == LocalizedHelp.Localized(string.Concat(ReflectionDatas.RefLocationId.GetValue(template), " Name")) && (is231up ? (player.Profile.Side == EPlayerSide.Savage ? 1 : 0) == ReflectionDatas.RefPlayerGroup.GetValue(template) : true))
                    {
                        string name = ReflectionDatas.RefNameLocaleKey.GetValue(template);

                        string traderId = ReflectionDatas.RefTraderId.GetValue(template);

                        object availableForFinishConditions = ReflectionDatas.RefAvailableForFinishConditions.GetValue(item);

                        IList availableForFinishConditionsList = ReflectionDatas.RefAvailableForFinishConditionsList.GetValue(availableForFinishConditions) as IList;

                        foreach (object condition in availableForFinishConditionsList)
                        {
                            if (condition is ConditionLeaveItemAtLocation)
                            {
                                string zoneId = ((ConditionLeaveItemAtLocation)condition).zoneId;

                                if (Triggers.PlaceItemTriggerPoint.TryGetValue(zoneId, out Vector3 where))
                                {
                                    CompassStaticInfo staticInfo = new CompassStaticInfo()
                                    {
                                        Where = where,
                                        ZoneId = zoneId,
                                        NameKey = name,
                                        DescriptionKey = ((Condition)condition).id,
                                        TraderId = traderId,
                                        QuestType = CompassStaticInfo.Type.ConditionLeaveItemAtLocation
                                    };

                                    Test.Add(staticInfo);
                                }
                            }
                            else if (condition is ConditionFindItem)
                            {
                                string[] itemIds = ((ConditionFindItem)condition).target;

                                foreach (string itemid in itemIds)
                                {
                                    if (lootItemsList.TryGetValue(itemid, out LootItem lootitem) && lootitem.Item.QuestItem)
                                    {
                                        CompassStaticInfo staticInfo = new CompassStaticInfo()
                                        {
                                            Where = lootitem.transform.position,
                                            NameKey = name,
                                            DescriptionKey = ((Condition)condition).id,
                                            TraderId = traderId,
                                            QuestType = CompassStaticInfo.Type.ConditionFindItem
                                        };

                                        Test.Add(staticInfo);
                                    }
                                }
                            }
                            else if (condition is ConditionCounterCreator && ReflectionDatas.RefConditionCounterCreatorType.GetValue((ConditionCounterCreator)condition) == 6) //Type == Experience
                            {
                                object counter = ReflectionDatas.RefConditionCounterCreatorCounter.GetValue((ConditionCounterCreator)condition);

                                object conditions = ReflectionDatas.RefConditions.GetValue(counter);

                                IList conditionsList = ReflectionDatas.RefConditionsList.GetValue(conditions) as IList;

                                foreach (object counterCondition in conditionsList)
                                {
                                    if (counterCondition is ConditionVisitPlace)
                                    {
                                        string zoneId = ((ConditionVisitPlace)counterCondition).target;

                                        if (Triggers.ExperienceTriggerPoint.TryGetValue(zoneId, out Vector3 where))
                                        {
                                            CompassStaticInfo staticInfo = new CompassStaticInfo()
                                            {
                                                Where = where,
                                                ZoneId = zoneId,
                                                NameKey = name,
                                                DescriptionKey = ((Condition)condition).id,
                                                TraderId = traderId,
                                                QuestType = CompassStaticInfo.Type.ConditionCounterCreator
                                            };

                                            Test.Add(staticInfo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        float GetAngle(Vector3 eulerangles, float northdirection, float offset)
        {
            float num = eulerangles.y - northdirection + offset;

            if (num >= 0)
            {
                return num;
            }
            else
            {
                return num + 360;
            }
        }

        void AddPoint(TriggerWithId trigger)
        {
            string id = trigger.Id;
            Vector3 pos = trigger.transform.position;

            Dictionary<string, Vector3> point;

            if (trigger is ExperienceTrigger)
            {
                point = Triggers.ExperienceTriggerPoint;
            }
            else if (trigger is PlaceItemTrigger)
            {
                point = Triggers.PlaceItemTriggerPoint;
            }
            else
            {
                point = Triggers.QuestTriggerPoint;
            }

            if (!point.ContainsKey(id))
            {
                point.Add(id, pos);
            }
            else
            {
                point.Add(string.Concat(id, "(", trigger.transform.parent.name, ")"), pos);
            }
        }

        public class CompassInfo
        {
            public Vector3 NorthVector;

            public float Angle;

            public Vector3 PlayerPosition;

            public Vector3 PlayerRight;

            public float CompassX
            {
                get
                {
                    return -(Angle / 15 * 120);
                }
            }

            public float GetToAngle(Vector3 lhs, float offset)
            {
                float num = Vector3.SignedAngle(lhs, NorthVector, Vector3.up) + offset;

                if (num >= 0)
                {
                    return num;
                }
                else
                {
                    return num + 360;
                }
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
            public Vector3 Where;

            public string ZoneId;

            public bool IsAirdrop;

            public string NameKey;

            public string DescriptionKey;

            public string TraderId;

            public Type QuestType;

            public enum Type
            {
                ConditionLeaveItemAtLocation,
                ConditionFindItem,
                ConditionCounterCreator
            }
        }

        public class TriggerInfo
        {
            public Dictionary<string, Vector3> ExperienceTriggerPoint = new Dictionary<string, Vector3>();

            public Dictionary<string, Vector3> PlaceItemTriggerPoint = new Dictionary<string, Vector3>();

            public Dictionary<string, Vector3> QuestTriggerPoint = new Dictionary<string, Vector3>();

            public void Clear()
            {
                ExperienceTriggerPoint.Clear();
                PlaceItemTriggerPoint.Clear();
                QuestTriggerPoint.Clear();
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

            public ConfigEntry<Vector2> KeyAnchoredPosition;
            public ConfigEntry<Vector2> KeySizeDelta;
            public ConfigEntry<Vector2> KeyLocalScale;
            public ConfigEntry<Vector2> KeyCompassFireSizeDelta;
            public ConfigEntry<Vector2> KeyCompassFireOutlineSizeDelta;
            public ConfigEntry<Vector2> KeyCompassFireDirectionAnchoredPosition;
            public ConfigEntry<Vector2> KeyCompassFireDirectionScale;

            public ConfigEntry<float> KeyAngleOffset;
            public ConfigEntry<float> KeyCompassFireHeight;
            public ConfigEntry<float> KeyCompassFireDistance;
            public ConfigEntry<float> KeyCompassFireActiveSpeed;
            public ConfigEntry<float> KeyCompassFireWaitSpeed;
            public ConfigEntry<float> KeyCompassFireToSmallSpeed;
            public ConfigEntry<float> KeyCompassFireSmallWaitSpeed;

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

            public ConfigEntry<FontStyles> KeyAzimuthsAngleStyles;
            public ConfigEntry<FontStyles> KeyDirectionStyles;
            public ConfigEntry<FontStyles> KeyAngleStyles;
            public ConfigEntry<FontStyles> KeyCompassFireDirectionStyles;
        }

        public class ReflectionData
        {
            public RefHelp.FieldRef<Player, object> RefQuestController;
            public RefHelp.FieldRef<object, object> RefQuests;
            public RefHelp.FieldRef<object, object> RefQuestsList;
            public RefHelp.FieldRef<object, object> RefAvailableForFinishConditionsList;
            public RefHelp.FieldRef<object, string> RefLocationId;
            public RefHelp.FieldRef<object, int> RefPlayerGroup;
            public RefHelp.FieldRef<object, string> RefTraderId;
            public RefHelp.FieldRef<ConditionCounterCreator, int> RefConditionCounterCreatorType;
            public RefHelp.FieldRef<ConditionCounterCreator, object> RefConditionCounterCreatorCounter;
            public RefHelp.FieldRef<object, object> RefConditionsList;
            public RefHelp.FieldRef<GameWorld, object> RefLootItems;
            public RefHelp.FieldRef<object, List<LootItem>> RefLootItemsList;

            public RefHelp.PropertyRef<object, EQuestStatus> RefQuestStatus;
            public RefHelp.PropertyRef<object, object> RefAvailableForFinishConditions;
            public RefHelp.PropertyRef<object, object> RefTemplate;
            public RefHelp.PropertyRef<object, string> RefNameLocaleKey;
            public RefHelp.PropertyRef<object, object> RefConditions;
        }
    }
}
#endif
