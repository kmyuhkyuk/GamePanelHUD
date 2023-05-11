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
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDLife", "kmyuhkyuk-GamePanelHUDLife", "2.6.4")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class GamePanelHUDLifePlugin : BaseUnityPlugin, IUpdate
    {
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

        internal static IHealthController HealthController;

        internal static readonly GamePanelHUDCorePlugin.HUDClass<Life, SettingsData> HUD = new GamePanelHUDCorePlugin.HUDClass<Life, SettingsData>();

        private bool LifeHUDSw;

        private readonly Life LData = new Life();

        private readonly SettingsData SetData = new SettingsData();

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDLife");

            ModUpdateCheck.DrawCheck(this);

            const string mainSettings = "主设置 Main Settings";
            const string positionScaleSettings = "位置高度大小设置 Position Size Scale Settings";
            const string colorSettings = "颜色设置 Color Settings";
            const string fontStylesSettings = "字体样式设置 Font Styles Settings";
            const string warningRateSettings = "警告率设置 Warning Rate Settings";
            const string speedSettings = "动画速度设置 Animation Speed Settings";

            SetData.KeyLifeHUDSw = Config.Bind<bool>(mainSettings, "生命指示栏显示 Life HUD display", true);
            SetData.KeyBuffSw = Config.Bind<bool>(mainSettings, "Buff显示 Buff Rate display", true);
            SetData.KeyArrowAnimation = Config.Bind<bool>(mainSettings, "Buff动画 Buff Arrow Animation", true);
            SetData.KeyArrowAnimationReverse = Config.Bind<bool>(mainSettings, "Buff动画反转 Buff Arrow Animation Reverse", false);

            SetData.KeyAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "指示栏位置 Anchored Position", new Vector2(250, 50));
            SetData.KeySizeDelta = Config.Bind<Vector2>(positionScaleSettings, "指示栏高度 Size Delta", new Vector2(250, 180));
            SetData.KeyLocalScale = Config.Bind<Vector2>(positionScaleSettings, "指示栏大小 Local Scale", new Vector2(1, 1));

            SetData.KeyHealthWarningRate = Config.Bind<int>(warningRateSettings, "生命值 Health", 25, new ConfigDescription("When Health < 25%, Current Color change to Red Warning", new AcceptableValueRange<int>(0, 100)));
            SetData.KeyHydrationWarningRate = Config.Bind<int>(warningRateSettings, "口渴度 Hydration", 10, new ConfigDescription("When Hydration < 10%, Current Color change to Red Warning", new AcceptableValueRange<int>(0, 100)));
            SetData.KeyEnergyWarningRate = Config.Bind<int>(warningRateSettings, "能量值 Energy", 10, new ConfigDescription("When Energy < 10%, Current Color change to Red Warning", new AcceptableValueRange<int>(0, 100)));

            SetData.KeyBuffSpeed = Config.Bind<float>(speedSettings, "Buff动画速度 Buff Animation Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));

            SetData.KeyHealthColor = Config.Bind<Color>(colorSettings, "生命值 Health", new Color(0.6039216f, 0.827451f, 0.1372549f)); //#9AD323
            SetData.KeyHydrationColor = Config.Bind<Color>(colorSettings, "口渴度 Hydration", new Color(0.3607843f, 0.682353f, 0.8431373f)); //#5CAED7
            SetData.KeyEnergyColor = Config.Bind<Color>(colorSettings, "能量值 Energy", new Color(0.854902f, 0.854902f, 0.7372549f)); //#DADABC
            SetData.KeyMaxColor = Config.Bind<Color>(colorSettings, "最大值 Maximum", new Color(0.5882353f, 0.6039216f, 0.6078432f)); //#969A9B
            SetData.KeyAddZerosColor = Config.Bind<Color>(colorSettings, "零 Zeros", new Color(0.6f, 0.6f, 0.6f, 0.5f)); //#9999
            SetData.KeyWarningColor = Config.Bind<Color>(colorSettings, "警告 Warning", new Color(0.7294118f, 0f, 0f )); //#BA0000
            SetData.KeyUpBuffArrowColor = Config.Bind<Color>(colorSettings, "Buff指针上升 Buff Arrow Up", new Color(0.3490196f, 0.6666667f, 0.8352941f)); //#59AAD5
            SetData.KeyDownBuffArrowColor = Config.Bind<Color>(colorSettings, "Buff指针下降 Buff Arrow Down", new Color(0.7294118f, 0f, 0f)); //#BA0000
            SetData.KeyUpBuffColor = Config.Bind<Color>(colorSettings, "Buff上升 Buff Up", new Color(0.3490196f, 0.6666667f, 0.8352941f)); //#59AAD5
            SetData.KeyDownBuffColor = Config.Bind<Color>(colorSettings, "Buff下降 Buff Down", new Color(0.7294118f, 0f, 0f)); //#BA0000

            SetData.KeyCurrentStyles = Config.Bind<FontStyles>(fontStylesSettings, "当前值 Current", FontStyles.Bold);
            SetData.KeyMaximumStyles = Config.Bind<FontStyles>(fontStylesSettings, "最大值 Maximum", FontStyles.Normal);

            new MainMenuControllerPatch().Enable();

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            GamePanelHUDCorePlugin.HUDCoreClass.LoadHUD("gamepanellifehud.bundle", "GamePanelLifeHUD");
        }

        public void IUpdate()
        {
            LifeHUDPlugin();
        }

        void LifeHUDPlugin()
        {
            LifeHUDSw = HUDCore.AllHUDSw && HealthController != null && HUDCore.HasPlayer && SetData.KeyLifeHUDSw.Value;

            HUD.Set(LData, SetData, LifeHUDSw);

            if (HUDCore.HasPlayer)
            {
                HealthController = HUDCore.YourPlayer.HealthController;
            }

            if (HealthController != null)
            {
                LData.Healths.Head = HealthController.GetBodyPartHealth(EBodyPart.Head);
                LData.Healths.Chest = HealthController.GetBodyPartHealth(EBodyPart.Chest);
                LData.Healths.Stomach = HealthController.GetBodyPartHealth(EBodyPart.Stomach);
                LData.Healths.LeftArm = HealthController.GetBodyPartHealth(EBodyPart.LeftArm);
                LData.Healths.RightArm = HealthController.GetBodyPartHealth(EBodyPart.RightArm);
                LData.Healths.LeftLeg = HealthController.GetBodyPartHealth(EBodyPart.LeftLeg);
                LData.Healths.RightLeg = HealthController.GetBodyPartHealth(EBodyPart.RightLeg);
                LData.Healths.Common = HealthController.GetBodyPartHealth(EBodyPart.Common);

                LData.Hydrations = HealthController.Hydration;

                LData.Energys = HealthController.Energy;

                LData.Rates = new Life.Rate(HealthController.HealthRate, HealthController.HydrationRate, HealthController.EnergyRate);
            }
        }

        public class Life
        {
            public Health Healths = new Health();

            public ValueStruct Hydrations;

            public ValueStruct Energys;

            public Rate Rates;

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

                public Rate(float health, float hydration, float energy)
                {
                    HealthRate = health;
                    HydrationRate = hydration;
                    EnergyRate = energy;
                }
            }
        }

        public class SettingsData
        {
            public ConfigEntry<bool> KeyLifeHUDSw;
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
