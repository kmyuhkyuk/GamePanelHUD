#if !UNITY_EDITOR

using BepInEx;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;

namespace GamePanelHUDCore
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCore", "GamePanelHUDCore", "2.7.8")]
    [BepInDependency("com.kmyuhkyuk.EFTApi", "1.1.8")]
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