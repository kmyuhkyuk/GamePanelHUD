#if !UNITY_EDITOR

using BepInEx;
using BepInEx.Configuration;
using EFT.HealthSystem;
using EFTUtils;
using GamePanelHUDCore;
using GamePanelHUDCore.Attributes;
using TMPro;
using UnityEngine;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDLife
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDLife", "kmyuhkyuk-GamePanelHUDLife", "2.7.7")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "2.7.7")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "localized/life")]
    public partial class GamePanelHUDLifePlugin : BaseUnityPlugin, IUpdate
    {
        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

        private static object _healthController;

        internal static readonly GamePanelHUDCorePlugin.HUDClass<Life, SettingsData> HUD =
            new GamePanelHUDCorePlugin.HUDClass<Life, SettingsData>();

        private bool _lifeHUDSw;

        private readonly Life _lifeData = new Life();

        private readonly SettingsData _setData;

        public GamePanelHUDLifePlugin()
        {
            _setData = new SettingsData(Config);
        }

        private void Start()
        {
            _MainMenuControllerHelper.Execute.Add(this, nameof(Execute));

            HUDCore.UpdateManger.Register(this);
        }

        private void Awake()
        {
            HUDCore.LoadHUD("gamepanellifehud.bundle", "GamePanelLifeHUD");
        }

        public void CustomUpdate()
        {
            LifeHUDPlugin();
        }

        private void LifeHUDPlugin()
        {
            _lifeHUDSw = HUDCore.AllHUDSw && _healthController != null && HUDCore.HasPlayer &&
                         _setData.KeyLifeHUDSw.Value;

            HUD.Set(_lifeData, _setData, _lifeHUDSw);

            if (HUDCore.HasPlayer)
            {
                _healthController = _PlayerHelper.HealthControllerHelper.HealthController;
            }

            if (_healthController != null)
            {
                _lifeData.Health.Head =
                    _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(_healthController, EBodyPart.Head);
                _lifeData.Health.Chest =
                    _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(_healthController, EBodyPart.Chest);
                _lifeData.Health.Stomach =
                    _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(_healthController, EBodyPart.Stomach);
                _lifeData.Health.LeftArm =
                    _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(_healthController, EBodyPart.LeftArm);
                _lifeData.Health.RightArm =
                    _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(_healthController, EBodyPart.RightArm);
                _lifeData.Health.LeftLeg =
                    _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(_healthController, EBodyPart.LeftLeg);
                _lifeData.Health.RightLeg =
                    _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(_healthController, EBodyPart.RightLeg);
                _lifeData.Health.Common =
                    _PlayerHelper.HealthControllerHelper.GetBodyPartHealth(_healthController, EBodyPart.Common);

                _lifeData.Hydration = _PlayerHelper.HealthControllerHelper.RefHydration.GetValue(_healthController);

                _lifeData.Energy = _PlayerHelper.HealthControllerHelper.RefEnergy.GetValue(_healthController);

                _lifeData.Rates = new Life.Rate(
                    _PlayerHelper.HealthControllerHelper.RefHealthRate.GetValue(_healthController),
                    _PlayerHelper.HealthControllerHelper.RefHydrationRate.GetValue(_healthController),
                    _PlayerHelper.HealthControllerHelper.RefEnergyRate.GetValue(_healthController));
            }
        }

        public class Life
        {
            public HealthClass Health = new HealthClass();

            public ValueStruct Hydration;

            public ValueStruct Energy;

            public Rate Rates;

            public class HealthClass
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
            public readonly ConfigEntry<bool> KeyLifeHUDSw;
            public readonly ConfigEntry<bool> KeyBuffSw;
            public readonly ConfigEntry<bool> KeyArrowAnimation;
            public readonly ConfigEntry<bool> KeyArrowAnimationReverse;

            public readonly ConfigEntry<Vector2> KeyAnchoredPosition;
            public readonly ConfigEntry<Vector2> KeySizeDelta;
            public readonly ConfigEntry<Vector2> KeyLocalScale;

            public readonly ConfigEntry<int> KeyHealthWarningRate;
            public readonly ConfigEntry<int> KeyHydrationWarningRate;
            public readonly ConfigEntry<int> KeyEnergyWarningRate;
            public readonly ConfigEntry<float> KeyBuffSpeed;

            public readonly ConfigEntry<Color> KeyHealthColor;
            public readonly ConfigEntry<Color> KeyHydrationColor;
            public readonly ConfigEntry<Color> KeyEnergyColor;
            public readonly ConfigEntry<Color> KeyMaxColor;
            public readonly ConfigEntry<Color> KeyAddZerosColor;
            public readonly ConfigEntry<Color> KeyWarningColor;
            public readonly ConfigEntry<Color> KeyUpBuffArrowColor;
            public readonly ConfigEntry<Color> KeyDownBuffArrowColor;
            public readonly ConfigEntry<Color> KeyUpBuffColor;
            public readonly ConfigEntry<Color> KeyDownBuffColor;

            public readonly ConfigEntry<FontStyles> KeyCurrentStyles;
            public readonly ConfigEntry<FontStyles> KeyMaximumStyles;

            public SettingsData(ConfigFile configFile)
            {
                const string mainSettings = "Main Settings";
                const string positionScaleSettings = "Position Size Scale Settings";
                const string colorSettings = "Color Settings";
                const string fontStylesSettings = "Font Styles Settings";
                const string warningRateSettings = "Warning Rate Settings";
                const string speedSettings = "Animation Speed Settings";

                KeyLifeHUDSw = configFile.Bind<bool>(mainSettings, "Life HUD display", true);
                KeyBuffSw = configFile.Bind<bool>(mainSettings, "Buff Rate display", true);
                KeyArrowAnimation = configFile.Bind<bool>(mainSettings, "Buff Arrow Animation", true);
                KeyArrowAnimationReverse =
                    configFile.Bind<bool>(mainSettings, "Buff Arrow Animation Reverse", false);

                KeyAnchoredPosition =
                    configFile.Bind<Vector2>(positionScaleSettings, "Anchored Position", new Vector2(250, 50));
                KeySizeDelta =
                    configFile.Bind<Vector2>(positionScaleSettings, "Size Delta", new Vector2(250, 180));
                KeyLocalScale =
                    configFile.Bind<Vector2>(positionScaleSettings, "Local Scale", new Vector2(1, 1));

                KeyHealthWarningRate = configFile.Bind<int>(warningRateSettings, "Health", 25,
                    new ConfigDescription("When Health < 25%, Current Color change to Warning",
                        new AcceptableValueRange<int>(0, 100)));
                KeyHydrationWarningRate = configFile.Bind<int>(warningRateSettings, "Hydration", 10,
                    new ConfigDescription("When Hydration < 10%, Current Color change to Warning",
                        new AcceptableValueRange<int>(0, 100)));
                KeyEnergyWarningRate = configFile.Bind<int>(warningRateSettings, "Energy", 10,
                    new ConfigDescription("When Energy < 10%, Current Color change to Warning",
                        new AcceptableValueRange<int>(0, 100)));

                KeyBuffSpeed = configFile.Bind<float>(speedSettings, "Buff Animation Speed", 1,
                    new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));

                KeyHealthColor =
                    configFile.Bind<Color>(colorSettings, "Health",
                        new Color(0.6039216f, 0.827451f, 0.1372549f)); //#9AD323
                KeyHydrationColor = configFile.Bind<Color>(colorSettings, "Hydration",
                    new Color(0.3607843f, 0.682353f, 0.8431373f)); //#5CAED7
                KeyEnergyColor =
                    configFile.Bind<Color>(colorSettings, "Energy",
                        new Color(0.854902f, 0.854902f, 0.7372549f)); //#DADABC
                KeyMaxColor = configFile.Bind<Color>(colorSettings, "Maximum",
                    new Color(0.5882353f, 0.6039216f, 0.6078432f)); //#969A9B
                KeyAddZerosColor =
                    configFile.Bind<Color>(colorSettings, "Zeros", new Color(0.6f, 0.6f, 0.6f, 0.5f)); //#9999
                KeyWarningColor =
                    configFile.Bind<Color>(colorSettings, "Warning", new Color(0.7294118f, 0f, 0f)); //#BA0000
                KeyUpBuffArrowColor = configFile.Bind<Color>(colorSettings, "Buff Arrow Up",
                    new Color(0.3490196f, 0.6666667f, 0.8352941f)); //#59AAD5
                KeyDownBuffArrowColor = configFile.Bind<Color>(colorSettings, "Buff Arrow Down",
                    new Color(0.7294118f, 0f, 0f)); //#BA0000
                KeyUpBuffColor = configFile.Bind<Color>(colorSettings, "Buff Up",
                    new Color(0.3490196f, 0.6666667f, 0.8352941f)); //#59AAD5
                KeyDownBuffColor =
                    configFile.Bind<Color>(colorSettings, "Buff Down", new Color(0.7294118f, 0f, 0f)); //#BA0000

                KeyCurrentStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Current", FontStyles.Bold);
                KeyMaximumStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Maximum", FontStyles.Normal);
            }
        }
    }
}

#endif