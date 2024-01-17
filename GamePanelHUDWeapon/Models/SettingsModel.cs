#if !UNITY_EDITOR

using System.Diagnostics.CodeAnalysis;
using BepInEx.Configuration;
using TMPro;
using UnityEngine;

namespace GamePanelHUDWeapon.Models
{
    public class SettingsModel
    {
        public static SettingsModel Instance { get; private set; }

        public readonly ConfigEntry<bool> KeyWeaponHUDSw;
        public readonly ConfigEntry<bool> KeyFireModeHUDSw;
        public readonly ConfigEntry<bool> KeyAmmoTypeHUDSw;
        public readonly ConfigEntry<bool> KeyWeaponNameAlways;
        public readonly ConfigEntry<bool> KeyWeaponShortName;
        public readonly ConfigEntry<bool> KeyZeroWarning;
        public readonly ConfigEntry<bool> KeyLookWeaponName;
        public readonly ConfigEntry<bool> KeyAutoWeaponName;
        public readonly ConfigEntry<bool> KeyHideGameAmmoPanel;

        public readonly ConfigEntry<Vector2> KeyAnchoredPosition;
        public readonly ConfigEntry<Vector2> KeyLocalScale;

        public readonly ConfigEntry<int> KeyWarningRate10;
        public readonly ConfigEntry<int> KeyWarningRate100;
        public readonly ConfigEntry<float> KeyWeaponNameSpeed;
        public readonly ConfigEntry<float> KeyZeroWarningSpeed;

        public readonly ConfigEntry<Color> KeyCurrentColor;
        public readonly ConfigEntry<Color> KeyMaxColor;
        public readonly ConfigEntry<Color> KeyPatronColor;
        public readonly ConfigEntry<Color> KeyWeaponNameColor;
        public readonly ConfigEntry<Color> KeyAmmoTypeColor;
        public readonly ConfigEntry<Color> KeyFireModeColor;
        public readonly ConfigEntry<Color> KeyAddZerosColor;
        public readonly ConfigEntry<Color> KeyWarningColor;

        public readonly ConfigEntry<FontStyles> KeyCurrentStyles;
        public readonly ConfigEntry<FontStyles> KeyMaximumStyles;
        public readonly ConfigEntry<FontStyles> KeyPatronStyles;
        public readonly ConfigEntry<FontStyles> KeyWeaponNameStyles;
        public readonly ConfigEntry<FontStyles> KeyAmmoTypeStyles;
        public readonly ConfigEntry<FontStyles> KeyFireModeStyles;

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
        private SettingsModel(ConfigFile configFile)
        {
            const string mainSettings = "Main Settings";
            const string positionScaleSettings = "Position Scale Settings";
            const string colorSettings = "Color Settings";
            const string fontStylesSettings = "Font Styles Settings";
            const string warningRateSettings = "Warning Rate Settings";
            const string speedSettings = "Animation Speed Settings";

            KeyWeaponHUDSw = configFile.Bind<bool>(mainSettings, "Weapon HUD display", true);
            KeyAmmoTypeHUDSw = configFile.Bind<bool>(mainSettings, "Ammo Type HUD display", true);
            KeyFireModeHUDSw = configFile.Bind<bool>(mainSettings, "Fire Mode display", true);
            KeyWeaponNameAlways = configFile.Bind<bool>(mainSettings, "Weapon Name Always display", false);
            KeyWeaponShortName = configFile.Bind<bool>(mainSettings, "Weapon ShortName", false);
            KeyZeroWarning = configFile.Bind<bool>(mainSettings, "Zero Warning Animation", true);
            KeyLookWeaponName = configFile.Bind<bool>(mainSettings, "Weapon Name Inspect display", true);
            KeyAutoWeaponName = configFile.Bind<bool>(mainSettings, "Weapon Name Auto display", true);
            KeyHideGameAmmoPanel = configFile.Bind<bool>(mainSettings, "Hide Game Ammo Panel", false);

            KeyAnchoredPosition =
                configFile.Bind<Vector2>(positionScaleSettings, "Anchored Position", new Vector2(-100, 40));
            KeyLocalScale =
                configFile.Bind<Vector2>(positionScaleSettings, "Local Scale", new Vector2(1, 1));

            KeyWarningRate10 = configFile.Bind<int>(warningRateSettings, "Max Ammo Within 10", 45,
                new ConfigDescription(
                    "When Max Ammo <= 10 and Current Ammo <= 45%, Current Color change to Warning",
                    new AcceptableValueRange<int>(0, 100)));
            KeyWarningRate100 = configFile.Bind<int>(warningRateSettings, "Max Ammo Within 100", 30,
                new ConfigDescription("When Max Ammo > 10 and Current Ammo < 30%, Current Color change to Warning",
                    new AcceptableValueRange<int>(0, 100)));

            KeyWeaponNameSpeed = configFile.Bind<float>(speedSettings,
                "Weapon Name Auto display Animation Speed", 1,
                new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));
            KeyZeroWarningSpeed = configFile.Bind<float>(speedSettings, "Zero Warning Animation Speed", 1,
                new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));

            KeyCurrentColor = configFile.Bind<Color>(colorSettings, "Current",
                new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            KeyMaxColor = configFile.Bind<Color>(colorSettings, "Maximum",
                new Color(0.5882353f, 0.6039216f, 0.6078432f)); //#969A9B
            KeyPatronColor =
                configFile.Bind<Color>(colorSettings, "Patron",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            KeyWeaponNameColor = configFile.Bind<Color>(colorSettings, "Weapon Name",
                new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            KeyAmmoTypeColor = configFile.Bind<Color>(colorSettings, "Ammo Type",
                new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            KeyFireModeColor = configFile.Bind<Color>(colorSettings, "Fire Mode",
                new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            KeyAddZerosColor =
                configFile.Bind<Color>(colorSettings, "Zeros", new Color(0.6f, 0.6f, 0.6f, 0.5f)); //#9999
            KeyWarningColor =
                configFile.Bind<Color>(colorSettings, "Warning", new Color(0.7294118f, 0f, 0f)); //#BA0000

            KeyCurrentStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Current", FontStyles.Bold);
            KeyMaximumStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Maximum", FontStyles.Normal);
            KeyPatronStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Patron", FontStyles.Normal);
            KeyWeaponNameStyles =
                configFile.Bind<FontStyles>(fontStylesSettings, "Weapon Name", FontStyles.Normal);
            KeyAmmoTypeStyles =
                configFile.Bind<FontStyles>(fontStylesSettings, "Ammo Type", FontStyles.Normal);
            KeyFireModeStyles =
                configFile.Bind<FontStyles>(fontStylesSettings, "Fire Mode", FontStyles.Normal);
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