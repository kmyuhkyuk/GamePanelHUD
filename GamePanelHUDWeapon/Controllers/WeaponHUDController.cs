﻿using UnityEngine;
#if !UNITY_EDITOR
using EFT;
using EFT.InventoryLogic;
using KmyTarkovApi;
using KmyTarkovUtils;
using static KmyTarkovApi.EFTHelpers;
using GamePanelHUDCore.Models;
using GamePanelHUDCore.Utils;
using GamePanelHUDWeapon.Models;
using SettingsModel = GamePanelHUDWeapon.Models.SettingsModel;

#endif

namespace GamePanelHUDWeapon.Controllers
{
    public class WeaponHUDController : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR

        private Weapon _currentWeapon;

        private Weapon _oldWeapon;

#endif

        private LauncherItemClass _currentLauncher;

        private MagazineItemClass _currentMag;

        private MagazineItemClass _oldMag;

        private Animator _animatorWeapon;

        private Animator _animatorLauncher;

#if !UNITY_EDITOR

        private Player.FirearmController _currentFirearmController;

#endif

        private bool _allReloadBool;

        private bool _weaponCacheBool = true;

        private bool _magCacheBool = true;

        private bool _launcherCacheBool;

#if !UNITY_EDITOR

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var hudCoreModel = HUDCoreModel.Instance;
            var weaponHUDModel = WeaponHUDModel.Instance;
            var reflectionModel = ReflectionModel.Instance;
            var settingsModel = SettingsModel.Instance;

            var hasPlayer = hudCoreModel.HasPlayer;

            weaponHUDModel.WeaponHUDSw = hudCoreModel.AllHUDSw && _currentWeapon != null && hasPlayer &&
                                         settingsModel.KeyWeaponHUDSw.Value;

            //Get Player
            if (!hasPlayer)
                return;

            reflectionModel.RefAmmoCount
                .GetValue(_EftBattleUIScreenHelper.AmmoCountPanel)
                .gameObject
                .SetActive(!settingsModel.KeyHideGameAmmoPanel.Value);

            _currentFirearmController = EFTGlobal.FirearmController;

            //Get Weapon Class
            _currentWeapon = EFTGlobal.Weapon;
            _animatorWeapon = _WeaponHelper.WeaponAnimator;

            _currentLauncher = _WeaponHelper.UnderbarrelWeapon;
            _animatorLauncher = _WeaponHelper.LauncherAnimator;

            var weaponActive = _currentWeapon != null;

            var launcherActive = weaponActive && _currentFirearmController != null &&
                                 _currentFirearmController.IsInLauncherMode();

            if (_weaponCacheBool)
            {
                _oldWeapon = _currentWeapon;

                if (!settingsModel.KeyWeaponNameAlways.Value && settingsModel.KeyAutoWeaponName.Value)
                {
                    weaponHUDModel.WeaponTrigger();
                }

                _weaponCacheBool = false;
                _launcherCacheBool = false;
            }
            //Not same Weapon trigger
            else if (weaponActive && _currentWeapon != _oldWeapon || !weaponActive)
            {
                _weaponCacheBool = true;
                _magCacheBool = true;
            }

            //Switch Launcher trigger
            if (launcherActive != _launcherCacheBool)
            {
                if (!settingsModel.KeyWeaponNameAlways.Value && settingsModel.KeyAutoWeaponName.Value)
                {
                    weaponHUDModel.WeaponTrigger();
                }

                _launcherCacheBool = launcherActive;
            }
            else if (!launcherActive)
            {
                _launcherCacheBool = false;
            }

            //Get Ammo Count
            if (!weaponActive)
                return;

            var currentAnimator = launcherActive ? _animatorLauncher : _animatorWeapon;

            var currentState = currentAnimator.GetCurrentAnimatorStateInfo(1).fullPathHash;

            weaponHUDModel.Weapon.WeaponNameAlways = settingsModel.KeyWeaponNameAlways.Value ||
                                                     currentState == 1355507738 &&
                                                     settingsModel.KeyLookWeaponName.Value; //2.LookWeapon

