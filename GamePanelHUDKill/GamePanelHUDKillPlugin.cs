﻿#if !UNITY_EDITOR
using BepInEx;
using KmyTarkovUtils;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using GamePanelHUDKill.Models;
using UnityEngine;
using static KmyTarkovApi.EFTHelpers;
using SettingsModel = GamePanelHUDKill.Models.SettingsModel;

namespace GamePanelHUDKill
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDKill", "GamePanelHUDKill", "3.3.0")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "3.3.0")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", @"localized\kill")]
    public partial class GamePanelHUDKillPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);

            var hudCoreModel = HUDCoreModel.Instance;
            var killHUDModel = KillHUDModel.Instance;

            var prefabs = hudCoreModel.LoadHUD("gamepanelkillhud.bundle", "GamePanelKillHUD");

            foreach (var value in prefabs.Init.Values)
            {
                value.ReplaceAllFont(EFTFontHelper.BenderNormal);
            }

            killHUDModel.ScreenRect = hudCoreModel.GamePanelHUDPublic.GetComponent<RectTransform>();
            killHUDModel.KillPrefab = prefabs.Asset["Kill"].ReplaceAllFont(EFTFontHelper.BenderNormal);
        }

        private void Start()
        {
            _PlayerHelper.OnBeenKilledByAggressor.Add(this, nameof(OnBeenKilledByAggressor));
        }
    }
}

#endif