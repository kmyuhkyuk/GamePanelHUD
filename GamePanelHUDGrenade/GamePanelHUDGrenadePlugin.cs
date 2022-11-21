#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using EFT;
using EFT.InventoryLogic;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDGrenade
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDGrenade", "kmyuhkyuk-GamePanelHUDGrenade", "2.3.1")]
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

        private object Inventory;
        private Item[] ContainedItems;

        private object Rig;
        private object Pocket;

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

            ReflectionDatas.RefInventory = RefHelp.FieldRef<Player, object>.Create("_inventoryController");
            ReflectionDatas.RefContainedItems = RefHelp.PropertyRef<object, Item[]>.Create(ReflectionDatas.RefInventory.FieldType, "ContainedItems");

            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            ReflectionDatas.RefGrids = RefHelp.FieldRef<object, object[]>.Create(RefHelp.GetEftType(x => x.GetMethod("TryGetLastForbiddenItem", BindingFlags.DeclaredOnly | flags) != null), "Grids");
            ReflectionDatas.RefItems = RefHelp.PropertyRef<object, IEnumerable<Item>>.Create(ReflectionDatas.RefGrids.FieldType.GetElementType(), "Items");
            ReflectionDatas.RefThrowType = RefHelp.PropertyRef<object, ThrowWeapType>.Create(RefHelp.GetEftType(x => x.GetProperty("ThrowType", flags) != null), "ThrowType");

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            HUDCore.LoadHUD("gamepanlgrenadehud.bundle", "gamepanlgrenadehud");
        }

        public void IUpdate()
        {
            GrenadePlugin();
        }

        void GrenadePlugin()
        {
            GrenadeHUDSW = HUDCore.AllHUDSW && Inventory != null && HUDCore.HasPlayer && SettingsDatas.KeyGrenadeHUDSW.Value;

            HUD.Set(AllAmount, SettingsDatas, GrenadeHUDSW);

            if (HUDCore.HasPlayer)
            {
                //Get Inventory and ContainedItems
                Inventory = ReflectionDatas.RefInventory.GetValue(HUDCore.IsYourPlayer);
                ContainedItems = ReflectionDatas.RefContainedItems.GetValue(Inventory);

                //Get Rig and Pocket
                Rig = ContainedItems[6];
                Pocket = ContainedItems[10];

                GetGrenadeAmount(Rig, RigAmount);
                GetGrenadeAmount(Pocket, PocketAmount);

                AllAmount.Frag = RigAmount.Frag + PocketAmount.Frag;
                AllAmount.Stun = RigAmount.Stun + PocketAmount.Stun;
                AllAmount.Flash = RigAmount.Flash + PocketAmount.Flash;
                AllAmount.Smoke = RigAmount.Smoke + PocketAmount.Smoke;
            }
        }

        void GetGrenadeAmount(object gear, GrenadeAmount grenadeamount)
        {
            if (gear != null)
            {
                object[] grids = ReflectionDatas.RefGrids.GetValue(gear);

                IEnumerable<Item> items = grids.SelectMany(x => ReflectionDatas.RefItems.GetValue(x));

                if (!SettingsDatas.KeyMergeGrenade.Value)
                {
                    grenadeamount.Clear();
                    foreach (Item item in items)
                    {
                        if (item.GetType() == GrenadeType.GrenadeItemType)
                        {
                            switch (ReflectionDatas.RefThrowType.GetValue(item))
                            {
                                case ThrowWeapType.frag_grenade:
                                    grenadeamount.Frag++;
                                    break;
                                case ThrowWeapType.stun_grenade:
                                    grenadeamount.Stun++;
                                    break;
                                case ThrowWeapType.flash_grenade:
                                    grenadeamount.Flash++;
                                    break;
                                case ThrowWeapType.smoke_grenade:
                                    grenadeamount.Smoke++;
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    grenadeamount.MergeClear();
                    grenadeamount.Frag = items.Where(x => x.GetType() == GrenadeType.GrenadeItemType).Count();
                }
            }
            else
            {
                grenadeamount.Clear();
            }
        }

        public class GrenadeAmount
        {
            public int Frag;
            public int Stun;
            public int Flash;
            public int Smoke;

            public void Clear()
            {
                Frag = 0;
                MergeClear();
            }

            public void MergeClear()
            {
                Stun = 0;
                Flash = 0;
                Smoke = 0;
            }
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
            public RefHelp.FieldRef<Player, object> RefInventory;
            public RefHelp.FieldRef<object, object[]> RefGrids;

            public RefHelp.PropertyRef<object, Item[]> RefContainedItems;
            public RefHelp.PropertyRef<object, IEnumerable<Item>> RefItems;
            public RefHelp.PropertyRef<object, ThrowWeapType> RefThrowType;
        }
    }
}
#endif