            //MagCount and PatronCount
            if (!launcherActive)
            {
                _allReloadBool =
                    _currentFirearmController != null && _currentFirearmController.IsInReloadOperation() ||
                    currentState == 1058993437; //1.OriginalReloadCheck 2.TakeHands

                //Get Weapon Name
                weaponHUDModel.Weapon.WeaponName = _LocalizedHelper.Localized(_currentWeapon.Name);

                weaponHUDModel.Weapon.WeaponShortName = _LocalizedHelper.Localized(_currentWeapon.ShortName);

                //Get Fire Mode
                weaponHUDModel.Weapon.FireMode =
                    _LocalizedHelper.Localized(_currentWeapon.SelectedFireMode.ToString());

                weaponHUDModel.Weapon.AmmoType = _LocalizedHelper.Localized(_currentWeapon.AmmoCaliber);

                if (_currentWeapon.ReloadMode != Weapon.EReloadMode.OnlyBarrel)
                {
                    var magInWeapon = _animatorWeapon.GetBool(AnimatorHash.MagInWeapon);
                    var magSame = _currentMag == _oldMag;

                    var ammoInChamber = (int)_animatorWeapon.GetFloat(AnimatorHash.AmmoInChamber);
                    var chambersCount = _currentWeapon.ChamberAmmoCount;
                    var maxMagazineCount = _currentWeapon.GetMaxMagazineCount();

                    _currentMag = _currentWeapon.GetCurrentMagazine();

                    if (_magCacheBool)
                    {
                        _oldMag = _currentMag;

                        _magCacheBool = false;
                    }

                    switch (magInWeapon)
                    {
                        case true when !_allReloadBool && magSame:
                        {
                            var count = _currentWeapon.GetCurrentMagazineCount();

                            weaponHUDModel.Weapon.Patron = chambersCount;

                            weaponHUDModel.Weapon.Normalized = (float)count / maxMagazineCount;

                            weaponHUDModel.Weapon.MagCount = count;

                            weaponHUDModel.Weapon.MagMaxCount = maxMagazineCount;
                            break;
                        }
                        case false when _allReloadBool:
                            weaponHUDModel.Weapon.Patron = ammoInChamber;

                            weaponHUDModel.Weapon.Normalized = 0;

                            weaponHUDModel.Weapon.MagCount = 0;

                            weaponHUDModel.Weapon.MagMaxCount = 0;

                            _oldMag = _currentMag;
                            break;
                        case true when _allReloadBool && magSame:
                        {
                            var count = (int)_animatorWeapon.GetFloat(AnimatorHash.AmmoInMag);

                            weaponHUDModel.Weapon.Patron = ammoInChamber;

                            weaponHUDModel.Weapon.Normalized = (float)count / maxMagazineCount;

                            weaponHUDModel.Weapon.MagCount = count;

                            weaponHUDModel.Weapon.MagMaxCount = maxMagazineCount;
                            break;
                        }
                        case false when ammoInChamber != 0 && !_allReloadBool:
                            weaponHUDModel.Weapon.Patron = chambersCount;

                            weaponHUDModel.Weapon.Normalized = 0;

                            weaponHUDModel.Weapon.MagCount = 0;

                            weaponHUDModel.Weapon.MagMaxCount = 0;
                            break;
                        case false when !_allReloadBool:
                            weaponHUDModel.Weapon.Patron = 0;

                            weaponHUDModel.Weapon.Normalized = 0;

                            weaponHUDModel.Weapon.MagCount = 0;

                            weaponHUDModel.Weapon.MagMaxCount = 0;
                            break;
                    }
                }
                else
                {
                    var ammoInChamber = (int)_animatorWeapon.GetFloat(AnimatorHash.AmmoInChamber);
                    var chambersCount = _currentWeapon.Chambers.Length;

                    switch (_allReloadBool)
                    {
                        case false when ammoInChamber != 0:
                        {
                            var count = _currentWeapon.ChamberAmmoCount;

                            weaponHUDModel.Weapon.Patron = 0;

                            weaponHUDModel.Weapon.MagCount = count;

                            weaponHUDModel.Weapon.MagMaxCount = chambersCount;

                            weaponHUDModel.Weapon.Normalized = (float)count / chambersCount - 0.1f;
                            break;
                        }
                        case true:
                        {
                            weaponHUDModel.Weapon.Patron = 0;

                            weaponHUDModel.Weapon.MagCount = ammoInChamber;

                            weaponHUDModel.Weapon.MagMaxCount = chambersCount;

                            weaponHUDModel.Weapon.Normalized = (float)ammoInChamber / chambersCount - 0.1f;
                            break;
                        }
                        case false:
                            weaponHUDModel.Weapon.Patron = 0;

                            weaponHUDModel.Weapon.MagCount = 0;

                            weaponHUDModel.Weapon.MagMaxCount = chambersCount;

                            weaponHUDModel.Weapon.Normalized = 0;
                            break;
                    }
                }
            }
            else
            {
                _allReloadBool = currentState == 1285477936; //1.LauncherReload

                //Get Weapon Name
                weaponHUDModel.Weapon.WeaponName = _LocalizedHelper.Localized(_currentLauncher.Name);

                weaponHUDModel.Weapon.WeaponShortName =
                    _LocalizedHelper.Localized(_currentLauncher.ShortName);

                var launcherTemplate = _currentLauncher.WeaponTemplate;

                //Get Fire Mode
                weaponHUDModel.Weapon.FireMode = _LocalizedHelper.Localized(nameof(Weapon.EFireMode.single));

                weaponHUDModel.Weapon.AmmoType = _LocalizedHelper.Localized(launcherTemplate.ammoCaliber);

                var ammoInChamber = (int)_animatorLauncher.GetFloat(AnimatorHash.AmmoInChamber);
                var chambersCount = _currentLauncher.Chambers.Length;

                switch (_allReloadBool)
                {
                    case false when ammoInChamber != 0:
                    {
                        var count = _currentLauncher.ChamberAmmoCount;

                        weaponHUDModel.Weapon.Patron = 0;

                        weaponHUDModel.Weapon.MagCount = count;

                        weaponHUDModel.Weapon.MagMaxCount = chambersCount;

                        weaponHUDModel.Weapon.Normalized = (float)count / chambersCount - 0.1f;
                        break;
                    }
                    case true:
                    {
                        weaponHUDModel.Weapon.Patron = 0;

                        weaponHUDModel.Weapon.MagCount = ammoInChamber;

                        weaponHUDModel.Weapon.MagMaxCount = chambersCount;

                        weaponHUDModel.Weapon.Normalized = (float)ammoInChamber / chambersCount - 0.1f;
                        break;
                    }
                    case false:
                        weaponHUDModel.Weapon.Patron = 0;

                        weaponHUDModel.Weapon.MagCount = 0;

                        weaponHUDModel.Weapon.MagMaxCount = chambersCount;

                        weaponHUDModel.Weapon.Normalized = 0;
                        break;
                }
            }
        }

#endif
    }
}