#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using TMPro;
using GamePanelHUDCore;
using GamePanelHUDCompass.Patches;
using GamePanelHUDCore.Utils;
using EFT;
using System;

namespace GamePanelHUDCompass
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCompass", "kmyuhkyuk-GamePanelHUDCompass", "2.3.4")]
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

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassInfo, SettingsData> CompassHUD = new GamePanelHUDCorePlugin.HUDClass<CompassInfo, SettingsData>();

        internal static readonly GamePanelHUDCorePlugin.HUDClass<CompassInfo, SettingsData> CompassFireHUD = new GamePanelHUDCorePlugin.HUDClass<CompassInfo, SettingsData>();

        internal static float NorthDirection;

        private bool CompassHUDSW;

        private Transform Cam;

        private readonly SettingsData SettingsDatas = new SettingsData();

        internal static GameObject FirePrefab { get; private set; }

        internal static Action<CompassFireInfo> ShowFire;

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDCompass");

            ModUpdateCheck.DrawNeedUpdate(Config, Info.Metadata.Version);

            const string mainSettings = "主设置 Main Settings";
            const string positionScaleSettings = "位置大小设置 Position Scale Settings";
            const string colorSettings = "颜色设置 Color Settings";
            const string fontStylesSettings = "字体样式设置 Font Styles Settings";
            const string otherSettings = "其他设置 Other Settings";

            SettingsDatas.KeyCompassHUDSW = Config.Bind<bool>(mainSettings, "罗盘指示栏显示 Compass HUD display", true);
            SettingsDatas.KeyAngleHUDSW = Config.Bind<bool>(mainSettings, "罗盘角度显示 Compass Angle HUD display", true);

            SettingsDatas.KeyCompassFireHUDSW = Config.Bind<bool>(mainSettings, "罗盘开火指示栏显示 Compass Fire HUD display", true);
            SettingsDatas.KeyCompassFireDirectionHUDSW = Config.Bind<bool>(mainSettings, "罗盘开火指示栏显示 Compass Fire Direction HUD display", true);
            SettingsDatas.KeyCompassFireSilenced = Config.Bind<bool>(mainSettings, "罗盘开火隐藏消音 Compass Fire Hide Silenced", true);

            SettingsDatas.KeyAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "指示栏位置 Anchored Position", new Vector2(0, 0));
            SettingsDatas.KeySizeDelta = Config.Bind<Vector2>(positionScaleSettings, "指示栏高度 Size Delta", new Vector2(600, 90));
            SettingsDatas.KeyLocalScale = Config.Bind<Vector2>(positionScaleSettings, "指示栏大小 Local Scale", new Vector2(1, 1));

            SettingsDatas.KeyCompassFireSizeDelta = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火高度 Compass Fire Size Delta", new Vector2(25, 25));
            SettingsDatas.KeyCompassFireOutlineSizeDelta = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火轮廓高度 Compass Fire Outline Size Delta", new Vector2(26, 26));
            SettingsDatas.KeyCompassFireDirectionAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火方向位置 Compass Fire Direction Anchored Position", new Vector2(15, -63));
            SettingsDatas.KeyCompassFireDirectionScale = Config.Bind<Vector2>(positionScaleSettings, "罗盘开火方向大小 Compass Fire Direction Local Scale", new Vector2(1, 1));

            SettingsDatas.KeyAngleOffset = Config.Bind<float>(otherSettings, "角度偏移 Angle Offset", 0);

            SettingsDatas.KeyCompassFireHeight = Config.Bind<float>(positionScaleSettings, "罗盘开火高度 Compass Fire Height", 8);
            SettingsDatas.KeyCompassFireDistance = Config.Bind<float>(mainSettings, "罗盘开火最大距离 Compass Fire Max Distance", 50);

            SettingsDatas.KeyArrowColor = Config.Bind<Color>(colorSettings, "指针 Arrow", new Color(1f, 1f, 1f));
            SettingsDatas.KeyAzimuthsColor = Config.Bind<Color>(colorSettings, "刻度 Azimuths", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyAzimuthsAngleColor = Config.Bind<Color>(colorSettings, "刻度角度 Azimuths Angle", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyDirectionColor = Config.Bind<Color>(colorSettings, "方向 Direction", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyAngleColor = Config.Bind<Color>(colorSettings, "角度 Angle", new Color(0.8901961f, 0.8901961f, 0.8392157f));

            SettingsDatas.KeyCompassFireColor = Config.Bind<Color>(colorSettings, "罗盘开火 Compass Fire", new Color(1f, 0f, 0f));
            SettingsDatas.KeyCompassFireOutlineColor = Config.Bind<Color>(colorSettings, "罗盘开火轮廓 Compass Fire Outline", new Color(0.5f, 0f, 0f));
            SettingsDatas.KeyCompassFireBossColor = Config.Bind<Color>(colorSettings, "罗盘开火 Compass Boss Fire", new Color(1f, 0f, 0f));
            SettingsDatas.KeyCompassFireBossOutlineColor = Config.Bind<Color>(colorSettings, "罗盘开火轮廓 Compass Boss Fire Outline", new Color(0.5f, 0f, 0f));
            SettingsDatas.KeyCompassFireFollowerColor = Config.Bind<Color>(colorSettings, "罗盘开火 Compass Follower Fire", new Color(1f, 0f, 0f));
            SettingsDatas.KeyCompassFireBossOutlineColor = Config.Bind<Color>(colorSettings, "罗盘开火轮廓 Compass Boss Follower Outline", new Color(0.5f, 0f, 0f));

            SettingsDatas.KeyAzimuthsAngleStyles = Config.Bind<FontStyles>(fontStylesSettings, "刻度角度 Azimuths Angle", FontStyles.Normal);
            SettingsDatas.KeyDirectionStyles = Config.Bind<FontStyles>(fontStylesSettings, "方向 Direction", FontStyles.Bold);
            SettingsDatas.KeyAngleStyles = Config.Bind<FontStyles>(fontStylesSettings, "角度 Angle", FontStyles.Bold);

            SettingsDatas.KeyCompassFireDirectionStyles = Config.Bind<FontStyles>(fontStylesSettings, "罗盘开火方向 Compass Fire Direction", FontStyles.Normal);

            new LevelSettingsPatch().Enable();
            new FirePatch().Enable();

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

            CompassHUD.Set(CompassInfos, SettingsDatas, CompassHUDSW);
            CompassFireHUD.Set(CompassInfos, SettingsDatas, CompassHUDSW);

            if (HUDCore.HasPlayer)
            {
                Cam = HUDCore.YourPlayer.CameraPosition;

                CompassInfos.NorthDirection = NorthDirection;
                
                CompassInfos.Angle = GetAngle(Cam.eulerAngles, NorthDirection, SettingsDatas.KeyAngleOffset.Value);

                CompassInfos.PlayerPosition = Cam.position;

                CompassInfos.PlayerRight = Cam.right;
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

        public class CompassInfo
        {
            public float NorthDirection;

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
        }

        public struct CompassFireInfo
        {
            public int Who;

            public Vector3 Where;

            public float Distance;

            public WildSpawnType Role;

            public bool IsSilenced;
        }

        public class SettingsData
        {
            public ConfigEntry<bool> KeyCompassHUDSW;
            public ConfigEntry<bool> KeyAngleHUDSW;
            public ConfigEntry<bool> KeyCompassFireHUDSW;
            public ConfigEntry<bool> KeyCompassFireDirectionHUDSW;
            public ConfigEntry<bool> KeyCompassFireSilenced;

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
    }
}
#endif
