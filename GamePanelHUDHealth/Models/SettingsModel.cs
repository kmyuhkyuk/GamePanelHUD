#if !UNITY_EDITOR

using System.Diagnostics.CodeAnalysis;
using BepInEx.Configuration;
using TMPro;
using UnityEngine;

namespace GamePanelHUDHealth.Models
{
    public class SettingsModel
    {
        public static SettingsModel Instance { get; private set; }

        public readonly ConfigEntry<bool> KeyHealthHUDSw;
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

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
        private SettingsModel(ConfigFile configFile)
        {
            const string mainSettings = "Main Settings";
            const string positionScaleSettings = "Position Size Scale Settings";
            const string colorSettings = "Color Settings";
            const string fontStylesSettings = "Font Styles Settings";
            const string warningRateSettings = "Warning Rate Settings";
            const string speedSettings = "Animation Speed Settings";

            KeyHealthHUDSw = configFile.Bind<bool>(mainSettings, "Health HUD display", true);
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

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static SettingsModel Create(ConfigFile configFile)
        {
            if (Instance != null)
                return Instance;

            return Instance = new SettingsModel(configFile);
        }
    }
}

#endif