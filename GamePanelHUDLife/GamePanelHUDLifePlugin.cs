#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using TMPro;
using EFT.HealthSystem;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
using GamePanelHUDLife.Patches;

namespace GamePanelHUDLife
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDLife", "kmyuhkyuk-GamePanelHUDLife", "2.3.0")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class GamePanelHUDLifePlugin : BaseUnityPlugin, IUpdate
    {
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore
        {
            get
            {
                return GamePanelHUDCorePlugin.HUDCore;
            }
        }

        internal static IHealthController HealthController;

        internal static readonly GamePanelHUDCorePlugin.HUDClass<Life, SettingsData> HUD = new GamePanelHUDCorePlugin.HUDClass<Life, SettingsData>();

        private bool LifeHUDSW;

        private readonly Life Lifes = new Life();

        private SettingsData SettingsDatas = new SettingsData();

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDLife");

            string mainSettings = "主设置 Main Settings";
            string positionScaleSettings = "位置高度大小设置 Position Size Scale Settings";
            string colorSettings = "颜色设置 Color Settings";
            string fontStylesSettings = "字体样式设置 Font Styles Settings";
            string warningRateSettings = "警告率设置 Warning Rate Settings";
            string speedSettings = "动画速度设置 Animation Speed Settings";

            SettingsDatas.KeyLifeHUDSW = Config.Bind<bool>(mainSettings, "生命指示栏显示 Life HUD display", true);
            SettingsDatas.KeyBuffSw = Config.Bind<bool>(mainSettings, "Buff显示 Buff Rate display", true);
            SettingsDatas.KeyArrowAnimation = Config.Bind<bool>(mainSettings, "Buff动画 Buff Arrow Animation", true);
            SettingsDatas.KeyArrowAnimationReverse = Config.Bind<bool>(mainSettings, "Buff动画反转 Buff Arrow Animation Reverse", false);

            SettingsDatas.KeyAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "指示栏位置 Anchored Position", new Vector2(250, 50));
            SettingsDatas.KeySizeDelta = Config.Bind<Vector2>(positionScaleSettings, "指示栏高度 Size Delta", new Vector2(250, 180));
            SettingsDatas.KeyLocalScale = Config.Bind<Vector2>(positionScaleSettings, "指示栏大小 Local Scale", new Vector2(1, 1));

            SettingsDatas.KeyHealthWarningRate = Config.Bind<int>(warningRateSettings, "生命值 Health", 25, new ConfigDescription("When Health < 25%, Current Color change to Red Warning", new AcceptableValueRange<int>(0, 100)));
            SettingsDatas.KeyHydrationWarningRate = Config.Bind<int>(warningRateSettings, "口渴度 Hydration", 10, new ConfigDescription("When Hydration < 10%, Current Color change to Red Warning", new AcceptableValueRange<int>(0, 100)));
            SettingsDatas.KeyEnergyWarningRate = Config.Bind<int>(warningRateSettings, "能量值 Energy", 10, new ConfigDescription("When Energy < 10%, Current Color change to Red Warning", new AcceptableValueRange<int>(0, 100)));

            SettingsDatas.KeyBuffSpeed = Config.Bind<float>(speedSettings, "Buff动画速度 Buff Animation Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));

            SettingsDatas.KeyHealthColor = Config.Bind<Color>(colorSettings, "生命值 Health", new Color(0.6039216f, 0.827451f, 0.1372549f)); //#9AD323
            SettingsDatas.KeyHydrationColor = Config.Bind<Color>(colorSettings, "口渴度 Hydration", new Color(0.3607843f, 0.682353f, 0.8431373f)); //#5CAED7
            SettingsDatas.KeyEnergyColor = Config.Bind<Color>(colorSettings, "能量值 Energy", new Color(0.854902f, 0.854902f, 0.7372549f)); //#DADABC
            SettingsDatas.KeyMaxColor = Config.Bind<Color>(colorSettings, "最大值 Maximum", new Color(0.5882353f, 0.6039216f, 0.6078432f)); //#969A9B
            SettingsDatas.KeyAddZerosColor = Config.Bind<Color>(colorSettings, "零 Zeros", new Color(0.6f, 0.6f, 0.6f, 0.5f)); //#9999
            SettingsDatas.KeyWarningColor = Config.Bind<Color>(colorSettings, "警告 Warning", new Color(0.7294118f, 0f, 0f )); //#BA0000
            SettingsDatas.KeyUpBuffArrowColor = Config.Bind<Color>(colorSettings, "Buff指针上升 Buff Arrow Up", new Color(0.3490196f, 0.6666667f, 0.8352941f)); //#59AAD5
            SettingsDatas.KeyDownBuffArrowColor = Config.Bind<Color>(colorSettings, "Buff指针下降 Buff Arrow Down", new Color(0.7294118f, 0f, 0f)); //#BA0000
            SettingsDatas.KeyUpBuffColor = Config.Bind<Color>(colorSettings, "Buff上升 Buff Up", new Color(0.3490196f, 0.6666667f, 0.8352941f)); //#59AAD5
            SettingsDatas.KeyDownBuffColor = Config.Bind<Color>(colorSettings, "Buff下降 Buff Down", new Color(0.7294118f, 0f, 0f)); //#BA0000

            SettingsDatas.KeyCurrentStyles = Config.Bind<FontStyles>(fontStylesSettings, "当前值 Current", FontStyles.Bold);
            SettingsDatas.KeyMaximumStyles = Config.Bind<FontStyles>(fontStylesSettings, "最大值 Maximum", FontStyles.Normal);

            new MainMenuControllerPatch().Enable();

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            HUDCore.LoadHUD("gamepanllifehud.bundle", "gamepanllifehud");
        }

        public void IUpdate()
        {
            LifeHUDPlugin();
        }

        void LifeHUDPlugin()
        {
            LifeHUDSW = HUDCore.AllHUDSW && HealthController != null && HUDCore.HasPlayer && SettingsDatas.KeyLifeHUDSW.Value;

            HUD.Set(Lifes, SettingsDatas, LifeHUDSW);

            if (HUDCore.HasPlayer)
            {
                HealthController = HUDCore.IsYourPlayer.HealthController;
            }

            if (HealthController != null)
            {
                Lifes.Healths.Head = HealthController.GetBodyPartHealth(EBodyPart.Head);
                Lifes.Healths.Chest = HealthController.GetBodyPartHealth(EBodyPart.Chest);
                Lifes.Healths.Stomach = HealthController.GetBodyPartHealth(EBodyPart.Stomach);
                Lifes.Healths.LeftArm = HealthController.GetBodyPartHealth(EBodyPart.LeftArm);
                Lifes.Healths.RightArm = HealthController.GetBodyPartHealth(EBodyPart.RightArm);
                Lifes.Healths.LeftLeg = HealthController.GetBodyPartHealth(EBodyPart.LeftLeg);
                Lifes.Healths.RightLeg = HealthController.GetBodyPartHealth(EBodyPart.RightLeg);
                Lifes.Healths.Common = HealthController.GetBodyPartHealth(EBodyPart.Common);

                Lifes.Hydrations = HealthController.Hydration;

                Lifes.Energys = HealthController.Energy;

                Lifes.Rates.HealthRate = HealthController.HealthRate;
                Lifes.Rates.HydrationRate = HealthController.HydrationRate;
                Lifes.Rates.EnergyRate = HealthController.EnergyRate;
            }
        }

        public class Life
        {
            public Health Healths = new Health();

            public ValueStruct Hydrations = new ValueStruct();

            public ValueStruct Energys = new ValueStruct();

            public Rate Rates = new Rate();

            public class Health
            {
                //Health Current float
                public ValueStruct Head;
                public ValueStruct Chest;
                public ValueStruct Stomach;
                public ValueStruct LeftArm;
                public ValueStruct RightArm;
                public ValueStruct LeftLeg;
                public ValueStruct RightLeg;
                public ValueStruct Common;
            }

            public struct Rate
            {
                //HealthRate HydrationRate EnergyRate
                public float HealthRate;
                public float HydrationRate;
                public float EnergyRate;
            }
        }

        public class SettingsData
        {
            public ConfigEntry<bool> KeyLifeHUDSW;
            public ConfigEntry<bool> KeyBuffSw;
            public ConfigEntry<bool> KeyArrowAnimation;
            public ConfigEntry<bool> KeyArrowAnimationReverse;

            public ConfigEntry<Vector2> KeyAnchoredPosition;
            public ConfigEntry<Vector2> KeySizeDelta;
            public ConfigEntry<Vector2> KeyLocalScale;

            public ConfigEntry<int> KeyHealthWarningRate;
            public ConfigEntry<int> KeyHydrationWarningRate;
            public ConfigEntry<int> KeyEnergyWarningRate;
            public ConfigEntry<float> KeyBuffSpeed;

            public ConfigEntry<Color> KeyHealthColor;
            public ConfigEntry<Color> KeyHydrationColor;
            public ConfigEntry<Color> KeyEnergyColor;
            public ConfigEntry<Color> KeyMaxColor;
            public ConfigEntry<Color> KeyAddZerosColor;
            public ConfigEntry<Color> KeyWarningColor;
            public ConfigEntry<Color> KeyUpBuffArrowColor;
            public ConfigEntry<Color> KeyDownBuffArrowColor;
            public ConfigEntry<Color> KeyUpBuffColor;
            public ConfigEntry<Color> KeyDownBuffColor;

            public ConfigEntry<FontStyles> KeyCurrentStyles;
            public ConfigEntry<FontStyles> KeyMaximumStyles;
        }
    }
}
#endif
