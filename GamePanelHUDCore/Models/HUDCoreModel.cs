#if !UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EFT;
using EFT.UI;
using GamePanelHUDCore.Controllers;
using KmyTarkovApi;
using KmyTarkovUtils;
using UnityEngine;
using UnityEngine.UI;
using static KmyTarkovApi.EFTHelpers;
using Object = UnityEngine.Object;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeMadeStatic.Global

namespace GamePanelHUDCore.Models
{
    public class HUDCoreModel
    {
        public static HUDCoreModel Instance { get; private set; }

        public Player YourPlayer => EFTGlobal.Player;

        public EftBattleUIScreen YourEftBattleUIScreen => EFTGlobal.EftBattleUIScreen;

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

        public readonly GameObject GamePanelHUDPublic =
            new GameObject("GamePanelHUDPublic", typeof(Canvas), typeof(CanvasScaler));

        public readonly UpdateManger UpdateManger = new UpdateManger();

        public readonly string ModPath = Path.Combine(BepInEx.Paths.PluginPath, "kmyuhkyuk-GamePanelHUD");

        private HUDCoreModel()
        {
            var canvas = GamePanelHUDPublic.GetComponent<Canvas>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1;
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
                                              AdditionalCanvasShaderChannels.Normal |
                                              AdditionalCanvasShaderChannels.Tangent;

            GamePanelHUDPublic.AddComponent<HUDCoreController>();

            Object.DontDestroyOnLoad(GamePanelHUDPublic);
        }

        // ReSharper disable once UnusedMethodReturnValue.Global
        public static HUDCoreModel Create()
        {
            if (Instance != null)
                return Instance;

            return Instance = new HUDCoreModel();
        }

        public string GetBundlePath(string bundleName)
        {
            return Path.Combine(ModPath, "bundles", bundleName);
        }

        public AssetModel<GameObject> LoadHUD(string bundleName, string initAssetName)
        {
            return LoadHUD(bundleName, new[] { initAssetName });
        }

        public AssetModel<GameObject> LoadHUD(string bundleName, IEnumerable<string> initAssetNames)
        {
            var assetBundle = AssetBundleHelper.LoadBundle(GetBundlePath(bundleName));

            var assetDictionary = assetBundle.LoadAllAssets<GameObject>().ToDictionary(x => x.name, x => x);

            var initDictionary = initAssetNames.ToDictionary(initAssetName => initAssetName,
                initAssetName => Object.Instantiate(assetDictionary[initAssetName], GamePanelHUDPublic.transform));

            assetBundle.Unload(false);

            return new AssetModel<GameObject>(assetDictionary, initDictionary);
        }
    }
}

#endif