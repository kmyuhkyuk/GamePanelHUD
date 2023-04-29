#if !UNITY_EDITOR
using BepInEx.Configuration;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using BepInEx;

namespace GamePanelHUDCore.Utils
{
    public static class ModUpdateCheck
    {
        private static readonly HUDVersion HUDVersions = new HUDVersion();

        private const string UpdateDataUrl = "https://dev.sp-tarkov.com/kmyuhkyuk/GamePanelHUD/raw/branch/master/Version.json";

        private const string Section = "主更新检查 Update Check";

        public static async void ServerCheckAndPull()
        {
            if (HUDVersions.Connecting)
                return;

            HUDVersions.Connecting = true;

            using (UnityWebRequest www = UnityWebRequest.Get(UpdateDataUrl))
            {
                www.timeout = 10;

                www.SendWebRequest();

                while (!www.isDone)
                    await Task.Yield();

                if (www.isHttpError || www.isNetworkError)
                {
                    HUDVersions.ConnectError();
                }
                else
                {
                    HUDVersions.Set(JsonConvert.DeserializeObject<HUDVersion>(www.downloadHandler.text));
                }
            }

            HUDVersions.Connecting = false;
        }

        public static void DrawCheck(BaseUnityPlugin plugin)
        {
            plugin.Config.Bind(Section, "Update", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1, HideDefaultButton = true, CustomDrawer = new Draw(plugin.Info.Metadata.Version).OnGui, HideSettingName = true }));
        }

        public class HUDVersion
        {
            [NonSerialized]
            public bool ServerConnect;

            [NonSerialized]
            public bool Connecting;

            public Version ModVersion;

            //First Support Game Version
            public Version FirstGameVersion;

            //Last Support Game Version
            public Version LastGameVersion;

            public string ModFileUrl;

            public string ModDownloadUrl;

            public void Set(HUDVersion hudversion)
            {
                ServerConnect = true;
                ModVersion = hudversion.ModVersion;
                FirstGameVersion = hudversion.FirstGameVersion;
                LastGameVersion = hudversion.LastGameVersion;
                ModFileUrl = hudversion.ModFileUrl;
                ModDownloadUrl = hudversion.ModDownloadUrl;
            }

            public void ConnectError()
            {
                ServerConnect = false;
                ModVersion = null;
                FirstGameVersion = null;
                LastGameVersion = null;
                ModFileUrl = null;
                ModDownloadUrl = null;
            }
        }

        public class Draw
        {
            private static readonly GUIStyle Red = new GUIStyle();

            private static readonly GUIStyle Yellow = new GUIStyle();

            private static readonly GUIStyle Green = new GUIStyle();

            public readonly Version ModVersion;

            static Draw()
            {
                Red.normal.textColor = Color.red;
                Red.fontStyle = FontStyle.Bold;

                Yellow.normal.textColor = Color.yellow;
                Yellow.fontStyle = FontStyle.Bold;

                Green.normal.textColor = Color.green;
                Green.fontStyle = FontStyle.Bold;
            }

            public Draw(Version version)
            {
                ModVersion = version;
            }

            public void OnGui(ConfigEntryBase entry)
            {
                if (!HUDVersions.Connecting)
                {
                    if (HUDVersions.ServerConnect)
                    {
                        if (HUDVersions.ModVersion > ModVersion)
                        {
                            ConnectNeedUpdate();
                        }
                        else
                        {
                            ConnectNotUpdate();
                        }

                        Version gameVersion = GamePanelHUDCorePlugin.HUDCoreClass.GameVersion;

                        if (HUDVersions.FirstGameVersion > gameVersion)
                        {
                            BelowSupport();
                        }
                        else if (HUDVersions.LastGameVersion < gameVersion)
                        {
                            HigherSupport();
                        }
                    }
                    else
                    {
                        ConnectFail();
                    }
                }
                else
                {
                    Connecting();
                }
            }

            private static void ConnectNeedUpdate()
            {
                GUILayout.BeginVertical();

                GUILayout.Label("有新版本 Have latest version", Yellow);

                GUILayout.Label(string.Concat("最新版本 Latest Version: ", HUDVersions.ModVersion), Yellow);

                GUILayout.EndVertical();

                GUILayout.BeginVertical();

                if (GUILayout.Button("Mod File", GUILayout.ExpandWidth(true)))
                {
                    Application.OpenURL(HUDVersions.ModFileUrl);
                }

                if (GUILayout.Button("Mod Download", GUILayout.ExpandWidth(true)))
                {
                    Application.OpenURL(HUDVersions.ModDownloadUrl);
                }

                if (GUILayout.Button("Retry", GUILayout.ExpandWidth(true)))
                {
                    ServerCheckAndPull();
                }

                GUILayout.EndVertical();
            }

            private static void ConnectNotUpdate()
            {
                GUILayout.Label("已经是最新版本 Already is latest version", Green);

                GUILayout.BeginVertical();

                if (GUILayout.Button("Mod File", GUILayout.ExpandWidth(true)))
                {
                    Application.OpenURL(HUDVersions.ModFileUrl);
                }

                if (GUILayout.Button("Retry", GUILayout.ExpandWidth(true)))
                {
                    ServerCheckAndPull();
                }

                GUILayout.EndVertical();
            }

            private static void Connecting()
            {
                GUILayout.Label("连接服务器中 Connecting Server", Yellow);
            }

            private static void ConnectFail()
            {
                GUILayout.Label("无法连接服务器 Can't Connect Server", Red);

                if (GUILayout.Button("Retry", GUILayout.ExpandWidth(true)))
                {
                    ServerCheckAndPull();
                }
            }

            private static void BelowSupport()
            {
                GUILayout.BeginVertical();

                GUILayout.Label("游戏版本低于模组支持版本, 它可能不会工作", Red);
                GUILayout.Label("Game Version Below Mod Support Version, It Maybe Can't Work", Red);

                GUILayout.EndVertical();
            }

            private static void HigherSupport()
            {
                GUILayout.BeginVertical();

                GUILayout.Label("游戏版本高于模组支持版本, 它可能会有些问题", Yellow);
                GUILayout.Label("Game Version Higher than Mod Support Version, It Maybe Will Be Problems", Yellow);

                GUILayout.EndVertical();
            }
        }
    }
}
#endif
