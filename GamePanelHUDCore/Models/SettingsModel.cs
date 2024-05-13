#if !UNITY_EDITOR

using System.Diagnostics.CodeAnalysis;
using BepInEx.Configuration;

namespace GamePanelHUDCore.Models
{
    internal class SettingsModel
    {
        public static SettingsModel Instance { get; private set; }

        public readonly ConfigEntry<bool> KeyAllHUDAlways;

        [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
        private SettingsModel(ConfigFile configFile)
        {
            const string mainSettings = "Main Settings";

            KeyAllHUDAlways = configFile.Bind<bool>(mainSettings, "All HUD Always display", false);
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