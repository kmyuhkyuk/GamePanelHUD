#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using EFT;
using EFT.UI;
using EFTApi;
using EFTUtils;
using GamePanelHUDCore.Attributes;
using GamePanelHUDCore.Utils;
using UnityEngine;
using UnityEngine.UI;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDCore
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCore", "kmyuhkyuk-GamePanelHUDCore", "2.7.4")]
    [BepInDependency("com.kmyuhkyuk.EFTApi", "1.1.5")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/652-game-panel-hud", "../localized/core")]
    public class GamePanelHUDCorePlugin : BaseUnityPlugin
    {
        public static readonly HUDCoreClass HUDCore = new HUDCoreClass();

        private bool _allHUDSw;

        private readonly SettingsData _setData;

        public GamePanelHUDCorePlugin()
        {
            _setData = new SettingsData(Config);
        }

        private void Update()
        {
            //All HUD always display 
            if (_setData.KeyAllHUDAlways.Value)
            {
                _allHUDSw = true;
            }
            //All HUD display 
            else if (HUDCore.YourGameUI != null && HUDCore.YourGameUI.BattleUiScreen != null)
            {
                _allHUDSw = HUDCore.YourGameUI.BattleUiScreen.gameObject.activeSelf;
            }
            else
            {
                _allHUDSw = false;
            }

            HUDCore.Set(_allHUDSw);

            HUDCore.UpdateManger.OutputMethodTime = _setData.KeyDebugMethodTime.Value;

            HUDCore.UpdateManger.Update();
        }

        public class HUDCoreClass
        {
            public Player YourPlayer => EFTGlobal.Player;

            public GameUI YourGameUI => EFTGlobal.GameUI;

            public GameWorld TheWorld => EFTGlobal.GameWorld;

            public event Action<GameWorld> WorldStart
            {
                add => _GameWorldHelper.OnGameStarted.Add(value);
                remove => _GameWorldHelper.OnGameStarted.Remove(value);
            }

            public event Action<GameWorld> WorldDispose
            {
                add => _GameWorldHelper.Dispose.Add(value);
                remove => _GameWorldHelper.Dispose.Remove(value);
            }

            public bool HasPlayer => YourPlayer != null;

            public bool AllHUDSw;

            public readonly UpdateManger UpdateManger = new UpdateManger();

            public readonly GameObject GamePanelHUDPublic =
                new GameObject("GamePanelHUDPublic", typeof(Canvas), typeof(CanvasScaler));

            public readonly string ModPath = Path.Combine(BepInEx.Paths.PluginPath, "kmyuhkyuk-GamePanelHUD");

            public HUDCoreClass()
            {
                var canvas = GamePanelHUDPublic.GetComponent<Canvas>();

                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 1;
                canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
                                                  AdditionalCanvasShaderChannels.Normal |
                                                  AdditionalCanvasShaderChannels.Tangent;

                DontDestroyOnLoad(GamePanelHUDPublic);
            }

            public string GetBundlePath(string bundleName)
            {
                return Path.Combine(ModPath, "bundles", bundleName);
            }

            public AssetData<GameObject> LoadHUD(string bundleName, string initAssetName)
            {
                return LoadHUD(bundleName, new[] { initAssetName });
            }

            public AssetData<GameObject> LoadHUD(string bundleName, string[] initAssetName)
            {
                var assetBundle = AssetBundleHelper.LoadBundle(GetBundlePath(bundleName));

                var asset = assetBundle.LoadAllAssets<GameObject>().ToDictionary(x => x.name, x => x);

                var init = new Dictionary<string, GameObject>();

                foreach (var name in initAssetName)
                {
                    InitAsset(asset, init, name);
                }

                assetBundle.Unload(false);

                return new AssetData<GameObject>(asset, init);
            }

            private void InitAsset(Dictionary<string, GameObject> asset, Dictionary<string, GameObject> init,
                string initAssetName)
            {
                init.Add(initAssetName, Instantiate(asset[initAssetName], GamePanelHUDPublic.transform));
            }

            public class AssetData<T>
            {
                public readonly IReadOnlyDictionary<string, T> Asset;

                public readonly IReadOnlyDictionary<string, T> Init;

                public AssetData(Dictionary<string, T> asset, Dictionary<string, T> init)
                {
                    Asset = asset;
                    Init = init;
                }
            }

            public void Set(bool allHUDSw)
            {
                AllHUDSw = allHUDSw;
            }
        }

        public class HUDClass<T, TV>
        {
            public T Info;

            public TV SetData;

            public bool HUDSw;

            public void Set(T info, TV setData, bool hudSw)
            {
                Info = info;
                SetData = setData;
                HUDSw = hudSw;
            }
        }

        public class SettingsData
        {
            public readonly ConfigEntry<bool> KeyAllHUDAlways;
            public readonly ConfigEntry<bool> KeyDebugMethodTime;

            public SettingsData(ConfigFile configFile)
            {
                const string mainSettings = "Main Settings";

                KeyAllHUDAlways = configFile.Bind<bool>(mainSettings, "All HUD Always display", false);
                KeyDebugMethodTime = configFile.Bind<bool>(mainSettings,
                    "Debug All HUD Method Invoke Time", false,
                    new ConfigDescription(string.Empty, null,
                        new ConfigurationManagerAttributes { IsAdvanced = true }));
            }
        }
    }
}
#endif