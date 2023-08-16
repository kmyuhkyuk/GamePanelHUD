#if !UNITY_EDITOR
using System;
using BepInEx;
using BepInEx.Configuration;
using EFT;
using EFT.InventoryLogic;
using EFT.UI;
using EFTApi;
using EFTReflection;
using GamePanelHUDCore;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Utils;
using TMPro;
using UnityEngine;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDMag
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDMag", "kmyuhkyuk-GamePanelHUDMag", "2.7.5")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "2.7.5")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "localized/mag")]
    public class GamePanelHUDMagPlugin : BaseUnityPlugin, IUpdate
    {
        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

        internal static readonly GamePanelHUDCorePlugin.HUDClass<WeaponData, SettingsData> HUD =
            new GamePanelHUDCorePlugin.HUDClass<WeaponData, SettingsData>();

        private readonly ReflectionData _reflectionData = new ReflectionData();

        private bool _magHUDSw;

        private Weapon _currentWeapon;

        private Weapon _oldWeapon;

        private object _currentLauncher;

        private object _currentMag;

        private object _oldMag;

        private Animator _animatorWeapon;

        private Animator _animatorLauncher;

        private readonly WeaponData _weaponData = new WeaponData();

        private readonly SettingsData _setData;

        private Player.FirearmController _currentFirearmController;

        private bool _allReloadBool;

        private bool _weaponCacheBool = true;

        private bool _magCacheBool = true;

        private bool _launcherCacheBool;

        internal static Action WeaponTrigger;

        public GamePanelHUDMagPlugin()
        {
            _setData = new SettingsData(Config);
        }

        private void Start()
        {
            HUDCore.UpdateManger.Register(this);
        }

        private void Awake()
        {
            HUDCore.LoadHUD("gamepanelmaghud.bundle", "GamePanelMagHUD");
        }

        public void CustomUpdate()
        {
            MagPlugin();
        }

        private void MagPlugin()
        {
            _magHUDSw = HUDCore.AllHUDSw && _currentWeapon != null && HUDCore.HasPlayer && _setData.KeyMagHUDSw.Value;

            HUD.Set(_weaponData, _setData, _magHUDSw);

            //Get Player
            if (HUDCore.HasPlayer)
            {
                _reflectionData.AmmoCount
                    .GetValue(_reflectionData.AmmoCountPanel.GetValue(HUDCore.YourGameUI.BattleUiScreen)).gameObject
                    .SetActive(!_setData.KeyHideGameAmmoPanel.Value);

                _currentFirearmController = EFTGlobal.FirearmController;

                //Get Weapon Class
                _currentWeapon = EFTGlobal.Weapon;
                _animatorWeapon = _PlayerHelper.WeaponHelper.WeaponAnimator;

                if (EFTVersion.AkiVersion > new Version("3.4.1"))
                {
                    _currentLauncher = EFTGlobal.UnderbarrelWeapon;
                    _animatorLauncher = _PlayerHelper.WeaponHelper.LauncherIAnimator;
                }

                var weaponActive = _currentWeapon != null;

                var launcherActive = weaponActive && _currentFirearmController != null &&
                                     _currentFirearmController.IsInLauncherMode();

                if (_weaponCacheBool)
                {
                    _oldWeapon = _currentWeapon;

                    if (!_setData.KeyWeaponNameAlways.Value && _setData.KeyAutoWeaponName.Value)
                    {
                        WeaponTrigger();
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
                    if (!_setData.KeyWeaponNameAlways.Value && _setData.KeyAutoWeaponName.Value)
                    {
                        WeaponTrigger();
                    }

                    _launcherCacheBool = launcherActive;
                }
                else if (!launcherActive)
                {
                    _launcherCacheBool = false;
                }

                //Get Ammo Count
                if (weaponActive)
                {
                    var currentAnimator = launcherActive ? _animatorLauncher : _animatorWeapon;

                    var currentState = currentAnimator.GetCurrentAnimatorStateInfo(1).fullPathHash;

                    _weaponData.WeaponNameAlways = _setData.KeyWeaponNameAlways.Value ||
                                                   currentState == 1355507738 &&
                                                   _setData.KeyLookWeaponName.Value; //2.LookWeapon

                    //MagCount and PatronCount
                    if (!launcherActive)
                    {
                        _allReloadBool =
                            _currentFirearmController != null && _currentFirearmController.IsInReloadOperation() ||
                            currentState == 1058993437; //1.OriginalReloadCheck 2.TakeHands

                        //Get Weapon Name
                        _weaponData.WeaponName = _LocalizedHelper.Localized(_currentWeapon.Name);

                        _weaponData.WeaponShortName = _LocalizedHelper.Localized(_currentWeapon.ShortName);

                        //Get Fire Mode
                        _weaponData.FireMode = _LocalizedHelper.Localized(_currentWeapon.SelectedFireMode.ToString());

                        _weaponData.AmmoType = _LocalizedHelper.Localized(_currentWeapon.AmmoCaliber);

                        if (_currentWeapon.ReloadMode != Weapon.EReloadMode.OnlyBarrel)
                        {
                            var magInWeapon = _animatorWeapon.GetBool(AnimatorHash.MagInWeapon);
                            var magSame = _currentMag == _oldMag;

                            var ammoInChamber = (int)_animatorWeapon.GetFloat(AnimatorHash.AmmoInChamber);
                            var chambersCount = _currentWeapon.ChamberAmmoCount;
                            var maxMagazineCount = _currentWeapon.GetMaxMagazineCount();

                            _currentMag = _PlayerHelper.WeaponHelper.CurrentMagazine;

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
                                    var maxCount = maxMagazineCount;

                                    _weaponData.Patron = chambersCount;

                                    _weaponData.Normalized = (float)count / maxCount;

                                    _weaponData.MagCount = count;

                                    _weaponData.MagMaxCount = maxCount;
                                    break;
                                }
                                case false when _allReloadBool:
                                    _weaponData.Patron = ammoInChamber;

                                    _weaponData.Normalized = 0;

                                    _weaponData.MagCount = 0;

                                    _weaponData.MagMaxCount = 0;

                                    _oldMag = _currentMag;
                                    break;
                                case true when _allReloadBool && magSame:
                                {
                                    var count = (int)_animatorWeapon.GetFloat(AnimatorHash.AmmoInMag);
                                    var maxCount = maxMagazineCount;

                                    _weaponData.Patron = ammoInChamber;

                                    _weaponData.Normalized = (float)count / maxCount;

                                    _weaponData.MagCount = count;

                                    _weaponData.MagMaxCount = maxCount;
                                    break;
                                }
                                case false when ammoInChamber != 0 && !_allReloadBool:
                                    _weaponData.Patron = chambersCount;

                                    _weaponData.Normalized = 0;

                                    _weaponData.MagCount = 0;

                                    _weaponData.MagMaxCount = 0;
                                    break;
                                case false when !_allReloadBool:
                                    _weaponData.Patron = 0;

                                    _weaponData.Normalized = 0;

                                    _weaponData.MagCount = 0;

                                    _weaponData.MagMaxCount = 0;
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
                                    var maxCount = chambersCount;

                                    _weaponData.Patron = 0;

                                    _weaponData.MagCount = count;

                                    _weaponData.MagMaxCount = maxCount;

                                    _weaponData.Normalized = (float)count / maxCount - 0.1f;
                                    break;
                                }
                                case true:
                                {
                                    var count = ammoInChamber;
                                    var maxCount = chambersCount;

                                    _weaponData.Patron = 0;

                                    _weaponData.MagCount = count;

                                    _weaponData.MagMaxCount = maxCount;

                                    _weaponData.Normalized = (float)count / maxCount - 0.1f;
                                    break;
                                }
                                case false:
                                    _weaponData.Patron = 0;

                                    _weaponData.MagCount = 0;

                                    _weaponData.MagMaxCount = chambersCount;

                                    _weaponData.Normalized = 0;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        _allReloadBool = currentState == 1285477936; //1.LauncherReload

                        //Get Weapon Name
                        _weaponData.WeaponName = _LocalizedHelper.Localized(((Item)_currentLauncher).Name);

                        _weaponData.WeaponShortName = _LocalizedHelper.Localized(((Item)_currentLauncher).ShortName);

                        var launcherTemplate = _PlayerHelper.WeaponHelper.UnderbarrelWeaponTemplate;

                        //Get Fire Mode
                        _weaponData.FireMode = _LocalizedHelper.Localized(nameof(Weapon.EFireMode.single));

                        _weaponData.AmmoType = _LocalizedHelper.Localized(launcherTemplate.ammoCaliber);

                        var ammoInChamber = (int)_animatorLauncher.GetFloat(AnimatorHash.AmmoInChamber);
                        var chambersCount = _PlayerHelper.WeaponHelper.UnderbarrelChambers.Length;

                        switch (_allReloadBool)
                        {
                            case false when ammoInChamber != 0:
                            {
                                var count = _PlayerHelper.WeaponHelper.UnderbarrelChamberAmmoCount;
                                var maxCount = chambersCount;

                                _weaponData.Patron = 0;

                                _weaponData.MagCount = count;

                                _weaponData.MagMaxCount = maxCount;

                                _weaponData.Normalized = (float)count / maxCount - 0.1f;
                                break;
                            }
                            case true:
                            {
                                var count = ammoInChamber;
                                var maxCount = chambersCount;

                                _weaponData.Patron = 0;

                                _weaponData.MagCount = ammoInChamber;

                                _weaponData.MagMaxCount = maxCount;

                                _weaponData.Normalized = (float)count / maxCount - 0.1f;
                                break;
                            }
                            case false:
                                _weaponData.Patron = 0;

                                _weaponData.MagCount = 0;

                                _weaponData.MagMaxCount = chambersCount;

                                _weaponData.Normalized = 0;
                                break;
                        }
                    }
                }
            }
        }

        public class WeaponData
        {
            public int Patron;

            public int MagCount;

            public int MagMaxCount;

            public float Normalized;

            public string WeaponName;

            public string WeaponShortName;

            public string AmmoType;

            public string FireMode;

            public bool WeaponNameAlways;
        }

        public class SettingsData
        {
            public readonly ConfigEntry<bool> KeyMagHUDSw;
            public readonly ConfigEntry<bool> KeyFireModeHUDSw;
            public readonly ConfigEntry<bool> KeyAmmoTypeHUDSw;
            public readonly ConfigEntry<bool> KeyWeaponNameAlways;
            public readonly ConfigEntry<bool> KeyWeaponShortName;
            public readonly ConfigEntry<bool> KeyZeroWarning;
            public readonly ConfigEntry<bool> KeyLookWeaponName;
            public readonly ConfigEntry<bool> KeyAutoWeaponName;
            public readonly ConfigEntry<bool> KeyHideGameAmmoPanel;

            public readonly ConfigEntry<Vector2> KeyAnchoredPosition;
            public readonly ConfigEntry<Vector2> KeyLocalScale;

            public readonly ConfigEntry<int> KeyWarningRate10;
            public readonly ConfigEntry<int> KeyWarningRate100;
            public readonly ConfigEntry<float> KeyWeaponNameSpeed;
            public readonly ConfigEntry<float> KeyZeroWarningSpeed;

            public readonly ConfigEntry<Color> KeyCurrentColor;
            public readonly ConfigEntry<Color> KeyMaxColor;
            public readonly ConfigEntry<Color> KeyPatronColor;
            public readonly ConfigEntry<Color> KeyWeaponNameColor;
            public readonly ConfigEntry<Color> KeyAmmoTypeColor;
            public readonly ConfigEntry<Color> KeyFireModeColor;
            public readonly ConfigEntry<Color> KeyAddZerosColor;
            public readonly ConfigEntry<Color> KeyWarningColor;

            public readonly ConfigEntry<FontStyles> KeyCurrentStyles;
            public readonly ConfigEntry<FontStyles> KeyMaximumStyles;
            public readonly ConfigEntry<FontStyles> KeyPatronStyles;
            public readonly ConfigEntry<FontStyles> KeyWeaponNameStyles;
            public readonly ConfigEntry<FontStyles> KeyAmmoTypeStyles;
            public readonly ConfigEntry<FontStyles> KeyFireModeStyles;

            public SettingsData(ConfigFile configFile)
            {
                const string mainSettings = "Main Settings";
                const string positionScaleSettings = "Position Scale Settings";
                const string colorSettings = "Color Settings";
                const string fontStylesSettings = "Font Styles Settings";
                const string warningRateSettings = "Warning Rate Settings";
                const string speedSettings = "Animation Speed Settings";

                KeyMagHUDSw = configFile.Bind<bool>(mainSettings, "Mag HUD display", true);
                KeyAmmoTypeHUDSw = configFile.Bind<bool>(mainSettings, "Ammo Type HUD display", true);
                KeyFireModeHUDSw = configFile.Bind<bool>(mainSettings, "Fire Mode display", true);
                KeyWeaponNameAlways = configFile.Bind<bool>(mainSettings, "Weapon Name Always display", false);
                KeyWeaponShortName = configFile.Bind<bool>(mainSettings, "Weapon ShortName", false);
                KeyZeroWarning = configFile.Bind<bool>(mainSettings, "Zero Warning Animation", true);
                KeyLookWeaponName = configFile.Bind<bool>(mainSettings, "Weapon Name Inspect display", true);
                KeyAutoWeaponName = configFile.Bind<bool>(mainSettings, "Weapon Name Auto display", true);
                KeyHideGameAmmoPanel = configFile.Bind<bool>(mainSettings, "Hide Game Ammo Panel", false);

                KeyAnchoredPosition =
                    configFile.Bind<Vector2>(positionScaleSettings, "Anchored Position", new Vector2(-100, 40));
                KeyLocalScale =
                    configFile.Bind<Vector2>(positionScaleSettings, "Local Scale", new Vector2(1, 1));

                KeyWarningRate10 = configFile.Bind<int>(warningRateSettings, "Max Ammo Within 10", 45,
                    new ConfigDescription(
                        "When Max Ammo <= 10 and Current Ammo <= 45%, Current Color change to Warning",
                        new AcceptableValueRange<int>(0, 100)));
                KeyWarningRate100 = configFile.Bind<int>(warningRateSettings, "Max Ammo Within 100", 30,
                    new ConfigDescription("When Max Ammo > 10 and Current Ammo < 30%, Current Color change to Warning",
                        new AcceptableValueRange<int>(0, 100)));

                KeyWeaponNameSpeed = configFile.Bind<float>(speedSettings,
                    "Weapon Name Auto display Animation Speed", 1,
                    new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));
                KeyZeroWarningSpeed = configFile.Bind<float>(speedSettings, "Zero Warning Animation Speed", 1,
                    new ConfigDescription(string.Empty, new AcceptableValueRange<float>(0, 10)));

                KeyCurrentColor = configFile.Bind<Color>(colorSettings, "Current",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
                KeyMaxColor = configFile.Bind<Color>(colorSettings, "Maximum",
                    new Color(0.5882353f, 0.6039216f, 0.6078432f)); //#969A9B
                KeyPatronColor =
                    configFile.Bind<Color>(colorSettings, "Patron",
                        new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
                KeyWeaponNameColor = configFile.Bind<Color>(colorSettings, "Weapon Name",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
                KeyAmmoTypeColor = configFile.Bind<Color>(colorSettings, "Ammo Type",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
                KeyFireModeColor = configFile.Bind<Color>(colorSettings, "Fire Mode",
                    new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
                KeyAddZerosColor =
                    configFile.Bind<Color>(colorSettings, "Zeros", new Color(0.6f, 0.6f, 0.6f, 0.5f)); //#9999
                KeyWarningColor =
                    configFile.Bind<Color>(colorSettings, "Warning", new Color(0.7294118f, 0f, 0f)); //#BA0000

                KeyCurrentStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Current", FontStyles.Bold);
                KeyMaximumStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Maximum", FontStyles.Normal);
                KeyPatronStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Patron", FontStyles.Normal);
                KeyWeaponNameStyles =
                    configFile.Bind<FontStyles>(fontStylesSettings, "Weapon Name", FontStyles.Normal);
                KeyAmmoTypeStyles =
                    configFile.Bind<FontStyles>(fontStylesSettings, "Ammo Type", FontStyles.Normal);
                KeyFireModeStyles =
                    configFile.Bind<FontStyles>(fontStylesSettings, "Fire Mode", FontStyles.Normal);
            }
        }

        private class ReflectionData
        {
            public readonly RefHelper.FieldRef<BattleUIScreen, AmmoCountPanel> AmmoCountPanel;
            public readonly RefHelper.FieldRef<AmmoCountPanel, CustomTextMeshProUGUI> AmmoCount;

            public ReflectionData()
            {
                AmmoCountPanel =
                    RefHelper.FieldRef<BattleUIScreen, AmmoCountPanel>.Create("_ammoCountPanel");
                AmmoCount = RefHelper.FieldRef<AmmoCountPanel, CustomTextMeshProUGUI>.Create("_ammoCount");
            }
        }
    }
}
#endif