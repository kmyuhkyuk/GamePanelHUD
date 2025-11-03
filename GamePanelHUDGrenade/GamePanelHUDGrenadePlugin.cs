#if !UNITY_EDITOR

using BepInEx;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using KmyTarkovUtils;
using SettingsModel = GamePanelHUDGrenade.Models.SettingsModel;

namespace GamePanelHUDGrenade
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDGrenade", "GamePanelHUDGrenade", "3.4.0")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "3.4.0")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", @"localized\grenade")]
    public class GamePanelHUDGrenadePlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);
        }

        private void Start()
        {
            foreach (var value in HUDCoreModel.Instance.LoadHUD("gamepanelgrenadehud.bundle", "GamePanelGrenadeHUD")
                         .Init.Values)
            {
                value.ReplaceAllFont(EFTFontHelper.BenderNormal);
            }
        }
    }
}

#endif