#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EFT;
using EFT.UI;
using GamePanelHUDCore.Patches;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCore
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDCore", "kmyuhkyuk-GamePanelHUDCore", "2.6.1")]
    public class GamePanelHUDCorePlugin : BaseUnityPlugin
    {
        public static readonly IUpdateManger UpdateManger = new IUpdateManger();

        internal static GameUI YourGameUI;

        internal static Player YourPlayer;

        internal static GameWorld TheWorld;

        public static readonly HUDCoreClass HUDCore = new HUDCoreClass();

        private bool AllHUDSW;

        private readonly SettingsData SettingsDatas = new SettingsData();

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDCore");

            ModUpdateCheck.ServerCheck();
            ModUpdateCheck.DrawNeedUpdate(Config, Info.Metadata.Version);

            const string mainSettings = "主设置 Main Settings";

            SettingsDatas.KeyAllHUDAlways = Config.Bind<bool>(mainSettings, "所有指示栏始终显示 All HUD Always display", false);
            SettingsDatas.KeyDebugMethodTime = Config.Bind<bool>(mainSettings, "调试所有指示栏调用时间 Debug All HUD Method Invoke Time", false, new ConfigDescription("", null, new ConfigurationManagerAttributes() { IsAdvanced = true }));

            new PlayerPatch().Enable();
            new GameWorldAwakePatch().Enable();
            new GameWorldOnGameStartedPatch().Enable();
            new GameWorldDisposePatch().Enable();
            new GameUIPatch().Enable();
            new MainApplicationPatch().Enable();
            new TriggerWithIdPatch().Enable();

            LocalizedHelp.Init();
            GrenadeType.Init();
            GetMag.Init();
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
            else if (YourGameUI != null && YourGameUI.BattleUiScreen != null)
            {
                AllHUDSW = YourGameUI.BattleUiScreen.gameObject.activeSelf;
            }
            else
            {
                AllHUDSW = false;
            }

            HUDCore.Set(YourPlayer, YourGameUI, TheWorld, AllHUDSW);

            UpdateManger.NeedMethodTime = SettingsDatas.KeyDebugMethodTime.Value;

            UpdateManger.Update();
        }

        public class HUDCoreClass
        {
            public Player YourPlayer;

            public GameUI YourGameUI;

            public GameWorld TheWorld;

            public static event Action<GameWorld> WorldStart;

            public static event Action<GameWorld> WorldDispose;

            public bool HasPlayer
            {
                get
                {
                    return YourPlayer != null;
                }
            }

            public bool AllHUDSW;

            public static readonly GameObject GamePanlHUDPublic = new GameObject("GamePanlHUDPublic", new Type[] { typeof(Canvas), typeof(CanvasScaler) });

            public static readonly string ModPath  = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BepInEx/plugins/kmyuhkyuk-GamePanelHUD");

            public static readonly Version GameVersion;

            private static readonly ManualLogSource LogSource = BepInEx.Logging.Logger.CreateLogSource("HUDCore");

            static HUDCoreClass()
            {
                FileVersionInfo exeInfo = Process.GetCurrentProcess().MainModule.FileVersionInfo;

                GameVersion = new Version(exeInfo.FileMajorPart, exeInfo.ProductMinorPart, exeInfo.ProductBuildPart, exeInfo.FilePrivatePart);

                Canvas canvas = GamePanlHUDPublic.GetComponent<Canvas>();

                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 1;
                canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;

                DontDestroyOnLoad(GamePanlHUDPublic);
            }

            public static string GetBundlePath(string bundlename)
            {
                return Path.Combine(ModPath, "bundles", bundlename);
            }

            public static AssetData<GameObject> LoadHUD(string bundlename, string initassetname)
            {
                return LoadHUD(bundlename, new string[] { initassetname });
            }

            public static AssetData<GameObject> LoadHUD(string bundlename, string[] initassetname)
            {
                AssetBundle assetBundle = BundleHelp.LoadBundle(LogSource, GetBundlePath(bundlename));

                Dictionary<string, GameObject> asset = assetBundle.LoadAllAssets<GameObject>().ToDictionary(x => x.name, x => x);

                Dictionary<string, GameObject> init = new Dictionary<string, GameObject>();

                foreach (string name in initassetname)
                {
                    InitAsset(asset, init, name);
                }

                assetBundle.Unload(false);

                return new AssetData<GameObject>(asset, init);
            }

            private static void InitAsset(Dictionary<string, GameObject> asset, Dictionary<string, GameObject> init, string initassetname)
            {
                init.Add(initassetname, GameObject.Instantiate(asset[initassetname], GamePanlHUDPublic.transform));
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

            public static void GameWorldDispose(GameWorld world)
            {
                if (WorldDispose != null)
                {
                    WorldDispose(world);
                }
            }

            public static void GameWorldStart(GameWorld world)
            {
                if (WorldStart != null)
                {
                    WorldStart(world);
                }
            }

            public void Set(Player yourplayer, GameUI yourgameui, GameWorld theworld, bool hudsw)
            {
                YourPlayer = yourplayer;
                YourGameUI = yourgameui;
                TheWorld = theworld;
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
#endif
