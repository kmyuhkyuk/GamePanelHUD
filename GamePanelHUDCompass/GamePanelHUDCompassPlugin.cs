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
using EFT.InventoryLogic;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
using GamePanelHUDCore.Utils.Zone;
using GamePanelHUDCompass.Patches;

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

        private readonly CompassData CompassInfos = new CompassData();

        private readonly CompassFireData CompassFireDatas = new CompassFireData();

        //private readonly CompassStaticData CompassStaticDatas = new CompassStaticData();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassData, SettingsData> CompassHUD = new GamePanelHUDCorePlugin.HUDClass<CompassData, SettingsData>();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassFireData, SettingsData> CompassFireHUD = new GamePanelHUDCorePlugin.HUDClass<CompassFireData, SettingsData>();

        //internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassStaticData, SettingsData> CompassStaticHUD = new GamePanelHUDCorePlugin.HUDClass<CompassStaticData, SettingsData>();

        internal static float NorthDirection;

        internal static Vector3 NorthVector;

        private bool CompassHUDSW;

        private bool CompassFireHUDSW;

        //private bool CompassStaticCacheBool = true;

        private Transform Cam;

        private RectTransform ScreenRect;

        private readonly SettingsData SettingsDatas = new SettingsData();

        private readonly ReflectionData ReflectionDatas = new ReflectionData(); 

        //private List<CompassStaticInfo> Test = new List<CompassStaticInfo>(); 

        internal static GameObject FirePrefab { get; private set; }

        //internal static GameObject StaticPrefab { get; private set; }

        internal static Action<CompassFireInfo> ShowFire;

        internal static Action<int> DestroyFire;

        //internal static Action<CompassStaticInfo> ShowStatic;

        //private bool Is231Up = GamePanelHUDCorePlugin.HUDCoreClass.GameVersion > new Version("0.12.12.17349");

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
            SettingsDatas.KeyCompassFireDirectionHUDSW = Config.Bind<bool>(mainSettings, "罗盘开火指示栏显示 Compass Fire Direction HUD display", true);
            SettingsDatas.KeyCompassFireSilenced = Config.Bind<bool>(mainSettings, "罗盘开火隐藏消音 Compass Fire Hide Silenced", true);
            SettingsDatas.KeyCompassFireDeadDestroy = Config.Bind<bool>(mainSettings, "罗盘开火死亡销毁 Compass Fire Dead Destroy", true);
            SettingsDatas.KeyAutoSizeDelta = Config.Bind<bool>(mainSettings, "自动高度 Auto Size Delta", true);

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

            SettingsDatas.KeyAzimuthsAngleStyles = Config.Bind<FontStyles>(fontStylesSettings, "刻度角度 Azimuths Angle", FontStyles.Normal);
            SettingsDatas.KeyDirectionStyles = Config.Bind<FontStyles>(fontStylesSettings, "方向 Direction", FontStyles.Bold);
            SettingsDatas.KeyAngleStyles = Config.Bind<FontStyles>(fontStylesSettings, "角度 Angle", FontStyles.Bold);

            SettingsDatas.KeyCompassFireDirectionStyles = Config.Bind<FontStyles>(fontStylesSettings, "罗盘开火方向 Compass Fire Direction", FontStyles.Normal);

            ReflectionDatas.RefLootItems = RefHelp.FieldRef<GameWorld, object>.Create("LootItems");
            ReflectionDatas.RefLootItemsList = RefHelp.FieldRef<object, List<LootItem>>.Create(ReflectionDatas.RefLootItems.FieldType, "list_0");

            new LevelSettingsPatch().Enable();
            new PlayerShotPatch().Enable();
            new PlayerDeadPatch().Enable();

            /*if (Is231Up)
            {
                new AirdropSynchronizableObjectPatch().Enable();
            }*/

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            BundleHelp.AssetData<GameObject> prefabs = GamePanelHUDCorePlugin.HUDCoreClass.LoadHUD("gamepanlcompasshud.bundle", "gamepanlcompasshud");

            FirePrefab = prefabs.Asset["fire"];

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
            //CompassStaticHUD.Set(CompassStaticDatas, SettingsDatas, false);

            if (HUDCore.HasPlayer)
            {
                Cam = HUDCore.YourPlayer.CameraPosition;

                CompassInfos.Angle = GetAngle(Cam.eulerAngles, NorthDirection, SettingsDatas.KeyAngleOffset.Value);

                CompassFireDatas.Copy(CompassInfos);

                CompassFireDatas.NorthVector = NorthVector;

                CompassFireDatas.PlayerPosition = Cam.position;

                CompassFireDatas.PlayerRight = Cam.right;

                //CompassStaticDatas.Copy(CompassFireDatas);

                //CompassStaticDatas.AllPlayerItems = HUDCore.YourPlayer.Profile.Inventory.AllPlayerItems;

                /*if (CompassStaticCacheBool)
                {
                    List<LootItem> lootItemsList = ReflectionDatas.RefLootItemsList.GetValue(ReflectionDatas.RefLootItems.GetValue(HUDCore.TheWorld));

                    if (lootItemsList.Count > 0)
                    {
                        ShowQuest(HUDCore.YourPlayer, lootItemsList, Is231Up, null);

                        CompassStaticCacheBool = false;
                    }
                }*/
            }
            /*else
            {
                CompassStaticCacheBool = true;
            }*/
        }

        /*void ShowQuest(Player player, List<LootItem> lootitemslist, bool is231up, Action<CompassStaticInfo> showstatic)
        {
            object questData = Traverse.Create(player).Field("_questController").GetValue<object>();

            object quests = Traverse.Create(questData).Field("Quests").GetValue<object>();

            IList questsList = Traverse.Create(quests).Field("list_0").GetValue<IList>();

            Dictionary<string, LootItem> questItemsDictionary = lootitemslist.Where(x => x.Item.QuestItem).ToDictionary(x => x.TemplateId, x => x);

            foreach (object item in questsList)
            {
                if (Traverse.Create(item).Property("QuestStatus").GetValue<EQuestStatus>() == EQuestStatus.Started)
                {
                    object template = Traverse.Create(item).Property("Template").GetValue<object>();

                    if (is231up ? (player.Profile.Side == EPlayerSide.Savage ? 1 : 0) == Traverse.Create(template).Field("PlayerGroup").GetValue<int>() : true)
                    {
                        string name = Traverse.Create(template).Property("NameLocaleKey").GetValue<string>();

                        string traderId = Traverse.Create(template).Field("TraderId").GetValue<string>();

                        object availableForFinishConditions = Traverse.Create(item).Property("AvailableForFinishConditions").GetValue<object>();

                        IList availableForFinishConditionsList = Traverse.Create(availableForFinishConditions).Field("list_0").GetValue<IList>();

                        foreach (object condition in availableForFinishConditionsList)
                        {
                            if (condition is ConditionLeaveItemAtLocation)
                            {
                                string zoneId = ((ConditionLeaveItemAtLocation)condition).zoneId;

                                if (ZoneHelp.TryPlaceItem(zoneId, out Vector3 where))
                                {
                                    CompassStaticInfo staticInfo = new CompassStaticInfo()
                                    {
                                        Where = where,
                                        ZoneId = zoneId,
                                        Target = ((ConditionLeaveItemAtLocation)condition).target,
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
                                    if (questItemsDictionary.TryGetValue(itemid, out LootItem lootitem))
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
                            else if (condition is ConditionCounterCreator)
                            {
                                object counter = Traverse.Create(condition).Field("counter").GetValue<object>();

                                object conditions = Traverse.Create(counter).Property("conditions").GetValue<object>();

                                IList conditionsList = Traverse.Create(conditions).Field("list_0").GetValue<IList>();

                                foreach (object counterCondition in conditionsList)
                                {
                                    if (counterCondition is ConditionVisitPlace)
                                    {
                                        string zoneId = ((ConditionVisitPlace)counterCondition).target;

                                        if (ZoneHelp.TryExperience(zoneId, out Vector3 where))
                                        {
                                            CompassStaticInfo staticInfo = new CompassStaticInfo()
                                            {
                                                Where = where,
                                                ZoneId = zoneId,
                                                NameKey = name,
                                                DescriptionKey = ((Condition)condition).id,
                                                TraderId = traderId,
                                                QuestType = CompassStaticInfo.Type.ConditionVisitPlace
                                            };

                                            Test.Add(staticInfo);
                                        }
                                    }
                                    else if (condition is ConditionInZone)
                                    {
                                        string[] zoneIds = ((ConditionInZone)counterCondition).zoneIds;

                                        foreach (string zoneid in zoneIds)
                                        {
                                            if (ZoneHelp.TryExperience(zoneid, out Vector3 where))
                                            {
                                                CompassStaticInfo staticInfo = new CompassStaticInfo()
                                                {
                                                    Where = where,
                                                    ZoneId = zoneid,
                                                    NameKey = name,
                                                    DescriptionKey = ((Condition)condition).id,
                                                    TraderId = traderId,
                                                    QuestType = CompassStaticInfo.Type.ConditionInZone
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
        }

        void ShowExfiltration(GameWorld world, Action<CompassStaticInfo> showstatic)
        {
            object exfiltrationController = Traverse.Create(world).Property("ExfiltrationController").GetValue<object>();

            ExfiltrationPoint[] exfiltrationPoints = Traverse.Create(exfiltrationController).Property("ExfiltrationPoints").GetValue<ExfiltrationPoint[]>();

            foreach (ExfiltrationPoint point in exfiltrationPoints)
            {
            }
        }*/

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

            public void Copy(CompassData data)
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

        /*public class CompassStaticData : CompassFireData
        {
            public IEnumerable<Item> AllPlayerItems;

            public ExfiltrationPoint[] PlayerExfiltrationPoints;
        }*/

        public struct CompassFireInfo
        {
            public int Who;

            public Vector3 Where;

            public float Distance;

            public WildSpawnType Role;

            public bool IsSilenced;
        }

        /*public struct CompassStaticInfo
        {
            public Vector3 Where;

            public string ZoneId;

            public string[] Target;

            public string NameKey;

            public string DescriptionKey;

            public string TraderId;

            public Type QuestType;

            public enum Type
            {
                Airdrop,
                Exfiltration,
                ConditionLeaveItemAtLocation,
                ConditionFindItem,
                ConditionVisitPlace,
                ConditionInZone
            }
        }*/

        public class SettingsData
        {
            public ConfigEntry<bool> KeyCompassHUDSW;
            public ConfigEntry<bool> KeyAngleHUDSW;
            public ConfigEntry<bool> KeyCompassFireHUDSW;
            public ConfigEntry<bool> KeyCompassFireDirectionHUDSW;
            public ConfigEntry<bool> KeyCompassFireSilenced;
            public ConfigEntry<bool> KeyCompassFireDeadDestroy;
            public ConfigEntry<bool> KeyAutoSizeDelta;

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

            public ConfigEntry<int> KeyAutoSizeDeltaRate;

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
            public RefHelp.FieldRef<GameWorld, object> RefLootItems;
            public RefHelp.FieldRef<object, List<LootItem>> RefLootItemsList;
        }
    }
}
#endif
