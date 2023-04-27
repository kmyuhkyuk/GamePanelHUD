#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EFT.InventoryLogic;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDGrenade
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDGrenade", "kmyuhkyuk-GamePanelHUDGrenade", "2.6.1")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class GamePanelHUDGrenadePlugin : BaseUnityPlugin, IUpdate
    {
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore
        {
            get
            {
                return GamePanelHUDCorePlugin.HUDCore;
            }
        }

        internal static readonly GamePanelHUDCorePlugin.HUDClass<GrenadeAmount, SettingsData> HUD = new GamePanelHUDCorePlugin.HUDClass<GrenadeAmount, SettingsData>();

        private readonly ReflectionData ReflectionDatas = new ReflectionData();

        private bool GrenadeHUDSW;

        private object Equipment;

        private Item Rig;
        private Item Pocket;

        private readonly GrenadeAmount RigAmount = new GrenadeAmount();

        private readonly GrenadeAmount PocketAmount = new GrenadeAmount();

        private readonly GrenadeAmount AllAmount = new GrenadeAmount();

        private readonly SettingsData SettingsDatas = new SettingsData();

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDGrenade");

            ModUpdateCheck.DrawNeedUpdate(Config, Info.Metadata.Version);

            const string mainSettings = "主设置 Main Settings";
            const string positionScaleSettings = "位置大小设置 Position Scale Settings";
            const string colorSettings = "颜色设置 Color Settings";
            const string fontStylesSettings = "字体样式设置 Font Styles Settings";

            SettingsDatas.KeyGrenadeHUDSW = Config.Bind<bool>(mainSettings, "手雷计数器显示 Grenade HUD display", true);
            SettingsDatas.KeyMergeGrenade = Config.Bind<bool>(mainSettings, "合并所有手雷统计 Merge All Grenade Count", false);
            SettingsDatas.KeyZeroWarning = Config.Bind<bool>(mainSettings, "手雷数量为零警告 Grenade Count Zero Warning", false);

            SettingsDatas.KeyAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "指示栏位置 Anchored Position", new Vector2(-75, 5));
            SettingsDatas.KeySizeDelta = Config.Bind<Vector2>(positionScaleSettings, "指示栏高度 Size Delta", new Vector2(180, 30));
            SettingsDatas.KeyLocalScale = Config.Bind<Vector2>(positionScaleSettings, "指示栏大小 Local Scale", new Vector2(1, 1));

            SettingsDatas.KeyFragColor = Config.Bind<Color>(colorSettings, "碎片 Frag", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SettingsDatas.KeyStunColor = Config.Bind<Color>(colorSettings, "闪光 Stun", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SettingsDatas.KeySmokeColor = Config.Bind<Color>(colorSettings, "烟雾 Smoke", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SettingsDatas.KeyWarningColor = Config.Bind<Color>(colorSettings, "警告 Warning", new Color(0.7294118f,0f,0f)); //#BA0000

            SettingsDatas.KeyFragStyles = Config.Bind<FontStyles>(fontStylesSettings, "碎片 Frag", FontStyles.Normal);
            SettingsDatas.KeyStunStyles = Config.Bind<FontStyles>(fontStylesSettings, "闪光 Stun", FontStyles.Normal);
            SettingsDatas.KeySmokeStyles = Config.Bind<FontStyles>(fontStylesSettings, "烟雾 Smoke", FontStyles.Normal);

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            ReflectionDatas.RefEquipment = RefHelp.FieldRef<InventoryClass, object>.Create("Equipment");
            ReflectionDatas.RefSlots = RefHelp.FieldRef<object, Slot[]>.Create(ReflectionDatas.RefEquipment.FieldType, "Slots");
            ReflectionDatas.RefGrids = RefHelp.FieldRef<object, object[]>.Create(RefHelp.GetEftType(x => x.GetMethod("TryGetLastForbiddenItem", BindingFlags.DeclaredOnly | flags) != null), "Grids");

            ReflectionDatas.RefItems = RefHelp.PropertyRef<object, IEnumerable<Item>>.Create(ReflectionDatas.RefGrids.FieldType.GetElementType(), "Items");
            ReflectionDatas.RefThrowType = RefHelp.PropertyRef<object, ThrowWeapType>.Create(RefHelp.GetEftType(x => x.GetProperty("ThrowType", flags) != null), "ThrowType");

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            GamePanelHUDCorePlugin.HUDCoreClass.LoadHUD("gamepanlgrenadehud.bundle", "GamePanlGrenadeHUD");
        }

        public void IUpdate()
        {
            GrenadePlugin();
        }

        void GrenadePlugin()
        {
            GrenadeHUDSW = HUDCore.AllHUDSW && HUDCore.HasPlayer && SettingsDatas.KeyGrenadeHUDSW.Value;

            HUD.Set(AllAmount, SettingsDatas, GrenadeHUDSW);

            if (HUDCore.HasPlayer)
            {
                //Performance Optimization
                if (Time.frameCount % 20 == 0)
                {
                    Slot[] slots = ReflectionDatas.RefSlots.GetValue(ReflectionDatas.RefEquipment.GetValue(HUDCore.YourPlayer.Profile.Inventory));

                    //Get Rig and Pocket
                    Rig = slots[6].ContainedItem;
                    Pocket = slots[10].ContainedItem;

                    GetGrenadeAmount(Rig, out RigAmount.Frag, out RigAmount.Stun, out RigAmount.Flash, out RigAmount.Smoke);
                    GetGrenadeAmount(Pocket, out PocketAmount.Frag, out PocketAmount.Stun, out PocketAmount.Frag, out PocketAmount.Smoke);
                }

                AllAmount.Frag = RigAmount.Frag + PocketAmount.Frag;
                AllAmount.Stun = RigAmount.Stun + PocketAmount.Stun;
                AllAmount.Flash = RigAmount.Flash + PocketAmount.Flash;
                AllAmount.Smoke = RigAmount.Smoke + PocketAmount.Smoke;
            }
        }

        void GetGrenadeAmount(Item gear, out int frag, out int stun, out int flash, out int smoke)
        {
            frag = 0;
            stun = 0;
            flash = 0;
            smoke = 0;

            if (gear != null)
            {
                object[] grids = ReflectionDatas.RefGrids.GetValue(gear);

                if (!SettingsDatas.KeyMergeGrenade.Value)
                {
                    foreach (object grid in grids)
                    {
                        foreach (Item item in ReflectionDatas.RefItems.GetValue(grid))
                        {
                            if (item.GetType() == GrenadeType.GrenadeItemType)
                            {
                                switch (ReflectionDatas.RefThrowType.GetValue(item))
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
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (object grid in grids)
                    {
                        foreach (Item item in ReflectionDatas.RefItems.GetValue(grid))
                        {
                            frag++;
                        }
                    }
                }
            }
        }

        public class GrenadeAmount
        {
            public int Frag;
            public int Stun;
            public int Flash;
            public int Smoke;
        }

        public class SettingsData
        {
            public ConfigEntry<bool> KeyGrenadeHUDSW;
            public ConfigEntry<bool> KeyMergeGrenade;
            public ConfigEntry<bool> KeyZeroWarning;

            public ConfigEntry<Vector2> KeyAnchoredPosition;
            public ConfigEntry<Vector2> KeySizeDelta;
            public ConfigEntry<Vector2> KeyLocalScale;

            public ConfigEntry<Color> KeyFragColor;
            public ConfigEntry<Color> KeyStunColor;
            public ConfigEntry<Color> KeySmokeColor;
            public ConfigEntry<Color> KeyWarningColor;

            public ConfigEntry<FontStyles> KeyFragStyles;
            public ConfigEntry<FontStyles> KeyStunStyles;
            public ConfigEntry<FontStyles> KeySmokeStyles;
        }

        public class ReflectionData
        {
            public RefHelp.FieldRef<object, Slot[]> RefSlots;
            public RefHelp.FieldRef<object, object[]> RefGrids;
            public RefHelp.FieldRef<InventoryClass, object> RefEquipment;

            public RefHelp.PropertyRef<object, IEnumerable<Item>> RefItems;
            public RefHelp.PropertyRef<object, ThrowWeapType> RefThrowType;
        }
    }
}
#endif
