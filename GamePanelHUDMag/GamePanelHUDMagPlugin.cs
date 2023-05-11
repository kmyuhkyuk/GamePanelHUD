#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using System;
using System.Reflection;
using UnityEngine;
using TMPro;
using EFT;
using EFT.UI;
using EFT.InventoryLogic;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDMag
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDMag", "kmyuhkyuk-GamePanelHUDMag", "2.6.4")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class GamePanelHUDMagPlugin : BaseUnityPlugin, IUpdate
    {
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

        internal static readonly GamePanelHUDCorePlugin.HUDClass<WeaponData, SettingsData> HUD = new GamePanelHUDCorePlugin.HUDClass<WeaponData, SettingsData>();

        private readonly ReflectionData RefData = new ReflectionData(); 

        private bool MagHUDSw;

        private Weapon NowWeapon;

        private Weapon OldWeapon;

        private object NowLauncher;

        private object NowMag;

        private object OldMag;

        private Animator Animator_Weapon;

        private Animator Animator_Launcher;

        private readonly WeaponData WData = new WeaponData();

        private readonly SettingsData SetData = new SettingsData();

        private Player.FirearmController NowFirearmController;

        private bool AllReloadBool;

        private bool WeaponCacheBool = true;

        private bool MagCacheBool = true;

        private bool LauncherCacheBool;

        private static readonly bool Is341Up = GamePanelHUDCorePlugin.HUDCoreClass.GameVersion > new Version("0.12.12.20765"); //3.5.0 Add Launcher

        internal static Action WeaponTrigger;

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDMag");

            ModUpdateCheck.DrawCheck(this);

            const string mainSettings = "主设置 Main Settings";
            const string positionScaleSettings = "位置大小设置 Position Scale Settings";
            const string colorSettings = "颜色设置 Color Settings";
            const string fontStylesSettings = "字体样式设置 Font Styles Settings";
            const string warningRateSettings = "警告率设置 Warning Rate Settings";
            const string speedSettings = "动画速度设置 Animation Speed Settings";

            SetData.KeyMagHUDSw = Config.Bind<bool>(mainSettings, "弹药计数器显示 Mag HUD display", true);
            SetData.KeyAmmoTypeHUDSw = Config.Bind<bool>(mainSettings, "弹药类型显示 Ammo Type HUD display", true);
            SetData.KeyFireModeHUDSw = Config.Bind<bool>(mainSettings, "开火模式显示 Fire Mode display", true);
            SetData.KeyWeaponNameAlways = Config.Bind<bool>(mainSettings, "武器名始终显示 Weapon Name Always display", false);
            SetData.KeyWeaponShortName = Config.Bind<bool>(mainSettings, "武器短名 Weapon ShortName", false);
            SetData.KeyZeroWarning = Config.Bind<bool>(mainSettings, "零警告动画 Zero Warning Animation", true);
            SetData.KeyLockWeaponName = Config.Bind<bool>(mainSettings, "检视时显示武器名 Weapon Name Inspect display", true);
            SetData.KeyAutoWeaponName = Config.Bind<bool>(mainSettings, "自动显示武器名 Weapon Name Auto display", true);
            SetData.KeyHideGameAmmoPanel = Config.Bind<bool>(mainSettings, "隐藏游戏弹药面板 Hide Game Ammo Panel", false);

            SetData.KeyAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "指示栏位置 Anchored Position", new Vector2(-100, 40));
            SetData.KeyLocalScale = Config.Bind<Vector2>(positionScaleSettings, "指示栏大小 Local Scale", new Vector2(1, 1));

            SetData.KeyWarningRate10 = Config.Bind<int>(warningRateSettings, "Max Ammo Within 10", 45, new ConfigDescription("When Max Ammo <= 10 and Current Ammo <= 45%, Current Color change to Red Warning", new AcceptableValueRange<int>(0, 100)));
            SetData.KeyWarningRate100 = Config.Bind<int>(warningRateSettings, "Max Ammo Within 100", 30, new ConfigDescription("When Max Ammo > 10 and Current Ammo < 30%, Current Color change to Red Warning", new AcceptableValueRange<int>(0, 100)));

            SetData.KeyWeaponNameSpeed = Config.Bind<float>(speedSettings, "武器名动画速度 Weapon Name Auto display Animation Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));
            SetData.KeyZeroWarningSpeed = Config.Bind<float>(speedSettings, "零警告动画速度 Zero Warning Animation Speed", 1, new ConfigDescription("", new AcceptableValueRange<float>(0, 10)));

            SetData.KeyCurrentColor = Config.Bind<Color>(colorSettings, "当前值 Current", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SetData.KeyMaxColor = Config.Bind<Color>(colorSettings, "最大值 Maximum", new Color(0.5882353f, 0.6039216f, 0.6078432f)); //#969A9B
            SetData.KeyPatronColor = Config.Bind<Color>(colorSettings, "枪膛 Patron", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SetData.KeyWeaponNameColor = Config.Bind<Color>(colorSettings, "武器名字 Weapon Name", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SetData.KeyAmmoTypeColor = Config.Bind<Color>(colorSettings, "弹药类型 Ammo Type", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SetData.KeyFireModeColor = Config.Bind<Color>(colorSettings, "开火模式 Fire Mode", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SetData.KeyAddZerosColor = Config.Bind<Color>(colorSettings, "零 Zeros", new Color(0.6f, 0.6f, 0.6f, 0.5f)); //#9999
            SetData.KeyWarningColor = Config.Bind<Color>(colorSettings, "警告 Warning", new Color(0.7294118f, 0f, 0f)); //#BA0000

            SetData.KeyCurrentStyles = Config.Bind<FontStyles>(fontStylesSettings, "当前值 Current", FontStyles.Bold);
            SetData.KeyMaximumStyles = Config.Bind<FontStyles>(fontStylesSettings, "最大值 Maximum", FontStyles.Normal);
            SetData.KeyPatronStyles = Config.Bind<FontStyles>(fontStylesSettings, "枪膛 Patron", FontStyles.Normal);
            SetData.KeyWeaponNameStyles = Config.Bind<FontStyles>(fontStylesSettings, "武器名字 Weapon Name", FontStyles.Normal);
            SetData.KeyAmmoTypeStyles = Config.Bind<FontStyles>(fontStylesSettings, "弹药类型 AmmoType", FontStyles.Normal);
            SetData.KeyFireModeStyles = Config.Bind<FontStyles>(fontStylesSettings, "开火模式 Fire Mode", FontStyles.Normal);

            RefData.RefIAnimator = RefHelp.PropertyRef<Player, object>.Create("ArmsAnimatorCommon");
            RefData.RefAnimator = RefHelp.PropertyRef<object, Animator>.Create(RefHelp.GetEftType(x => x.GetMethod("CreateAnimatorStateInfoWrapper", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance) != null), "Animator");
            RefData.RefAmmoCountPanel = RefHelp.FieldRef<BattleUIScreen, AmmoCountPanel>.Create("_ammoCountPanel");
            RefData.RefAmmoCount = RefHelp.FieldRef<AmmoCountPanel, CustomTextMeshProUGUI>.Create("_ammoCount");

            if (Is341Up)
            {
                Type launcherType = RefHelp.GetEftType(x => x.GetMethod("GetCenterOfImpact", BindingFlags.Public | BindingFlags.Instance) != null);

                RefData.RefUnderbarrelWeapon = RefHelp.FieldRef<Player.FirearmController, Item>.Create("UnderbarrelWeapon");
                RefData.RefChambers = RefHelp.FieldRef<object, Slot[]>.Create(launcherType, "Chambers");

                RefData.RefWeaponTemplate = RefHelp.PropertyRef<object, WeaponTemplate>.Create(launcherType, "WeaponTemplate");
                RefData.RefLauncherIAnimator = RefHelp.PropertyRef<Player, object>.Create("UnderbarrelWeaponArmsAnimator");
                RefData.RefChamberAmmoCount = RefHelp.PropertyRef<object, int>.Create(launcherType, "ChamberAmmoCount");
            }

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            GamePanelHUDCorePlugin.HUDCoreClass.LoadHUD("gamepanelmaghud.bundle", "GamePanelMagHUD");
        }

        public void IUpdate()
        {
            MagPlugin();
        }

        void MagPlugin()
        {
            MagHUDSw = HUDCore.AllHUDSw && NowWeapon != null && HUDCore.HasPlayer && SetData.KeyMagHUDSw.Value;

            HUD.Set(WData, SetData, MagHUDSw);

            //Get Player
            if (HUDCore.HasPlayer)
            {
                RefData.RefAmmoCount.GetValue(RefData.RefAmmoCountPanel.GetValue(HUDCore.YourGameUI.BattleUiScreen)).gameObject.SetActive(!SetData.KeyHideGameAmmoPanel.Value);

                NowFirearmController = HUDCore.YourPlayer.HandsController as Player.FirearmController;

                //Get Weapon Class
                NowWeapon = NowFirearmController != null ? NowFirearmController.Item : null;
                Animator_Weapon = RefData.RefAnimator.GetValue(RefData.RefIAnimator.GetValue(HUDCore.YourPlayer));

                if (Is341Up)
                {
                    NowLauncher = RefData.RefUnderbarrelWeapon.GetValue(NowFirearmController); 
                    Animator_Launcher = RefData.RefAnimator.GetValue(RefData.RefLauncherIAnimator.GetValue(HUDCore.YourPlayer));
                }

                bool weaponActive = NowWeapon != null;

                bool launcherActive = weaponActive && NowFirearmController != null && NowFirearmController.IsInLauncherMode();

                if (WeaponCacheBool)
                {
                    OldWeapon = NowWeapon;

                    if (!SetData.KeyWeaponNameAlways.Value && SetData.KeyAutoWeaponName.Value)
                    {
                        WeaponTrigger();
                    }

                    WeaponCacheBool = false;
                    LauncherCacheBool = false;
                }
                //Not same Weapon trigger
                else if (NowWeapon != OldWeapon && weaponActive || !weaponActive)
                {
                    WeaponCacheBool = true;
                    MagCacheBool = true;
                }

                //Switch Launcher trigger
                if (launcherActive != LauncherCacheBool)
                {
                    if (!SetData.KeyWeaponNameAlways.Value && SetData.KeyAutoWeaponName.Value)
                    {
                        WeaponTrigger();
                    }

                    LauncherCacheBool = launcherActive;
                }
                else if (!launcherActive)
                {
                    LauncherCacheBool = false;
                }

                //Get Ammo Count
                if (weaponActive)
                {
                    Animator currentAnimator = launcherActive ? Animator_Launcher : Animator_Weapon;

                    int currentState = currentAnimator.GetCurrentAnimatorStateInfo(1).fullPathHash;

                    WData.WeaponNameAlways = SetData.KeyWeaponNameAlways.Value || currentState == 1355507738 && SetData.KeyLockWeaponName.Value; //2.LookWeapon

                    //MagCount and PatronCount
                    if (!launcherActive)
                    {
                        AllReloadBool = NowFirearmController != null && NowFirearmController.IsInReloadOperation() || currentState == 1058993437; //1.OriginalReloadCheck 2.TakeHands

                        //Get Weapon Name
                        WData.WeaponName = LocalizedHelp.Localized(NowWeapon.Name, EStringCase.None);

                        WData.WeaponShortName = LocalizedHelp.Localized(NowWeapon.ShortName, EStringCase.None);

                        //Get Fire Mode
                        WData.FireMode = LocalizedHelp.Localized(NowWeapon.SelectedFireMode.ToString(), EStringCase.None);

                        WData.AmmoType = LocalizedHelp.Localized(NowWeapon.AmmoCaliber, EStringCase.None);

                        if (NowWeapon.ReloadMode != Weapon.EReloadMode.OnlyBarrel)
                        {
                            bool magInWeapon = Animator_Weapon.GetBool(AnimatorHash.MagInWeapon);
                            bool magSame = NowMag == OldMag;

                            int ammoInChamber = (int)Animator_Weapon.GetFloat(AnimatorHash.AmmoInChamber);
                            int chambersCount = NowWeapon.ChamberAmmoCount;
                            int maxMagazineCount = NowWeapon.GetMaxMagazineCount();

                            NowMag = GetMag.GetCurrentMagazine(NowWeapon);

                            if (MagCacheBool)
                            {
                                OldMag = NowMag;

                                MagCacheBool = false;
                            }

                            if (magInWeapon && !AllReloadBool && magSame)
                            {
                                int count = NowWeapon.GetCurrentMagazineCount();
                                int maxCount = maxMagazineCount;

                                WData.Patron = chambersCount;

                                WData.Normalized = (float)count / maxCount;

                                WData.MagCount = count;

                                WData.MagMaxCount = maxCount;
                            }
                            else if (!magInWeapon && AllReloadBool)
                            {
                                WData.Patron = ammoInChamber;

                                WData.Normalized = 0;

                                WData.MagCount = 0;

                                WData.MagMaxCount = 0;

                                OldMag = NowMag;
                            }
                            else if (magInWeapon && AllReloadBool && magSame)
                            {
                                int count = (int)Animator_Weapon.GetFloat(AnimatorHash.AmmoInMag);
                                int maxCount = maxMagazineCount;

                                WData.Patron = ammoInChamber;

                                WData.Normalized = (float)count / maxCount;

                                WData.MagCount = count;

                                WData.MagMaxCount = maxCount;
                            }
                            else if (!magInWeapon && ammoInChamber != 0 && !AllReloadBool)
                            {
                                WData.Patron = chambersCount;

                                WData.Normalized = 0;

                                WData.MagCount = 0;

                                WData.MagMaxCount = 0;
                            }
                            else if (!magInWeapon && !AllReloadBool)
                            {
                                WData.Patron = 0;

                                WData.Normalized = 0;

                                WData.MagCount = 0;

                                WData.MagMaxCount = 0;
                            }
                        }
                        else
                        {
                            int ammoInChamber = (int)Animator_Weapon.GetFloat(AnimatorHash.AmmoInChamber);
                            int chambersCount = NowWeapon.Chambers.Length;

                            if (!AllReloadBool && ammoInChamber != 0)
                            {
                                int count = NowWeapon.ChamberAmmoCount;
                                int maxCount = chambersCount;

                                WData.Patron = 0;

                                WData.MagCount = count;

                                WData.MagMaxCount = maxCount;

                                WData.Normalized = (float)count / maxCount - 0.1f;
                            }
                            else if (AllReloadBool)
                            {
                                int count = ammoInChamber;
                                int maxCount = chambersCount;

                                WData.Patron = 0;

                                WData.MagCount = count;

                                WData.MagMaxCount = maxCount;

                                WData.Normalized = (float)count / maxCount - 0.1f;
                            }
                            else if (!AllReloadBool)
                            {
                                WData.Patron = 0;

                                WData.MagCount = 0;

                                WData.MagMaxCount = chambersCount;

                                WData.Normalized = 0;
                            }
                        }
                    }
                    else
                    {
                        AllReloadBool = currentState == 1285477936; //1.LauncherReload

                        //Get Weapon Name
                        WData.WeaponName = LocalizedHelp.Localized(((Item)NowLauncher).Name, EStringCase.None);

                        WData.WeaponShortName = LocalizedHelp.Localized(((Item)NowLauncher).ShortName, EStringCase.None);

                        WeaponTemplate launcherTemplate = RefData.RefWeaponTemplate.GetValue(NowLauncher);

                        //Get Fire Mode
                        WData.FireMode = LocalizedHelp.Localized(nameof(Weapon.EFireMode.single), EStringCase.None);

                        WData.AmmoType = LocalizedHelp.Localized(launcherTemplate.ammoCaliber, EStringCase.None);

                        int ammoInChamber = (int)Animator_Launcher.GetFloat(AnimatorHash.AmmoInChamber);
                        int chambersCount = RefData.RefChambers.GetValue(NowLauncher).Length;

                        if (!AllReloadBool && ammoInChamber != 0)
                        {
                            int count = RefData.RefChamberAmmoCount.GetValue(NowLauncher);
                            int maxCount = chambersCount;

                            WData.Patron = 0;

                            WData.MagCount = count;

                            WData.MagMaxCount = maxCount;

                            WData.Normalized = (float)count / maxCount - 0.1f;
                        }
                        else if (AllReloadBool)
                        {
                            int count = ammoInChamber;
                            int maxCount = chambersCount;

                            WData.Patron = 0;

                            WData.MagCount = ammoInChamber;

                            WData.MagMaxCount = maxCount;

                            WData.Normalized = (float)count / maxCount - 0.1f;
                        }
                        else if (!AllReloadBool)
                        {
                            WData.Patron = 0;

                            WData.MagCount = 0;

                            WData.MagMaxCount = chambersCount;

                            WData.Normalized = 0;
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
            public ConfigEntry<bool> KeyMagHUDSw;
            public ConfigEntry<bool> KeyFireModeHUDSw;
            public ConfigEntry<bool> KeyAmmoTypeHUDSw;
            public ConfigEntry<bool> KeyWeaponNameAlways;
            public ConfigEntry<bool> KeyWeaponShortName;
            public ConfigEntry<bool> KeyZeroWarning;
            public ConfigEntry<bool> KeyLockWeaponName;
            public ConfigEntry<bool> KeyAutoWeaponName;
            public ConfigEntry<bool> KeyHideGameAmmoPanel;

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
            public ConfigEntry<Color> KeyAmmoTypeColor;
            public ConfigEntry<Color> KeyFireModeColor;
            public ConfigEntry<Color> KeyAddZerosColor;
            public ConfigEntry<Color> KeyWarningColor;

            public ConfigEntry<FontStyles> KeyCurrentStyles;
            public ConfigEntry<FontStyles> KeyMaximumStyles;
            public ConfigEntry<FontStyles> KeyPatronStyles;
            public ConfigEntry<FontStyles> KeyWeaponNameStyles;
            public ConfigEntry<FontStyles> KeyAmmoTypeStyles;
            public ConfigEntry<FontStyles> KeyFireModeStyles;
        }

        public class ReflectionData
        {
            public RefHelp.FieldRef<Player.FirearmController, Item> RefUnderbarrelWeapon;
            public RefHelp.FieldRef<object, Slot[]> RefChambers;
            public RefHelp.FieldRef<BattleUIScreen, AmmoCountPanel> RefAmmoCountPanel;
            public RefHelp.FieldRef<AmmoCountPanel, CustomTextMeshProUGUI> RefAmmoCount;

            public RefHelp.PropertyRef<object, WeaponTemplate> RefWeaponTemplate;
            public RefHelp.PropertyRef<object, int> RefChamberAmmoCount;
            public RefHelp.PropertyRef<Player, object> RefIAnimator;
            public RefHelp.PropertyRef<Player, object> RefLauncherIAnimator;
            public RefHelp.PropertyRef<object, Animator> RefAnimator;
        }
    }
}
#endif
