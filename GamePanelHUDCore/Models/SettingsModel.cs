#if !UNITY_EDITOR

using BepInEx.Configuration;

namespace GamePanelHUDCore.Models
{
    public class SettingsModel
    {
        public static SettingsModel Instance { get; private set; }

        public readonly ConfigEntry<bool> KeyAllHUDAlways;

        private SettingsModel(ConfigFile configFile)
        {
            const string mainSettings = "Main Settings";

            KeyAllHUDAlways = configFile.Bind<bool>(mainSettings, "All HUD Always display", false);
        }

        public static SettingsModel Create(ConfigFile configFile)
        {
            if (Instance != null)
                return Instance;

            return Instance = new SettingsModel(configFile);
        }
    }
}

#endif