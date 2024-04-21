using System;
using UnityEngine;
#if !UNITY_EDITOR
using GamePanelHUDCore.Models;
using GamePanelHUDGrenade.Models;
using SettingsModel = GamePanelHUDGrenade.Models.SettingsModel;
using static EFTApi.EFTHelpers;
using EFT.InventoryLogic;
using EFTReflection;
using EFTUtils;
#endif

namespace GamePanelHUDGrenade.Controllers
{
    public class GrenadeHUDController : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR

        private Item _rig;

        private Item _pocket;

        private Type _grenadeItemType;

        private void Awake()
        {
            _grenadeItemType = RefTool.GetEftType(x =>
                x.GetMethod("CreateFragment", RefTool.Public) != null &&
                x.GetProperty("GetExplDelay", RefTool.Public) != null);
        }

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var hudCoreModel = HUDCoreModel.Instance;
            var grenadeHUDModel = GrenadeHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            var hasPlayer = hudCoreModel.HasPlayer;

            grenadeHUDModel.GrenadeHUDSw = hudCoreModel.AllHUDSw && hasPlayer &&
                                           settingsModel.KeyGrenadeHUDSw.Value;

            if (!hasPlayer)
                return;

            var rigAmount = grenadeHUDModel.RigAmount;
            var pocketAmount = grenadeHUDModel.PocketAmount;
            var allAmount = grenadeHUDModel.AllAmount;

            //Performance Optimization
            if (Time.frameCount % 20 == 0)
            {
                var slots = _InventoryHelper.EquipmentSlots;

                //Get Rig and Pocket
                _rig = slots[6].ContainedItem;
                _pocket = slots[10].ContainedItem;

                GetGrenadeAmount(_rig, out rigAmount.Frag, out rigAmount.Stun, out rigAmount.Flash,
                    out rigAmount.Smoke);
                GetGrenadeAmount(_pocket, out pocketAmount.Frag, out pocketAmount.Stun, out pocketAmount.Flash,
                    out pocketAmount.Smoke);
            }

            allAmount.Frag = rigAmount.Frag + pocketAmount.Frag;
            allAmount.Stun = rigAmount.Stun + pocketAmount.Stun;
            allAmount.Flash = rigAmount.Flash + pocketAmount.Flash;
            allAmount.Smoke = rigAmount.Smoke + pocketAmount.Smoke;
        }

        private void GetGrenadeAmount(Item gear, out int frag, out int stun, out int flash, out int smoke)
        {
            frag = 0;
            stun = 0;
            flash = 0;
            smoke = 0;

            if (gear == null)
                return;

            var grids = _InventoryHelper.RefGrids.GetValue(gear);

            if (!SettingsModel.Instance.KeyMergeGrenade.Value)
            {
                foreach (var grid in grids)
                {
                    foreach (var item in _InventoryHelper.RefItems.GetValue(grid))
                    {
                        if (item.GetType() == _grenadeItemType)
                        {
                            var throwType = ReflectionModel.Instance.RefThrowType.GetValue(item);

                            switch (throwType)
                            {
                                case ThrowWeapType.frag_grenade:
                                    frag++;
                                    break;
                                case ThrowWeapType.stun_grenade:
                                    stun++;
                                    break;
                                case ThrowWeapType.flash_grenade:
                                    flash++;
                                    break;
                                case ThrowWeapType.smoke_grenade:
                                    smoke++;
                                    break;
                                case ThrowWeapType.gas_grenade:
                                case ThrowWeapType.incendiary_grenade:
                                case ThrowWeapType.sonar:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(throwType), throwType, null);
                            }
                        }
                    }
                }
            }
            else
            {
                // ReSharper disable once LoopCanBeConvertedToQuery
                foreach (var grid in grids)
                {
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var item in _InventoryHelper.RefItems.GetValue(grid))
                    {
                        if (item.GetType() == _grenadeItemType)
                        {
                            frag++;
                        }
                    }
                }
            }
        }

#endif
    }
}