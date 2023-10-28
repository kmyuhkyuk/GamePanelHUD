#if !UNITY_EDITOR

using BepInEx;
using EFTUtils;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using UnityEngine;
using UnityEngine.UI;

namespace GamePanelHUDCore
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCore", "kmyuhkyuk-GamePanelHUDCore", "2.7.8")]
    [BepInDependency("com.kmyuhkyuk.EFTApi", "1.1.8")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "../localized/core")]
    public class GamePanelHUDCorePlugin : BaseUnityPlugin
    {
        private static readonly GameObject GamePanelHUDPublic =
            new GameObject("GamePanelHUDPublic", typeof(Canvas), typeof(CanvasScaler));

        private static readonly UpdateManger UpdateManger = new UpdateManger();

        private void Awake()
        {
            SettingsModel.Create(Config);

            HUDCoreModel.Create(GamePanelHUDPublic, UpdateManger);
        }
    }
}

#endif