#if !UNITY_EDITOR

using BepInEx;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using static EFTApi.EFTHelpers;
using SettingsModel = GamePanelHUDHealth.Models.SettingsModel;

namespace GamePanelHUDHealth
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDHealth", "kmyuhkyuk-GamePanelHUDHealth", "2.7.8")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "2.7.8")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "localized/health")]
    public partial class GamePanelHUDHealthPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);

            HUDCoreModel.Instance.LoadHUD("gamepanelhealthhud.bundle", "GamePanelHealthHUD");
        }

        private void Start()
        {
            _MainMenuControllerHelper.Execute.Add(this, nameof(Execute));
        }
    }
}

#endif