#if !UNITY_EDITOR

using BepInEx;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using KmyTarkovUtils;
using static KmyTarkovApi.EFTHelpers;
using SettingsModel = GamePanelHUDHealth.Models.SettingsModel;

namespace GamePanelHUDHealth
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDHealth", "GamePanelHUDHealth", "3.3.0")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "3.3.0")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", @"localized\health")]
    public partial class GamePanelHUDHealthPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);
        }

        private void Start()
        {
            foreach (var value in HUDCoreModel.Instance.LoadHUD("gamepanelhealthhud.bundle", "GamePanelHealthHUD")
                         .Init.Values)
            {
                value.ReplaceAllFont(EFTFontHelper.BenderNormal);
            }

            _MainMenuControllerClassHelper.Execute.Add(this, nameof(Execute));
        }
    }
}

#endif