#if !UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EFT;
using EFT.UI;
using EFTApi;
using EFTUtils;
using GamePanelHUDCore.Controllers;
using UnityEngine;
using static EFTApi.EFTHelpers;
using Object = UnityEngine.Object;

namespace GamePanelHUDCore.Models
{
    public class HUDCoreModel
    {
        public static HUDCoreModel Instance { get; private set; }

        public Player YourPlayer => EFTGlobal.Player;

        public GameUI YourGameUI => EFTGlobal.GameUI;

        public GameWorld TheWorld => EFTGlobal.GameWorld;

        public AbstractGame TheGame => EFTGlobal.AbstractGame;

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

        public readonly GameObject GamePanelHUDPublic;

        public readonly UpdateManger UpdateManger;

        public readonly string ModPath = Path.Combine(BepInEx.Paths.PluginPath, "kmyuhkyuk-GamePanelHUD");

        private HUDCoreModel(GameObject gamePanelHUDPublic, UpdateManger updateManger)
        {
            GamePanelHUDPublic = gamePanelHUDPublic;
            UpdateManger = updateManger;

            var canvas = gamePanelHUDPublic.GetComponent<Canvas>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
                                              AdditionalCanvasShaderChannels.Normal |
                                              AdditionalCanvasShaderChannels.Tangent;

            gamePanelHUDPublic.AddComponent<HUDCoreController>();

            Object.DontDestroyOnLoad(gamePanelHUDPublic);
        }

        public static HUDCoreModel Create(GameObject gamePanelHUDPublic, UpdateManger updateManger)
        {
            if (Instance != null)
                return Instance;

            return Instance = new HUDCoreModel(gamePanelHUDPublic, updateManger);
        }

        public string GetBundlePath(string bundleName)
        {
            return Path.Combine(ModPath, "bundles", bundleName);
        }

        public AssetModel<GameObject> LoadHUD(string bundleName, string initAssetName)
        {
            return LoadHUD(bundleName, new[] { initAssetName });
        }

        public AssetModel<GameObject> LoadHUD(string bundleName, string[] initAssetName)
        {
            var assetBundle = AssetBundleHelper.LoadBundle(GetBundlePath(bundleName));

            var asset = assetBundle.LoadAllAssets<GameObject>().ToDictionary(x => x.name, x => x);

            var init = new Dictionary<string, GameObject>();

            foreach (var name in initAssetName)
            {
                InitAsset(asset, init, name);
            }

            assetBundle.Unload(false);

            return new AssetModel<GameObject>(asset, init);
        }

        private void InitAsset(Dictionary<string, GameObject> asset, Dictionary<string, GameObject> init,
            string initAssetName)
        {
            init.Add(initAssetName, Object.Instantiate(asset[initAssetName], GamePanelHUDPublic.transform));
        }
    }
}

#endif