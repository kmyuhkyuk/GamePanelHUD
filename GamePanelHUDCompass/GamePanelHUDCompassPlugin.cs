#if !UNITY_EDITOR

using System;
using BepInEx;
using EFT.Airdrop;
using EFT.Interactive;
using EFT.SynchronizableObjects;
using KmyTarkovApi;
using KmyTarkovUtils;
using GamePanelHUDCompass.Models;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Models;
using UnityEngine;
using static KmyTarkovApi.EFTHelpers;
using SettingsModel = GamePanelHUDCompass.Models.SettingsModel;

namespace GamePanelHUDCompass
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCompass", "GamePanelHUDCompass", "3.3.0")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "3.3.0")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", @"localized\compass")]
    public partial class GamePanelHUDCompassPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            SettingsModel.Create(Config);

            var prefabs = HUDCoreModel.Instance.LoadHUD("gamepanelcompasshud.bundle", "GamePanelCompassHUD");

            foreach (var value in prefabs.Init.Values)
            {
                value.ReplaceAllFont(EFTFontHelper.BenderNormal);
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
            _PlayerHelper.SetPropVisibility.Add(this, nameof(SetPropVisibility));

            _AirdropLogicClassHelper.RaycastGround.Add(this, nameof(RaycastGround));

            _AbstractQuestControllerClassHelper.OnConditionValueChanged.Add(this, nameof(OnConditionValueChanged));
        }

        private static void GetNameDescriptionKey(AirdropSynchronizableObject boxSync, out string nameKey,
            out string descriptionKey)
        {
            switch (boxSync.AirdropType)
            {
                case EAirdropType.Common:
                    nameKey = "6223349b3136504a544d1608 Name";
                    descriptionKey = "6223349b3136504a544d1608 Description";
                    break;
                case EAirdropType.Supply:
                    nameKey = "622334fa3136504a544d160c Name";
                    descriptionKey = "622334fa3136504a544d160c Description";
                    break;
                case EAirdropType.Medical:
                    nameKey = "622334c873090231d904a9fc Name";
                    descriptionKey = "622334c873090231d904a9fc Description";
                    break;
                case EAirdropType.Weapon:
                    nameKey = "6223351bb5d97a7b2c635ca7 Name";
                    descriptionKey = "6223351bb5d97a7b2c635ca7 Description";
                    break;
                default:
                    nameKey = "Unknown";
                    descriptionKey = "Unknown";
                    break;
            }
        }

        private static void ShowAirdrop(Vector3 position, string nameKey, string descriptionKey,
            LootableContainer container)
        {
            var compassStaticHUDModel = CompassStaticHUDModel.Instance;

            var controller = container.ItemOwner;

            var item = (SearchableItemItemClass)controller.RootItem;

            var staticModel = new StaticModel
            {
                Id = $"Airdrop_{compassStaticHUDModel.AirdropCount++}",
                Where = position,
                NameKey = nameKey,
                DescriptionKey = descriptionKey,
                InfoType = StaticModel.Type.Airdrop,
                Requirements = new Func<bool>[]
                {
                    () => EFTGlobal.Player.SearchController.IsSearched(item)
                }
            };

            compassStaticHUDModel.ShowStatic(staticModel);
        }
    }
}

#endif