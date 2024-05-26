#if !UNITY_EDITOR

using BepInEx;
using EFTUtils;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using SettingsModel = GamePanelHUDWeapon.Models.SettingsModel;

namespace GamePanelHUDWeapon
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDWeapon", "GamePanelHUDWeapon", "3.1.0")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "3.1.0")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "localized/weapon")]
    public class GamePanelHUDWeaponPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);

            foreach (var keyValue in HUDCoreModel.Instance.LoadHUD("gamepanelweaponhud.bundle", "GamePanelWeaponHUD")
                         .Init)
            {
                keyValue.Value.ReplaceAllFont(EFTFontHelper.BenderNormal);
            }
        }
    }
}

#endif