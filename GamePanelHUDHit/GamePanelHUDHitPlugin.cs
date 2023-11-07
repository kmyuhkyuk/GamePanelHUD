#if !UNITY_EDITOR

using BepInEx;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using GamePanelHUDHit.Models;
using HarmonyLib;
using UnityEngine;
using static EFTApi.EFTHelpers;
using SettingsModel = GamePanelHUDHit.Models.SettingsModel;

namespace GamePanelHUDHit
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDHit", "GamePanelHUDHit", "2.7.8")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "2.7.8")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "localized/hit")]
    public partial class GamePanelHUDHitPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);

            var hudCoreModel = HUDCoreModel.Instance;

            hudCoreModel.LoadHUD("gamepanelhithud.bundle", "GamePanelHitHUD");

            HitHUDModel.Instance.ScreenRect = hudCoreModel.GamePanelHUDPublic.GetComponent<RectTransform>();
        }

        private void Start()
        {
            _PlayerHelper.ApplyDamageInfo.Add(this, nameof(ApplyDamageInfo));
            _PlayerHelper.ArmorComponentHelper.ApplyDamage.Add(this, nameof(ApplyDamage),
                HarmonyPatchType.ILManipulator);
        }
    }
}

#endif