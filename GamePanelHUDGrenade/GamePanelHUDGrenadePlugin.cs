#if !UNITY_EDITOR

using BepInEx;
using EFTUtils;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using SettingsModel = GamePanelHUDGrenade.Models.SettingsModel;

namespace GamePanelHUDGrenade
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDGrenade", "GamePanelHUDGrenade", "3.1.1")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "3.1.1")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "localized/grenade")]
    public class GamePanelHUDGrenadePlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);

            foreach (var value in HUDCoreModel.Instance.LoadHUD("gamepanelgrenadehud.bundle", "GamePanelGrenadeHUD")
                         .Init.Values)
            {
                value.ReplaceAllFont(EFTFontHelper.BenderNormal);
            }
        }
    }
}

#endif