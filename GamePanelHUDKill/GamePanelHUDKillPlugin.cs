﻿#if !UNITY_EDITOR
using BepInEx;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using GamePanelHUDKill.Models;
using UnityEngine;
using static EFTApi.EFTHelpers;
using SettingsModel = GamePanelHUDKill.Models.SettingsModel;

namespace GamePanelHUDKill
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDKill", "GamePanelHUDKill", "2.7.8")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "2.7.8")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "localized/kill")]
    public partial class GamePanelHUDKillPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);

            var hudCoreModel = HUDCoreModel.Instance;
            var killHUDModel = KillHUDModel.Instance;

            var prefabs = hudCoreModel.LoadHUD("gamepanelkillhud.bundle", "GamePanelKillHUD");

            killHUDModel.ScreenRect = hudCoreModel.GamePanelHUDPublic.GetComponent<RectTransform>();
            killHUDModel.KillPrefab = prefabs.Asset["Kill"];
        }

        private void Start()
        {
            _PlayerHelper.OnBeenKilledByAggressor.Add(this, nameof(OnBeenKilledByAggressor));
        }
    }
}

#endif