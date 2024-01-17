﻿#if !UNITY_EDITOR

using BepInEx;
using EFTUtils;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using SettingsModel = GamePanelHUDGrenade.Models.SettingsModel;

namespace GamePanelHUDGrenade
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDGrenade", "GamePanelHUDGrenade", "2.7.8")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "2.7.8")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "localized/grenade")]
    public class GamePanelHUDGrenadePlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);

            foreach (var keyValue in HUDCoreModel.Instance.LoadHUD("gamepanelgrenadehud.bundle", "GamePanelGrenadeHUD")
                         .Init)
            {
                keyValue.Value.ReplaceAllFont(EFTFontHelper.BenderNormal);
            }
        }
    }
}

#endif