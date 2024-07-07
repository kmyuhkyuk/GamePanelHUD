#if !UNITY_EDITOR

using BepInEx;
using EFTUtils;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using static EFTApi.EFTHelpers;
using SettingsModel = GamePanelHUDHealth.Models.SettingsModel;

namespace GamePanelHUDHealth
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDHealth", "GamePanelHUDHealth", "3.1.1")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "3.1.1")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "localized/health")]
    public partial class GamePanelHUDHealthPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);

            foreach (var keyValue in HUDCoreModel.Instance.LoadHUD("gamepanelhealthhud.bundle", "GamePanelHealthHUD")
                         .Init)
            {
                keyValue.Value.ReplaceAllFont(EFTFontHelper.BenderNormal);
            }
        }

        private void Start()
        {
            _MainMenuControllerHelper.Execute.Add(this, nameof(Execute));
        }
    }
}

#endif