#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using System;
using System.Reflection;
using UnityEngine;
using TMPro;
using EFT;
using EFT.InventoryLogic;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDMag
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDMag", "kmyuhkyuk-GamePanelHUDMag", "2.3.0")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class GamePanelHUDMagPlugin : BaseUnityPlugin, IUpdate
    {
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore
        {
            get
            {
                return GamePanelHUDCorePlugin.HUDCore;
            }
        }

        internal static readonly GamePanelHUDCorePlugin.HUDClass<WeaponData, SettingsData> HUD = new GamePanelHUDCorePlugin.HUDClass<WeaponData, SettingsData>();

        private readonly ReflectionData ReflectionDatas = new ReflectionData(); 

        private bool MagHUDSW;

        private WeaponPrefab WeaponPrefab;

        private Weapon Weapon;

        private Weapon OldWeapon;

        private object Mag;

        private object OldMag;

        private Animator Animator_Weapon;

        private readonly WeaponData WeaponDatas = new WeaponData();

        private readonly SettingsData SettingsDatas = new SettingsData();

        private object FirearmController;

        private object FirearmsAnimator;

        private bool AllReloadbool;

        private bool WeaponCachebool = true;

        private bool MagCachebool = true;

        internal static Action WeaponTirgger;

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDMag");

            string mainSettings = "主设置 Main Settings";
            string positionScaleSettings = "位置大小设置 Position Scale Settings";
            string colorSettings = "颜色设置 Color Settings";
            string fontStylesSettings = "字体样式设置 Font Styles Settings";
            string warningRateSettings = "警告率设置 Warning Rate Settings";
            string speedSettings = "动画速度设置 Animation Speed Settings";

            SettingsDatas.KeyMagHUDSW = Config.Bind<bool>(mainSettings, "弹药计数器显示 Mag HUD display", true);
            SettingsDatas.KeyAmmoTypeHUDSW = Config.Bind<bool>(mainSettings, "弹药类型显示 Ammo Type HUD display", true);
            SettingsDatas.KeyFireModeHUDSW = Config.Bind<bool>(mainSettings, "开火模式显示 Fire Mode display", true);
            SettingsDatas.KeyWeaponNameAlways = Config.Bind<bool>(mainSettings, "武器名始终显示 Weapon Name Always display", false);
            SettingsDatas.KeyWeaponShortName = Config.Bind<bool>(mainSettings, "武器短名 Weapon ShortName", false);
            SettingsDatas.KeyZeroWarning = Config.Bind<bool>(mainSettings, "零警告动画 Zero Warning Animation", true);
            SettingsDatas.KeyLockWeaponName = Config.Bind<bool>(mainSettings, "检视时显示武器名 Weapon Name Inspect display", true);
            SettingsDatas.KeyAutoWeaponName = Config.Bind<bool>(mainSettings, "自动显示武器名 Weapon Name Auto display", true);

            SettingsDatas.KeyAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "指示栏位置 Anchored Position", new Vector2(-100, 40));
            SettingsDatas.KeyLocalScale = Config.Bind<Vector2>(positionScaleSettings, "指示栏大小 Local Scale", new Vector2(1, 1));

            SettingsDatas.KeyWarningRate10 = Config.Bind<int>(warningRateSettings, "Max Ammo Within 10", 45, new ConfigDescription("When Max Ammo <= 10 and Current Ammo <= 45%, Current Color change to Red Warning", new AcceptableValueRange<int>(0, 100)));
            SettingsDatas.KeyWarningRate100 = Config.Bind<int>(warningRateSettings, "Max Ammo Within 100", 30, new ConfigDescription("When Max Ammo > 10 and Current Ammo < 30%, Current Color change to Red Warning", new AcceptableValueRange<int>(0, 100)));

            SettingsDatas.KeyWeaponNameSpeed = Config.Bind<float>(speedSettings, "武器名动画速度 Weapon Name Auto display Animation Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SettingsDatas.KeyZeroWarningSpeed = Config.Bind<float>(speedSettings, "零警告动画速度 Zero Warning Animation Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));

            SettingsDatas.KeyCurrentColor = Config.Bind<Color>(colorSettings, "当前值 Current", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SettingsDatas.KeyMaxColor = Config.Bind<Color>(colorSettings, "最大值 Maximum", new Color(0.5882353f, 0.6039216f, 0.6078432f)); //#969A9B
            SettingsDatas.KeyPatronColor = Config.Bind<Color>(colorSettings, "枪膛 Patron", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SettingsDatas.KeyWeaponNameColor = Config.Bind<Color>(colorSettings, "武器名字 Weapon Name", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SettingsDatas.KeyAmmonTypeColor = Config.Bind<Color>(colorSettings, "弹药类型 Ammon Type", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SettingsDatas.KeyFireModeColor = Config.Bind<Color>(colorSettings, "开火模式 Fire Mode", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SettingsDatas.KeyAddZerosColor = Config.Bind<Color>(colorSettings, "零 Zeros", new Color(0.6f, 0.6f, 0.6f, 0.5f)); //#9999
            SettingsDatas.KeyWarningColor = Config.Bind<Color>(colorSettings, "警告 Warning", new Color(0.7294118f, 0f, 0f)); //#BA0000

            SettingsDatas.KeyCurrentStyles = Config.Bind<FontStyles>(fontStylesSettings, "当前值 Current", FontStyles.Bold);
            SettingsDatas.KeyMaximumStyles = Config.Bind<FontStyles>(fontStylesSettings, "最大值 Maximum", FontStyles.Normal);
            SettingsDatas.KeyPatronStyles = Config.Bind<FontStyles>(fontStylesSettings, "枪膛 Patron", FontStyles.Normal);
            SettingsDatas.KeyWeaponNameStyles = Config.Bind<FontStyles>(fontStylesSettings, "武器名字 Weapon Name", FontStyles.Normal);
            SettingsDatas.KeyAmmonTypeStyles = Config.Bind<FontStyles>(fontStylesSettings, "弹药类型 AmmonType", FontStyles.Normal);
            SettingsDatas.KeyFireModeStyles = Config.Bind<FontStyles>(fontStylesSettings, "开火模式 Fire Mode", FontStyles.Normal);

            ReflectionDatas.RefWeaponPrefab = RefHelp.FieldRef<object, WeaponPrefab>.Create(typeof(Player.FirearmController), new string[] { "WeaponPrefab_0", "weaponPrefab_0" }); //0.12.12.15.17349, 0.12.12.32.19904
            ReflectionDatas.RefWeapon = RefHelp.FieldRef<WeaponPrefab, Weapon>.Create(new string[] { "Weapon_0", "weapon_0" }); //0.12.12.15.17349, 0.12.12.32.19904

            ReflectionDatas.RefIAnimator = RefHelp.PropertyRef<object, object>.Create(typeof(FirearmsAnimator), "Animator");
            ReflectionDatas.RefAnimator = RefHelp.PropertyRef<object, Animator>.Create(RefHelp.GetEftType(x => x.GetMethod("CreateAnimatorStateInfoWrapper", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance) != null), "Animator");

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            HUDCore.LoadHUD("gamepanlmaghud.bundle", new string[] { "gamepanlmaghud" });
        }

        public void IUpdate()
        {
            MagPlugin();
        }

        void MagPlugin()
        {
            MagHUDSW = HUDCore.AllHUDSW && Weapon != null && HUDCore.HasPlayer && SettingsDatas.KeyMagHUDSW.Value;

            HUD.Set(WeaponDatas, SettingsDatas, MagHUDSW);

            //Get Player
            if (HUDCore.HasPlayer)
            {
                FirearmController = HUDCore.IsYourPlayer.HandsController;
                FirearmsAnimator = HUDCore.IsYourPlayer.HandsAnimator;

                //Get WeaponPrefab
                WeaponPrefab = ReflectionDatas.RefWeaponPrefab.GetValue(FirearmController);

                //Get Weapon Class
                Weapon = ReflectionDatas.RefWeapon.GetValue(WeaponPrefab);

                bool weaponActive = Weapon != null;

                if (WeaponCachebool)
                {
                    OldWeapon = Weapon;

                    if (!SettingsDatas.KeyWeaponNameAlways.Value && SettingsDatas.KeyAutoWeaponName.Value)
                    {
                        WeaponTirgger();
                    }

                    WeaponCachebool = false;
                }
                //not same Weapon trigger
                else if (Weapon != OldWeapon && weaponActive || !weaponActive)
                {
                    WeaponCachebool = true;
                    MagCachebool = true;
                }

                //Get Ammon Count
                
                if (weaponActive)
                {
                    Animator_Weapon = ReflectionDatas.RefAnimator.GetValue(ReflectionDatas.RefIAnimator.GetValue(FirearmsAnimator));

                    //Get Weapon Name
                    WeaponDatas.WeaponName = LocalizedHelp.Localized(Weapon.Name, EStringCase.None);

                    WeaponDatas.WeaponShortName = LocalizedHelp.Localized(Weapon.ShortName, EStringCase.None);

                    //Get Fire Mode
                    WeaponDatas.FireMode = LocalizedHelp.Localized(Weapon.SelectedFireMode.ToString(), EStringCase.None);

                    WeaponDatas.AmmonType = LocalizedHelp.Localized(Weapon.AmmoCaliber, EStringCase.None);

                    int currentState = Animator_Weapon.GetCurrentAnimatorStateInfo(1).fullPathHash;

                    AllReloadbool = ReloadOperation.IsInReloadOperation((Player.FirearmController)FirearmController) || currentState == 1058993437;

                    WeaponDatas.WeaponNameAlways = SettingsDatas.KeyWeaponNameAlways.Value || currentState == 1355507738 && SettingsDatas.KeyLockWeaponName.Value;

                    //MagCount and PatronCount
                    if (Weapon.ReloadMode != Weapon.EReloadMode.OnlyBarrel)
                    {
                        bool magInWeapon = Animator_Weapon.GetBool(AnimatorHash.MagInWeapon);
                        bool magSame = Mag == OldMag;

                        Mag = GetMag.GetCurrentMagazine(Weapon);

                        if (MagCachebool)
                        {
                            OldMag = Mag;

                            MagCachebool = false;
                        }

                        if (magInWeapon && !AllReloadbool && magSame)
                        {
                            int count = Weapon.GetCurrentMagazineCount();
                            int maxCount = Weapon.GetMaxMagazineCount();

                            WeaponDatas.Patron = Weapon.ChamberAmmoCount;

                            WeaponDatas.Normalized = (float)count / (float)maxCount;

                            WeaponDatas.MagCount = count;

                            WeaponDatas.MagMaxCount = maxCount;
                        }
                        else if (!magInWeapon && AllReloadbool)
                        {
                            WeaponDatas.Patron = (int)Animator_Weapon.GetFloat(AnimatorHash.AmmoInChamber);

                            WeaponDatas.Normalized = 0;

                            WeaponDatas.MagCount = 0;

                            WeaponDatas.MagMaxCount = 0;

                            OldMag = Mag;
                        }
                        else if (magInWeapon && AllReloadbool && magSame)
                        {
                            int count = (int)Animator_Weapon.GetFloat(AnimatorHash.AmmoInMag);
                            int maxCount = Weapon.GetMaxMagazineCount();

                            WeaponDatas.Patron = (int)Animator_Weapon.GetFloat(AnimatorHash.AmmoInChamber);

                            WeaponDatas.Normalized = (float)count / (float)maxCount;

                            WeaponDatas.MagCount = count;

                            WeaponDatas.MagMaxCount = maxCount;
                        }
                        else if (!magInWeapon && Animator_Weapon.GetFloat(AnimatorHash.AmmoInChamber) != 0 && !AllReloadbool)
                        {
                            WeaponDatas.Patron = Weapon.ChamberAmmoCount;

                            WeaponDatas.Normalized = 0;

                            WeaponDatas.MagCount = 0;

                            WeaponDatas.MagMaxCount = 0;
                        }
                        else if (!magInWeapon && !AllReloadbool)
                        {
                            WeaponDatas.Patron = 0;

                            WeaponDatas.Normalized = 0;

                            WeaponDatas.MagCount = 0;

                            WeaponDatas.MagMaxCount = 0;
                        }
                    }
                    else
                    {
                        if (!AllReloadbool && Animator_Weapon.GetFloat(AnimatorHash.AmmoInChamber) != 0)
                        {
                            WeaponDatas.Patron = 0;

                            WeaponDatas.MagCount = Weapon.ChamberAmmoCount;

                            WeaponDatas.MagMaxCount = Weapon.Chambers.Length;

                            WeaponDatas.Normalized = (float)WeaponDatas.MagCount / (float)WeaponDatas.MagMaxCount - 0.1f;
                        }
                        else if (AllReloadbool && Animator_Weapon.GetBool("Armed"))
                        {
                            WeaponDatas.Patron = 0;

                            WeaponDatas.MagCount = (int)Animator_Weapon.GetFloat(AnimatorHash.AmmoInChamber);

                            WeaponDatas.MagMaxCount = Weapon.Chambers.Length;

                            WeaponDatas.Normalized = (float)WeaponDatas.MagCount / (float)WeaponDatas.MagMaxCount - 0.1f;
                        }
                        else if (!AllReloadbool)
                        {
                            WeaponDatas.Patron = 0;

                            WeaponDatas.MagCount = 0;

                            WeaponDatas.MagMaxCount = Weapon.Chambers.Length;

                            WeaponDatas.Normalized = 0;
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

            public string AmmonType;

            public string FireMode;

            public bool WeaponNameAlways;
        }

        public class SettingsData
        {
            public ConfigEntry<bool> KeyMagHUDSW;
            public ConfigEntry<bool> KeyFireModeHUDSW;
            public ConfigEntry<bool> KeyAmmoTypeHUDSW;
            public ConfigEntry<bool> KeyWeaponNameAlways;
            public ConfigEntry<bool> KeyWeaponShortName;
            public ConfigEntry<bool> KeyZeroWarning;
            public ConfigEntry<bool> KeyLockWeaponName;
            public ConfigEntry<bool> KeyAutoWeaponName;

            public ConfigEntry<Vector2> KeyAnchoredPosition;
            public ConfigEntry<Vector2> KeyLocalScale;

            public ConfigEntry<int> KeyWarningRate10;
            public ConfigEntry<int> KeyWarningRate100;
            public ConfigEntry<float> KeyWeaponNameSpeed;
            public ConfigEntry<float> KeyZeroWarningSpeed;

            public ConfigEntry<Color> KeyCurrentColor;
            public ConfigEntry<Color> KeyMaxColor;
            public ConfigEntry<Color> KeyPatronColor;
            public ConfigEntry<Color> KeyWeaponNameColor;
            public ConfigEntry<Color> KeyAmmonTypeColor;
            public ConfigEntry<Color> KeyFireModeColor;
            public ConfigEntry<Color> KeyAddZerosColor;
            public ConfigEntry<Color> KeyWarningColor;

            public ConfigEntry<FontStyles> KeyCurrentStyles;
            public ConfigEntry<FontStyles> KeyMaximumStyles;
            public ConfigEntry<FontStyles> KeyPatronStyles;
            public ConfigEntry<FontStyles> KeyWeaponNameStyles;
            public ConfigEntry<FontStyles> KeyAmmonTypeStyles;
            public ConfigEntry<FontStyles> KeyFireModeStyles;
        }

        public class ReflectionData
        {
            public RefHelp.FieldRef<object, WeaponPrefab> RefWeaponPrefab;
            public RefHelp.FieldRef<WeaponPrefab, Weapon> RefWeapon;

            public RefHelp.PropertyRef<object, object> RefIAnimator;
            public RefHelp.PropertyRef<object, Animator> RefAnimator;
        }
    }
}
#endif
