﻿#if !UNITY_EDITOR

using System;
using BepInEx;
using EFT.Interactive;
using EFTApi;
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
            _FirearmControllerHelper.InitiateShot.Add(this, nameof(InitiateShot));
            _PlayerHelper.OnDead.Add(this, nameof(OnDead));

            if (EFTVersion.AkiVersion > EFTVersion.Parse("3.4.1"))
            {
                _AirdropBoxHelper.OnBoxLand?.Add(this, nameof(OnBoxLand));

                //Coop
                _AirdropBoxHelper.CoopOnBoxLand?.Add(this, nameof(CoopOnBoxLand));
            }
            else
            {
                _AirdropLogicClassHelper.RaycastGround.Add(this, nameof(RaycastGround));
            }

            _QuestHelper.OnConditionValueChanged.Add(this, nameof(OnConditionValueChanged));
        }

        private static void BaseOnBoxLand(Vector3 position, object boxSync, LootableContainer container)
        {
            string nameKey;
            string descriptionKey;
            switch (_AirdropSynchronizableObjectHelper.RefAirdropType?.GetValue(boxSync))
            {
                case 0:
                    nameKey = "6223349b3136504a544d1608 Name";
                    descriptionKey = "6223349b3136504a544d1608 Description";
                    break;
                case 1:
                    nameKey = "622334fa3136504a544d160c Name";
                    descriptionKey = "622334fa3136504a544d160c Description";
                    break;
                case 2:
                    nameKey = "622334c873090231d904a9fc Name";
                    descriptionKey = "622334c873090231d904a9fc Description";
                    break;
                case 3:
                    nameKey = "6223351bb5d97a7b2c635ca7 Name";
                    descriptionKey = "6223351bb5d97a7b2c635ca7 Description";
                    break;
                default:
                    nameKey = "Unknown";
                    descriptionKey = "Unknown";
                    break;
            }

            ShowAirdrop(position, nameKey, descriptionKey, container);
        }

        private static void ShowAirdrop(Vector3 position, string nameKey, string descriptionKey,
            LootableContainer container)
        {
            var compassStaticHUDModel = CompassStaticHUDModel.Instance;

            var controller = _LootableContainerHelper.RefItemOwner.GetValue(container);

            var item = _LootableContainerHelper.RefRootItem.GetValue(controller);

            var searchStates = _SearchableItemClassHelper.RefSearchStates.GetValue(item);

            var staticModel = new StaticModel
            {
                Id = $"Airdrop_{compassStaticHUDModel.AirdropCount++}",
                Where = position,
                NameKey = nameKey,
                DescriptionKey = descriptionKey,
                InfoType = StaticModel.Type.Airdrop,
                Requirements = new Func<bool>[]
                {
                    () => searchStates.ContainsKey(compassStaticHUDModel.CompassStatic.YourProfileId)
                }
            };

            compassStaticHUDModel.ShowStatic(staticModel);
        }
    }
}

#endif