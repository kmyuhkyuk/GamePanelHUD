#if !UNITY_EDITOR

using BepInEx;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;

namespace GamePanelHUDCore
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCore", "GamePanelHUDCore", "3.1.1")]
    [BepInDependency("com.kmyuhkyuk.EFTApi", "1.2.2")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "../localized/core")]
    public class GamePanelHUDCorePlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);

            HUDCoreModel.Create();
        }
    }
}

#endif