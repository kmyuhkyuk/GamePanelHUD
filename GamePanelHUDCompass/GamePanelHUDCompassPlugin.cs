#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using TMPro;
using GamePanelHUDCore;
using GamePanelHUDCompass.Patches;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCompass
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCompass", "kmyuhkyuk-GamePanelHUDCompass", "2.3.0")]
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

        internal static readonly GamePanelHUDCorePlugin.HUDClass<float, SettingsData> HUD = new GamePanelHUDCorePlugin.HUDClass<float, SettingsData>();

        internal static float NorthDirection;

        private bool CompassHUDSW;

        private Transform Cam;

        private float AngleNum;

        private readonly SettingsData SettingsDatas = new SettingsData();

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDCompass");

            ModUpdateCheck.DrawNeedUpdate(Config, Info.Metadata.Version);

            string mainSettings = "主设置 Main Settings";
            string positionScaleSettings = "位置大小设置 Position Scale Settings";
            string colorSettings = "颜色设置 Color Settings";
            string fontStylesSettings = "字体样式设置 Font Styles Settings";

            SettingsDatas.KeyCompassHUDSW = Config.Bind<bool>(mainSettings, "罗盘指示栏显示 Compass HUD display", true);
            SettingsDatas.KeyAngleHUDSW = Config.Bind<bool>(mainSettings, "罗盘角度显示 Compass Angle HUD display", true);

            SettingsDatas.KeyAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "指示栏位置 Anchored Position", new Vector2(0, 0));
            SettingsDatas.KeySizeDelta = Config.Bind<Vector2>(positionScaleSettings, "指示栏高度 Size Delta", new Vector2(600, 90));
            SettingsDatas.KeyLocalScale = Config.Bind<Vector2>(positionScaleSettings, "指示栏大小 Local Scale", new Vector2(1, 1));

            SettingsDatas.KeyAngleOffset = Config.Bind<float>(mainSettings, "角度偏移 Angle Offset", 0);

            SettingsDatas.KeyArrowColor = Config.Bind<Color>(colorSettings, "指针 Arrow", new Color(1f, 1f, 1f));
            SettingsDatas.KeyAzimuthsColor = Config.Bind<Color>(colorSettings, "刻度 Azimuths", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyAzimuthsAngleColor = Config.Bind<Color>(colorSettings, "刻度角度 Azimuths Angle", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyDirectionColor = Config.Bind<Color>(colorSettings, "方向 Direction", new Color(0.8901961f, 0.8901961f, 0.8392157f));
            SettingsDatas.KeyAngleColor = Config.Bind<Color>(colorSettings, "角度 Angle", new Color(0.8901961f, 0.8901961f, 0.8392157f));

            SettingsDatas.KeyAzimuthsAngleStyles = Config.Bind<FontStyles>(fontStylesSettings, "刻度角度 Azimuths Angle", FontStyles.Normal);
            SettingsDatas.KeyDirectionStyles = Config.Bind<FontStyles>(fontStylesSettings, "方向 Direction", FontStyles.Bold);
            SettingsDatas.KeyAngleStyles = Config.Bind<FontStyles>(fontStylesSettings, "角度 Angle", FontStyles.Bold);

            new LevelSettingsPatch().Enable();

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            HUDCore.LoadHUD("gamepanlcompasshud.bundle", "gamepanlcompasshud");
        }

        public void IUpdate()
        {
            CompassPlugin();
        }

        void CompassPlugin()
        {
            CompassHUDSW = HUDCore.AllHUDSW && Cam != null && HUDCore.HasPlayer && SettingsDatas.KeyCompassHUDSW.Value;

            HUD.Set(AngleNum, SettingsDatas, CompassHUDSW);

            if (HUDCore.HasPlayer)
            {
                Cam = HUDCore.IsYourPlayer.CameraPosition;

                AngleNum = GetAngle(Cam, NorthDirection, SettingsDatas.KeyAngleOffset.Value);
            }
        }

        float GetAngle(Transform transform, float northdirection, float offset)
        {
            float num = transform.eulerAngles.y - northdirection + offset;

            if (num >= 0)
            {
                return num;
            }
            else
            {
                return num + 360;
            }
        }

        public class SettingsData
        {
            public ConfigEntry<bool> KeyCompassHUDSW;
            public ConfigEntry<bool> KeyAngleHUDSW;

            public ConfigEntry<Vector2> KeyAnchoredPosition;
            public ConfigEntry<Vector2> KeySizeDelta;
            public ConfigEntry<Vector2> KeyLocalScale;

            public ConfigEntry<float> KeyAngleOffset;

            public ConfigEntry<Color> KeyArrowColor;
            public ConfigEntry<Color> KeyAzimuthsColor;
            public ConfigEntry<Color> KeyAzimuthsAngleColor;
            public ConfigEntry<Color> KeyDirectionColor;
            public ConfigEntry<Color> KeyAngleColor;

            public ConfigEntry<FontStyles> KeyAzimuthsAngleStyles;
            public ConfigEntry<FontStyles> KeyDirectionStyles;
            public ConfigEntry<FontStyles> KeyAngleStyles;
        }
    }
}
#endif
