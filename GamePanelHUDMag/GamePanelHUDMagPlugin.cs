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
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDMag", "kmyuhkyuk-GamePanelHUDMag", "2.4.1")]
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

        private Weapon NowWeapon;

        private Weapon OldWeapon;

        private object NowLauncher;

        private object NowMag;

        private object OldMag;

        private Animator Animator_Weapon;

        private Animator Animator_Launcher;

        private readonly WeaponData WeaponDatas = new WeaponData();

        private readonly SettingsData SettingsDatas = new SettingsData();

        private Player.FirearmController NowFirearmController;

        private bool AllReloadBool;

        private bool WeaponCacheBool = true;

        private bool MagCacheBool = true;

        private bool LauncherCacheBool = false;

        private static readonly bool Is341Up = GamePanelHUDCorePlugin.HUDCoreClass.GameVersion > new Version("0.12.12.20765"); //3.5.0 Add Launcher

        internal static Action WeaponTirgger;

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDMag");

            ModUpdateCheck.DrawNeedUpdate(Config, Info.Metadata.Version);

            const string mainSettings = "主设置 Main Settings";
            const string positionScaleSettings = "位置大小设置 Position Scale Settings";
            const string colorSettings = "颜色设置 Color Settings";
            const string fontStylesSettings = "字体样式设置 Font Styles Settings";
            const string warningRateSettings = "警告率设置 Warning Rate Settings";
            const string speedSettings = "动画速度设置 Animation Speed Settings";

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

            ReflectionDatas.RefIAnimator = RefHelp.PropertyRef<Player, object>.Create("ArmsAnimatorCommon");
            ReflectionDatas.RefAnimator = RefHelp.PropertyRef<object, Animator>.Create(RefHelp.GetEftType(x => x.GetMethod("CreateAnimatorStateInfoWrapper", BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance) != null), "Animator");

            if (Is341Up)
            {
                Type launcherType = RefHelp.GetEftType(x => x.GetMethod("GetCenterOfImpact", BindingFlags.Public | BindingFlags.Instance) != null);

                ReflectionDatas.RefUnderbarrelWeapon = RefHelp.FieldRef<Player.FirearmController, Item>.Create("UnderbarrelWeapon");
                ReflectionDatas.RefChambers = RefHelp.FieldRef<object, Slot[]>.Create(launcherType, "Chambers");

                ReflectionDatas.RefWeaponTemplate = RefHelp.PropertyRef<object, WeaponTemplate>.Create(launcherType, "WeaponTemplate");
                ReflectionDatas.RefLauncherIAnimator = RefHelp.PropertyRef<Player, object>.Create("UnderbarrelWeaponArmsAnimator");
                ReflectionDatas.RefChamberAmmoCount = RefHelp.PropertyRef<object, int>.Create(launcherType, "ChamberAmmoCount");
            }

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            GamePanelHUDCorePlugin.HUDCoreClass.LoadHUD("gamepanlmaghud.bundle", "gamepanlmaghud");
        }

        public void IUpdate()
        {
            MagPlugin();
        }

        void MagPlugin()
        {
            MagHUDSW = HUDCore.AllHUDSW && NowWeapon != null && HUDCore.HasPlayer && SettingsDatas.KeyMagHUDSW.Value;

            HUD.Set(WeaponDatas, SettingsDatas, MagHUDSW);

            //Get Player
            if (HUDCore.HasPlayer)
            {
                NowFirearmController = HUDCore.YourPlayer.HandsController as Player.FirearmController;

                //Get Weapon Class
                NowWeapon = NowFirearmController != null ? NowFirearmController.Item : null;
                Animator_Weapon = ReflectionDatas.RefAnimator.GetValue(ReflectionDatas.RefIAnimator.GetValue(HUDCore.YourPlayer));

                if (Is341Up)
                {
                    NowLauncher = ReflectionDatas.RefUnderbarrelWeapon.GetValue(NowFirearmController);
                    Animator_Launcher = ReflectionDatas.RefAnimator.GetValue(ReflectionDatas.RefLauncherIAnimator.GetValue(HUDCore.YourPlayer));
                }

                bool weaponActive = NowWeapon != null;

                bool launcherActive = weaponActive ? NowFirearmController.IsInLauncherMode() : false;

                if (WeaponCacheBool)
                {
                    OldWeapon = NowWeapon;

                    if (!SettingsDatas.KeyWeaponNameAlways.Value && SettingsDatas.KeyAutoWeaponName.Value)
                    {
                        WeaponTirgger();
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
                    LauncherCacheBool = launcherActive;

                    if (!SettingsDatas.KeyWeaponNameAlways.Value && SettingsDatas.KeyAutoWeaponName.Value)
                    {
                        WeaponTirgger();
                    }
                }
                else if (!launcherActive)
                {
                    LauncherCacheBool = false;
                }

                //Get Ammon Count
                if (weaponActive)
                {
                    Animator currentAnimator = launcherActive ? Animator_Launcher : Animator_Weapon;

                    int currentState = currentAnimator.GetCurrentAnimatorStateInfo(1).fullPathHash;

                    AllReloadBool = NowFirearmController.IsInReloadOperation() || currentState == 1058993437 || currentState == 1285477936; //1.OriginalReloadCheck 2.TakeHands 3.LauncherReload

                    WeaponDatas.WeaponNameAlways = SettingsDatas.KeyWeaponNameAlways.Value || currentState == 1355507738 && SettingsDatas.KeyLockWeaponName.Value; //2.LookWeapon

                    //MagCount and PatronCount
                    if (!launcherActive)
                    {
                        //Get Weapon Name
                        WeaponDatas.WeaponName = LocalizedHelp.Localized(NowWeapon.Name, EStringCase.None);

                        WeaponDatas.WeaponShortName = LocalizedHelp.Localized(NowWeapon.ShortName, EStringCase.None);

                        //Get Fire Mode
                        WeaponDatas.FireMode = LocalizedHelp.Localized(NowWeapon.SelectedFireMode.ToString(), EStringCase.None);

                        WeaponDatas.AmmonType = LocalizedHelp.Localized(NowWeapon.AmmoCaliber, EStringCase.None);

                        if (NowWeapon.ReloadMode != Weapon.EReloadMode.OnlyBarrel)
                        {
                            bool magInWeapon = Animator_Weapon.GetBool(AnimatorHash.MagInWeapon);
                            bool magSame = NowMag == OldMag;

                            NowMag = GetMag.GetCurrentMagazine(NowWeapon);

                            if (MagCacheBool)
                            {
                                OldMag = NowMag;

                                MagCacheBool = false;
                            }

                            if (magInWeapon && !AllReloadBool && magSame)
                            {
                                int count = NowWeapon.GetCurrentMagazineCount();
                                int maxCount = NowWeapon.GetMaxMagazineCount();

                                WeaponDatas.Patron = NowWeapon.ChamberAmmoCount;

                                WeaponDatas.Normalized = (float)count / (float)maxCount;

                                WeaponDatas.MagCount = count;

                                WeaponDatas.MagMaxCount = maxCount;
                            }
                            else if (!magInWeapon && AllReloadBool)
                            {
                                WeaponDatas.Patron = (int)Animator_Weapon.GetFloat(AnimatorHash.AmmoInChamber);

                                WeaponDatas.Normalized = 0;

                                WeaponDatas.MagCount = 0;

                                WeaponDatas.MagMaxCount = 0;

                                OldMag = NowMag;
                            }
                            else if (magInWeapon && AllReloadBool && magSame)
                            {
                                int count = (int)Animator_Weapon.GetFloat(AnimatorHash.AmmoInMag);
                                int maxCount = NowWeapon.GetMaxMagazineCount();

                                WeaponDatas.Patron = (int)Animator_Weapon.GetFloat(AnimatorHash.AmmoInChamber);

                                WeaponDatas.Normalized = (float)count / (float)maxCount;

                                WeaponDatas.MagCount = count;

                                WeaponDatas.MagMaxCount = maxCount;
                            }
                            else if (!magInWeapon && Animator_Weapon.GetFloat(AnimatorHash.AmmoInChamber) != 0 && !AllReloadBool)
                            {
                                WeaponDatas.Patron = NowWeapon.ChamberAmmoCount;

                                WeaponDatas.Normalized = 0;

                                WeaponDatas.MagCount = 0;

                                WeaponDatas.MagMaxCount = 0;
                            }
                            else if (!magInWeapon && !AllReloadBool)
                            {
                                WeaponDatas.Patron = 0;

                                WeaponDatas.Normalized = 0;

                                WeaponDatas.MagCount = 0;

                                WeaponDatas.MagMaxCount = 0;
                            }
                        }
                        else
                        {
                            if (!AllReloadBool && Animator_Weapon.GetFloat(AnimatorHash.AmmoInChamber) != 0)
                            {
                                int count = NowWeapon.ChamberAmmoCount;
                                int maxCount = NowWeapon.Chambers.Length;

                                WeaponDatas.Patron = 0;

                                WeaponDatas.MagCount = count;

                                WeaponDatas.MagMaxCount = maxCount;

                                WeaponDatas.Normalized = (float)count / (float)maxCount - 0.1f;
                            }
                            else if (AllReloadBool && Animator_Weapon.GetBool("Armed"))
                            {
                                int count = (int)Animator_Weapon.GetFloat(AnimatorHash.AmmoInChamber);
                                int maxCount = NowWeapon.Chambers.Length;

                                WeaponDatas.Patron = 0;

                                WeaponDatas.MagCount = count;

                                WeaponDatas.MagMaxCount = maxCount;

                                WeaponDatas.Normalized = (float)count / (float)maxCount - 0.1f;
                            }
                            else if (!AllReloadBool)
                            {
                                WeaponDatas.Patron = 0;

                                WeaponDatas.MagCount = 0;

                                WeaponDatas.MagMaxCount = NowWeapon.Chambers.Length;

                                WeaponDatas.Normalized = 0;
                            }
                        }
                    }
                    else
                    {
                        //Get Weapon Name
                        WeaponDatas.WeaponName = LocalizedHelp.Localized(((Item)NowLauncher).Name, EStringCase.None);

                        WeaponDatas.WeaponShortName = LocalizedHelp.Localized(((Item)NowLauncher).ShortName, EStringCase.None);

                        WeaponTemplate launcherTemplate = ReflectionDatas.RefWeaponTemplate.GetValue(NowLauncher);

                        //Get Fire Mode
                        WeaponDatas.FireMode = LocalizedHelp.Localized(Weapon.EFireMode.single.ToString(), EStringCase.None);

                        WeaponDatas.AmmonType = LocalizedHelp.Localized(launcherTemplate.ammoCaliber, EStringCase.None);

                        if (!AllReloadBool && Animator_Launcher.GetFloat(AnimatorHash.AmmoInChamber) != 0)
                        {
                            int count = ReflectionDatas.RefChamberAmmoCount.GetValue(NowLauncher);
                            int maxCount = ReflectionDatas.RefChambers.GetValue(NowLauncher).Length;

                            WeaponDatas.Patron = 0;

                            WeaponDatas.MagCount = count;

                            WeaponDatas.MagMaxCount = maxCount;

                            WeaponDatas.Normalized = (float)count / (float)maxCount - 0.1f;
                        }
                        else if (AllReloadBool && Animator_Launcher.GetBool("Armed"))
                        {
                            int count = (int)Animator_Launcher.GetFloat(AnimatorHash.AmmoInChamber);
                            int maxCount = ReflectionDatas.RefChambers.GetValue(NowLauncher).Length;

                            WeaponDatas.Patron = 0;

                            WeaponDatas.MagCount = count;

                            WeaponDatas.MagMaxCount = maxCount;

                            WeaponDatas.Normalized = (float)count / (float)maxCount - 0.1f;
                        }
                        else if (!AllReloadBool)
                        {
                            WeaponDatas.Patron = 0;

                            WeaponDatas.MagCount = 0;

                            WeaponDatas.MagMaxCount = ReflectionDatas.RefChambers.GetValue(NowLauncher).Length;

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
            public RefHelp.FieldRef<Player.FirearmController, Item> RefUnderbarrelWeapon;
            public RefHelp.FieldRef<object, Slot[]> RefChambers;

            public RefHelp.PropertyRef<object, WeaponTemplate> RefWeaponTemplate;
            public RefHelp.PropertyRef<object, int> RefChamberAmmoCount;
            public RefHelp.PropertyRef<Player, object> RefIAnimator;
            public RefHelp.PropertyRef<Player, object> RefLauncherIAnimator;
            public RefHelp.PropertyRef<object, Animator> RefAnimator;
        }
    }
}
#endif
