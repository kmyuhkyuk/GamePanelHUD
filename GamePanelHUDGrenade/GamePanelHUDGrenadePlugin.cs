#if !UNITY_EDITOR
using System;
using BepInEx;
using BepInEx.Configuration;
using EFT.InventoryLogic;
using EFTReflection;
using EFTUtils;
using GamePanelHUDCore;
using GamePanelHUDCore.Attributes;
using TMPro;
using UnityEngine;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDGrenade
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDGrenade", "kmyuhkyuk-GamePanelHUDGrenade", "2.7.5")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore", "2.7.5")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "localized/grenade")]
    public class GamePanelHUDGrenadePlugin : BaseUnityPlugin, IUpdate
    {
        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

        internal static readonly GamePanelHUDCorePlugin.HUDClass<GrenadeAmount, SettingsData> HUD =
            new GamePanelHUDCorePlugin.HUDClass<GrenadeAmount, SettingsData>();

        private readonly ReflectionData _reflectionData = new ReflectionData();

        private bool _grenadeHUDSw;

        private Item _rig;

        private Item _pocket;

        private readonly GrenadeAmount _rigAmount = new GrenadeAmount();

        private readonly GrenadeAmount _pocketAmount = new GrenadeAmount();

        private readonly GrenadeAmount _allAmount = new GrenadeAmount();

        private readonly SettingsData _setData;

        private readonly Type _grenadeItemType;

        public GamePanelHUDGrenadePlugin()
        {
            _grenadeItemType = RefTool.GetEftType(x =>
                x.GetMethod("CreateFragment", RefTool.Public) != null &&
                x.GetProperty("GetExplDelay", RefTool.Public) != null);

            _setData = new SettingsData(Config);
        }

        private void Start()
        {
            HUDCore.UpdateManger.Register(this);
        }

        private void Awake()
        {
            HUDCore.LoadHUD("gamepanelgrenadehud.bundle", "GamePanelGrenadeHUD");
        }

        public void CustomUpdate()
        {
            GrenadePlugin();
        }

        private void GrenadePlugin()
        {
            _grenadeHUDSw = HUDCore.AllHUDSw && HUDCore.HasPlayer && _setData.KeyGrenadeHUDSw.Value;

            HUD.Set(_allAmount, _setData, _grenadeHUDSw);

            if (HUDCore.HasPlayer)
            {
                //Performance Optimization
                if (Time.frameCount % 20 == 0)
                {
                    var slots = _PlayerHelper.InventoryHelper.EquipmentSlots;

                    //Get Rig and Pocket
                    _rig = slots[6].ContainedItem;
                    _pocket = slots[10].ContainedItem;

                    GetGrenadeAmount(_rig, out _rigAmount.Frag, out _rigAmount.Stun, out _rigAmount.Flash,
                        out _rigAmount.Smoke);
                    GetGrenadeAmount(_pocket, out _pocketAmount.Frag, out _pocketAmount.Stun, out _pocketAmount.Frag,
                        out _pocketAmount.Smoke);
                }

                _allAmount.Frag = _rigAmount.Frag + _pocketAmount.Frag;
                _allAmount.Stun = _rigAmount.Stun + _pocketAmount.Stun;
                _allAmount.Flash = _rigAmount.Flash + _pocketAmount.Flash;
                _allAmount.Smoke = _rigAmount.Smoke + _pocketAmount.Smoke;
            }
        }

        private void GetGrenadeAmount(Item gear, out int frag, out int stun, out int flash, out int smoke)
        {
            frag = 0;
            stun = 0;
            flash = 0;
            smoke = 0;

            if (gear == null)
                return;

            var grids = _PlayerHelper.InventoryHelper.RefGrids.GetValue(gear);

            if (!_setData.KeyMergeGrenade.Value)
            {
                foreach (var grid in grids)
                {
                    foreach (var item in _PlayerHelper.InventoryHelper.RefItems.GetValue(grid))
                    {
                        if (item.GetType() == _grenadeItemType)
                        {
                            switch (_reflectionData.ThrowType.GetValue(item))
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
                foreach (var grid in grids)
                {
                    foreach (var _ in _PlayerHelper.InventoryHelper.RefItems.GetValue(grid))
                    {
                        frag++;
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
            public readonly ConfigEntry<bool> KeyGrenadeHUDSw;
            public readonly ConfigEntry<bool> KeyMergeGrenade;
            public readonly ConfigEntry<bool> KeyZeroWarning;

            public readonly ConfigEntry<Vector2> KeyAnchoredPosition;
            public readonly ConfigEntry<Vector2> KeySizeDelta;
            public readonly ConfigEntry<Vector2> KeyLocalScale;

            public readonly ConfigEntry<Color> KeyFragColor;
            public readonly ConfigEntry<Color> KeyStunColor;
            public readonly ConfigEntry<Color> KeySmokeColor;
            public readonly ConfigEntry<Color> KeyWarningColor;

            public readonly ConfigEntry<FontStyles> KeyFragStyles;
            public readonly ConfigEntry<FontStyles> KeyStunStyles;
            public readonly ConfigEntry<FontStyles> KeySmokeStyles;

            public SettingsData(ConfigFile configFile)
            {
                const string mainSettings = "Main Settings";
                const string positionScaleSettings = "Position Scale Settings";
                const string colorSettings = "Color Settings";
                const string fontStylesSettings = "Font Styles Settings";

                KeyGrenadeHUDSw = configFile.Bind<bool>(mainSettings, "Grenade HUD display", true);
                KeyMergeGrenade = configFile.Bind<bool>(mainSettings, "Merge All Grenade Count", false);
                KeyZeroWarning = configFile.Bind<bool>(mainSettings, "Grenade Count Zero Warning", false);

                KeyAnchoredPosition =
                    configFile.Bind<Vector2>(positionScaleSettings, "Anchored Position", new Vector2(-75, 5));
                KeySizeDelta =
                    configFile.Bind<Vector2>(positionScaleSettings, "Size Delta", new Vector2(180, 30));
                KeyLocalScale =
                    configFile.Bind<Vector2>(positionScaleSettings, "Local Scale", new Vector2(1, 1));

                KeyFragColor =
                    configFile.Bind<Color>(colorSettings, "Frag",
                        new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
                KeyStunColor =
                    configFile.Bind<Color>(colorSettings, "Stun",
                        new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
                KeySmokeColor =
                    configFile.Bind<Color>(colorSettings, "Smoke",
                        new Color(0.8901961f, 0.8901961f, 0.8392157f)); //#E3E3D6
                KeyWarningColor =
                    configFile.Bind<Color>(colorSettings, "Warning", new Color(0.7294118f, 0f, 0f)); //#BA0000

                KeyFragStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Frag", FontStyles.Normal);
                KeyStunStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Stun", FontStyles.Normal);
                KeySmokeStyles = configFile.Bind<FontStyles>(fontStylesSettings, "Smoke", FontStyles.Normal);
            }
        }

        private class ReflectionData
        {
            public readonly RefHelper.PropertyRef<object, ThrowWeapType> ThrowType;

            public ReflectionData()
            {
                ThrowType = RefHelper.PropertyRef<object, ThrowWeapType>.Create(
                    RefTool.GetEftType(x => x.GetProperty("ThrowType", RefTool.Public) != null), "ThrowType");
            }
        }
    }
}
#endif