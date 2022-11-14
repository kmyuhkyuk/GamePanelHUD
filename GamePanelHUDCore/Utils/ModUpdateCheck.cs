using BepInEx.Configuration;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace GamePanelHUDCore.Utils
{
    public class ModUpdateCheck
    {
        private static readonly HUDVersion HUDVersions = new HUDVersion();

        private static readonly string UpdateDataUrl;

        public static async void ServerCheck()
        {
            UnityWebRequest www = UnityWebRequest.Get(UpdateDataUrl);

            www.SendWebRequest();

            while (!www.isDone)
                await Task.Yield();

            if (!www.isHttpError || !www.isNetworkError)
            {
                HUDVersion hudVersion = JsonConvert.DeserializeObject<HUDVersion>(www.downloadHandler.text);

                HUDVersions.Set(true, hudVersion.ModVersion, hudVersion.ModFileUrl, hudVersion.ModDownloadUrl);
            }
            else
            {
                HUDVersions.ServerConnect = false;
            }
        }

        public static async void DrawNeedUpdate(ConfigFile config, Version version)
        {
            Action<ConfigEntryBase> draw;

            while (!HUDVersions.ServerConnect.HasValue)
                await Task.Yield();

            if (!(bool)HUDVersions.ServerConnect)
            {
                draw = Draw.CantKnowUpdate;
            }
            else if (HUDVersions.ModVersion > version)
            {
                draw = Draw.NeedUpdate;
            }
            else
            {
                draw = Draw.NotNeedUpdate;
            }

            config.Bind("模组更新检查 Update Check", "Update", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1, HideDefaultButton = true, CustomDrawer = draw, HideSettingName = true }));
        }

        public class HUDVersion
        {
            public bool? ServerConnect;

            public Version ModVersion;

            public string ModFileUrl;

            public string ModDownloadUrl;

            public void Set(bool connect, Version version, string fileurl, string downloadurl)
            {
                ServerConnect = connect;
                ModVersion = version;
                ModFileUrl = fileurl;
                ModDownloadUrl = downloadurl;
            }
        }

        public class Draw
        {
            public static void NeedUpdate(ConfigEntryBase entry)
            {
                GUIStyle style = new GUIStyle();

                style.normal.textColor = Color.yellow;

                style.fontStyle = FontStyle.Bold;

                GUILayout.Label("有新版本 Has New Version", style);

                GUIStyle style2 = new GUIStyle();

                style2.normal.textColor = Color.yellow;

                GUILayout.Label(string.Concat("最新版本 Latest Version: ", HUDVersions.ModVersion), style2);

                if (GUILayout.Button("Mod File", GUILayout.ExpandWidth(true)))
                {
                    Application.OpenURL(HUDVersions.ModFileUrl);
                }

                if (GUILayout.Button("Mod Download", GUILayout.ExpandWidth(true)))
                {
                    Application.OpenURL(HUDVersions.ModDownloadUrl);
                }
            }

            public static void NotNeedUpdate(ConfigEntryBase entry)
            {
                GUIStyle style = new GUIStyle();

                style.normal.textColor = Color.green;

                style.fontStyle = FontStyle.Bold;

                GUILayout.Label("没有新版本 Not Has New Version", style);

                if (GUILayout.Button("Mod File", GUILayout.ExpandWidth(true)))
                {
                    Application.OpenURL(HUDVersions.ModFileUrl);
                }
            }

            public static void CantKnowUpdate(ConfigEntryBase entry)
            {
                GUIStyle style = new GUIStyle();

                style.normal.textColor = Color.red;

                style.fontStyle = FontStyle.Bold;

                GUILayout.Label("无法连接服务器 Can't Connect Server", style);
            }
        }
    }
}
