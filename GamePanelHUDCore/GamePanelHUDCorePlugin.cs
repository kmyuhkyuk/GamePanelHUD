#if !UNITY_EDITOR

using BepInEx;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;

namespace GamePanelHUDCore
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCore", "GamePanelHUDCore", "4.1.5")]
    [BepInDependency("com.kmyuhkyuk.KmyTarkovApi", "2.0.0")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", @"..\localized\core")]
    public class GamePanelHUDCorePlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);
        }

        private void Start()
        {
            HUDCoreModel.Create();
        }
    }
}

#endif