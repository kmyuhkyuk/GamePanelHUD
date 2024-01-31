#if !UNITY_EDITOR

using BepInEx;
using EFTUtils;
using GamePanelHUDCompass.Models;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using UnityEngine;
using static EFTApi.EFTHelpers;
using SettingsModel = GamePanelHUDCompass.Models.SettingsModel;

namespace GamePanelHUDCompass
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCompass", "GamePanelHUDCompass", "2.7.8")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "2.7.8")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "localized/compass")]
    public partial class GamePanelHUDCompassPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);

            var prefabs = HUDCoreModel.Instance.LoadHUD("gamepanelcompasshud.bundle", "GamePanelCompassHUD");

            foreach (var keyValue in prefabs.Init)
            {
                keyValue.Value.ReplaceAllFont(EFTFontHelper.BenderNormal);
            }

            CompassHUDModel.Instance.ScreenRect =
                HUDCoreModel.Instance.GamePanelHUDPublic.GetComponent<RectTransform>();

            CompassFireHUDModel.Instance.FirePrefab = prefabs.Asset["Fire"].ReplaceAllFont(EFTFontHelper.BenderNormal);

            CompassStaticHUDModel.Instance.StaticPrefab =
                prefabs.Asset["Point"].ReplaceAllFont(EFTFontHelper.BenderNormal);
        }

        private void Start()
        {
            HUDCoreModel.Instance.WorldStart += OnWorldStart;

            _FirearmControllerHelper.InitiateShot.Add(this, nameof(InitiateShot));
            _PlayerHelper.OnDead.Add(this, nameof(OnDead));
            _AirdropBoxHelper.OnBoxLand?.Add(this, nameof(OnBoxLand));
            _QuestHelper.OnConditionValueChanged.Add(this, nameof(OnConditionValueChanged));
        }
    }
}

#endif