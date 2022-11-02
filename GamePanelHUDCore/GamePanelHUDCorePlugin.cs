﻿using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EFT;
using GamePanelHUDCore.Patches;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCore
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCore", "kmyuhkyuk-GamePanelHUDCore", "2.3.0")]
    public class GamePanelHUDCorePlugin : BaseUnityPlugin
    {
        public static readonly IUpdateManger UpdateManger = new IUpdateManger();

        internal static GameObject EnvironmentUIRoot;
        internal static GameObject InventoryScreen;
        internal static GameObject HideoutScreenOverlay;

        internal static Player IsYourPlayer;

        public static readonly HUDCoreClass HUDCore = new HUDCoreClass();

        private bool AllHUDSW;

        private readonly SettingsData SettingsDatas = new SettingsData();

        internal static ManualLogSource LogLogger { get; private set; }

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDCore");

            LogLogger = Logger;

            string mainSettings = "主设置 Main Settings";

            SettingsDatas.KeyAllHUDAlways = Config.Bind<bool>(mainSettings, "所有指示栏始终显示 All HUD Always display", false);
            SettingsDatas.KeyDebugMethodTime = Config.Bind<bool>(mainSettings, "调试所有指示栏调用时间 Debug All HUD Mehod Invoke Time", false, new ConfigDescription("", null, new ConfigurationManagerAttributes() { IsAdvanced = true }));

            Canvas canvs = HUDCore.GamePanlHUDPublic.GetComponent<Canvas>();

            canvs.renderMode = RenderMode.ScreenSpaceOverlay;
            canvs.sortingOrder = 1;
            canvs.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;

            DontDestroyOnLoad(HUDCore.GamePanlHUDPublic);

            new PlayerPatch().Enable();
            new EnvironmentUIRootPatch().Enable();
            new InventoryScreenPatch().Enable();
            new HideoutScreenOverlayPatch().Enable();
            new MainApplicationPatch().Enable();

            LocalizedHelp.Init();
            GrenadeType.Init();
            GetMag.Init();
            ReloadOperation.Init();
            RoleHelp.Init();
            RuToEn.Init();
        }

        void Update()
        {
            //All HUD always display 
            if (SettingsDatas.KeyAllHUDAlways.Value)
            {
                AllHUDSW = true;
            }
            //All HUD display 
            else if (EnvironmentUIRoot != null)
            {
                GameObject[] gameobjects = new GameObject[] { EnvironmentUIRoot, InventoryScreen, HideoutScreenOverlay };

                AllHUDSW = !gameobjects.Where(x => x != null).Select(x => x.activeSelf).Contains(true);
            }
            else
            {
                AllHUDSW = false;
            }

            HUDCore.Set(IsYourPlayer, AllHUDSW);

            UpdateManger.NeedMethodTime = SettingsDatas.KeyDebugMethodTime.Value;

            UpdateManger.Update();
        }

        public class HUDCoreClass
        {
            public Player IsYourPlayer;

            public bool HasPlayer
            {
                get
                {
                    return IsYourPlayer != null;
                }
            }

            public bool AllHUDSW;

            public readonly GameObject GamePanlHUDPublic = new GameObject("GamePanlHUDPublic", new Type[] { typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster) });

            public readonly string ModPath  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BepInEx/plugins/kmyuhkyuk-GamePanelHUD");

            public string GetBundlePath(string bundlename)
            {
                return Path.Combine(ModPath, "bundles", bundlename);
            }

            public BundleHelp.AssetData<GameObject> LoadHUD(string bundlename, string[] initassetname)
            {
                AssetBundle assetBundle = BundleHelp.LoadBundle(GetBundlePath(bundlename));

                Dictionary<string, GameObject> asset = BundleHelp.LoadAllAsset<GameObject>(assetBundle).ToDictionary(x => x.name.ToLower(), x => x);

                Dictionary<string, GameObject> init = new Dictionary<string, GameObject>();

                foreach (string name in initassetname)
                {
                    string initAssetName = name.ToLower();

                    GameObject initAsset;

                    asset.TryGetValue(initAssetName, out initAsset);

                    if (initAsset != null)
                    {
                        BundleHelp.InitAsset(initAsset, HUDCore.GamePanlHUDPublic.transform);
                    }
                }

                assetBundle.Unload(false);

                return new BundleHelp.AssetData<GameObject>(asset, init);
            }

            public BundleHelp.AssetData<GameObject> LoadHUD(string bundlename, string initassetname)
            {
                AssetBundle assetBundle = BundleHelp.LoadBundle(GetBundlePath(bundlename));

                Dictionary<string, GameObject> asset = BundleHelp.LoadAllAsset<GameObject>(assetBundle).ToDictionary(x => x.name.ToLower(), x => x);

                Dictionary<string, GameObject> init = new Dictionary<string, GameObject>();

                GameObject initAsset;

                asset.TryGetValue(initassetname.ToLower(), out initAsset);

                if (initAsset != null)
                {
                    BundleHelp.InitAsset(initAsset, HUDCore.GamePanlHUDPublic.transform);
                }

                assetBundle.Unload(false);

                return new BundleHelp.AssetData<GameObject>(asset, init);
            }

            public void Set(Player isyourplayer, bool hudsw)
            {
                IsYourPlayer = isyourplayer;
                AllHUDSW = hudsw;
            }
        }

        public class HUDClass<T, V>
        {
            public T Info;

            public V SettingsData;

            public bool HUDSW;

            public void Set(T info, V settingsdata, bool hudsw)
            {
                Info = info;
                SettingsData = settingsdata;
                HUDSW = hudsw;
            }
        }

        public class SettingsData
        {
            public ConfigEntry<bool> KeyAllHUDAlways;
            public ConfigEntry<bool> KeyDebugMethodTime;
        }
    }
}
