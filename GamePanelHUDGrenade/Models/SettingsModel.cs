#if !UNITY_EDITOR

using System.Diagnostics.CodeAnalysis;
using BepInEx.Configuration;
using TMPro;
using UnityEngine;

namespace GamePanelHUDGrenade.Models
{
    internal class SettingsModel
    {
        public static SettingsModel Instance { get; private set; }

        public readonly ConfigEntry<bool> KeyGrenadeHUDSw;
        public readonly ConfigEntry<bool> KeyMergeGrenade;
        public readonly ConfigEntry<bool> KeyZeroWarning;

        public readonly ConfigEntry<Vector2> KeyAnchoredPosition;
        public readonly ConfigEntry<Vector2> KeySizeDelta;
        public readonly ConfigEntry<Vector2> KeyLocalScale;

        public readonly ConfigEntry<Color> KeyFragColor;
        public readonly ConfigEntry<Color> KeyStunColor;
        public readonly ConfigEntry<Color> KeySmokeColor;
        public readonly ConfigEntry<Color> KeyWarningColor;

        public readonly ConfigEntry<FontStyles> KeyFragStyles;
        public readonly ConfigEntry<FontStyles> KeyStunStyles;
        public readonly ConfigEntry<FontStyles> KeySmokeStyles;

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
        private SettingsModel(ConfigFile configFile)
        {
            const string mainSettings = "Main Settings";
            const string positionScaleSettings = "Position Scale Settings";
            const string colorSettings = "Color Settings";
            const string fontStylesSettings = "Font Styles Settings";

            KeyGrenadeHUDSw = configFile.Bind<bool>(mainSettings, "Grenade HUD display", true);
            KeyMergeGrenade = configFile.Bind<bool>(mainSettings, "Merge All Grenade Count", false);
            KeyZeroWarning = configFile.Bind<bool>(mainSettings, "Grenade Count Zero Warning", false);

            KeyAnchoredPosition =
                configFile.Bind<Vector2>(positionScaleSettings, "Anchored Position", new Vector2(-75, 5));
            KeySizeDelta =
                configFile.Bind<Vector2>(positionScaleSettings, "Size Delta", new Vector2(180, 30));
            KeyLocalScale =
                configFile.Bind<Vector2>(positionScaleSettings, "Local Scale", new Vector2(1, 1));

            KeyFragColor =
                configFile.Bind<Color>(colorSettings, "Frag",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            KeyStunColor =
                configFile.Bind<Color>(colorSettings, "Stun",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            KeySmokeColor =
                configFile.Bind<Color>(colorSettings, "Smoke",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            KeyWarningColor =
                configFile.Bind<Color>(colorSettings, "Warning", new Color(0.7294118f, 0f, 0f)); //#BA0000

            KeyFragStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Frag", FontStyles.Normal);
            KeyStunStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Stun", FontStyles.Normal);
            KeySmokeStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Smoke", FontStyles.Normal);
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