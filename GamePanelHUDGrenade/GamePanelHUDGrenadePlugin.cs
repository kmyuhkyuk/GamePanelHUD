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
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDGrenade", "kmyuhkyuk-GamePanelHUDGrenade", "2.6.4")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class GamePanelHUDGrenadePlugin : BaseUnityPlugin, IUpdate
    {
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

        internal static readonly GamePanelHUDCorePlugin.HUDClass<GrenadeAmount, SettingsData> HUD = new GamePanelHUDCorePlugin.HUDClass<GrenadeAmount, SettingsData>();

        private readonly ReflectionData RefData = new ReflectionData();

        private bool GrenadeHUDSw;

        private Item Rig;
        private Item Pocket;

        private readonly GrenadeAmount RigAmount = new GrenadeAmount();

        private readonly GrenadeAmount PocketAmount = new GrenadeAmount();

        private readonly GrenadeAmount AllAmount = new GrenadeAmount();

        private readonly SettingsData SetData = new SettingsData();

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDGrenade");

            ModUpdateCheck.DrawCheck(this);

            const string mainSettings = "主设置 Main Settings";
            const string positionScaleSettings = "位置大小设置 Position Scale Settings";
            const string colorSettings = "颜色设置 Color Settings";
            const string fontStylesSettings = "字体样式设置 Font Styles Settings";

            SetData.KeyGrenadeHUDSw = Config.Bind<bool>(mainSettings, "手雷计数器显示 Grenade HUD display", true);
            SetData.KeyMergeGrenade = Config.Bind<bool>(mainSettings, "合并所有手雷统计 Merge All Grenade Count", false);
            SetData.KeyZeroWarning = Config.Bind<bool>(mainSettings, "手雷数量为零警告 Grenade Count Zero Warning", false);

            SetData.KeyAnchoredPosition = Config.Bind<Vector2>(positionScaleSettings, "指示栏位置 Anchored Position", new Vector2(-75, 5));
            SetData.KeySizeDelta = Config.Bind<Vector2>(positionScaleSettings, "指示栏高度 Size Delta", new Vector2(180, 30));
            SetData.KeyLocalScale = Config.Bind<Vector2>(positionScaleSettings, "指示栏大小 Local Scale", new Vector2(1, 1));

            SetData.KeyFragColor = Config.Bind<Color>(colorSettings, "碎片 Frag", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SetData.KeyStunColor = Config.Bind<Color>(colorSettings, "闪光 Stun", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SetData.KeySmokeColor = Config.Bind<Color>(colorSettings, "烟雾 Smoke", new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
            SetData.KeyWarningColor = Config.Bind<Color>(colorSettings, "警告 Warning", new Color(0.7294118f,0f,0f)); //#BA0000

            SetData.KeyFragStyles = Config.Bind<FontStyles>(fontStylesSettings, "碎片 Frag", FontStyles.Normal);
            SetData.KeyStunStyles = Config.Bind<FontStyles>(fontStylesSettings, "闪光 Stun", FontStyles.Normal);
            SetData.KeySmokeStyles = Config.Bind<FontStyles>(fontStylesSettings, "烟雾 Smoke", FontStyles.Normal);

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            RefData.RefEquipment = RefHelp.FieldRef<InventoryClass, object>.Create("Equipment");
            RefData.RefSlots = RefHelp.FieldRef<object, Slot[]>.Create(RefData.RefEquipment.FieldType, "Slots");
            RefData.RefGrids = RefHelp.FieldRef<object, object[]>.Create(RefHelp.GetEftType(x => x.GetMethod("TryGetLastForbiddenItem", BindingFlags.DeclaredOnly | flags) != null), "Grids");

            RefData.RefItems = RefHelp.PropertyRef<object, IEnumerable<Item>>.Create(RefData.RefGrids.FieldType.GetElementType(), "Items");
            RefData.RefThrowType = RefHelp.PropertyRef<object, ThrowWeapType>.Create(RefHelp.GetEftType(x => x.GetProperty("ThrowType", flags) != null), "ThrowType");

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            GamePanelHUDCorePlugin.HUDCoreClass.LoadHUD("gamepanelgrenadehud.bundle", "GamePanelGrenadeHUD");
        }

        public void IUpdate()
        {
            GrenadePlugin();
        }

        void GrenadePlugin()
        {
            GrenadeHUDSw = HUDCore.AllHUDSw && HUDCore.HasPlayer && SetData.KeyGrenadeHUDSw.Value;

            HUD.Set(AllAmount, SetData, GrenadeHUDSw);

            if (HUDCore.HasPlayer)
            {
                //Performance Optimization
                if (Time.frameCount % 20 == 0)
                {
                    Slot[] slots = RefData.RefSlots.GetValue(RefData.RefEquipment.GetValue(HUDCore.YourPlayer.Profile.Inventory));

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
                object[] grids = RefData.RefGrids.GetValue(gear);

                if (!SetData.KeyMergeGrenade.Value)
                {
                    foreach (object grid in grids)
                    {
                        foreach (Item item in RefData.RefItems.GetValue(grid))
                        {
                            if (item.GetType() == GrenadeType.GrenadeItemType)
                            {
                                switch (RefData.RefThrowType.GetValue(item))
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
                        foreach (Item _ in RefData.RefItems.GetValue(grid))
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
            public ConfigEntry<bool> KeyGrenadeHUDSw;
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
