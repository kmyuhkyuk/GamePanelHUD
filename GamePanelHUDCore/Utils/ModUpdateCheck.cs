using BepInEx.Configuration;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Xml.Schema;

namespace GamePanelHUDCore.Utils
{
    public class ModUpdateCheck
    {
        private static readonly HUDVersion HUDVersions = new HUDVersion();

        private static readonly string UpdateDataUrl = "https://dev.sp-tarkov.com/kmyuhkyuk/GamePanelHUD/raw/branch/master/GamePanelHUDCore/Version.json";

        private static readonly List<UpdateData> UpdateDatas = new List<UpdateData>();

        public static async void ServerCheck()
        {
            HUDVersions.ServerConnect = null;

            UnityWebRequest www = UnityWebRequest.Get(UpdateDataUrl);

            www.SendWebRequest();

            while (!www.isDone)
                await Task.Yield();

            if (www.isHttpError || www.isNetworkError)
            {
                HUDVersions.Set(false, new Version(), null, null);
            }
            else
            {
                HUDVersion hudVersion = JsonConvert.DeserializeObject<HUDVersion>(www.downloadHandler.text);

                HUDVersions.Set(true, hudVersion.ModVersion, hudVersion.ModFileUrl, hudVersion.ModDownloadUrl);
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

            ConfigEntry<string> configEntry = config.Bind("主更新检查 Update Check", "Update", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1, HideDefaultButton = true, CustomDrawer = draw, HideSettingName = true }));

            UpdateDatas.Add(new UpdateData(config, configEntry, version));
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

        public class UpdateData
        {
            public ConfigFile ModConfigFile;
            
            public ConfigEntry<string> ModConfigEntry;

            public Version ModVersion;

            public UpdateData(ConfigFile configfile, ConfigEntry<string> configentry, Version version)
            {
                ModConfigFile = configfile;
                ModConfigEntry = configentry;
                ModVersion = version;
            }
        }

        public class Draw
        {
            private static readonly GUIStyle Red = new GUIStyle();

            static Draw()
            {
                Red.normal.textColor = Color.yellow;
                Red.fontStyle = FontStyle.Bold;
            }

            public static void NeedUpdate(ConfigEntryBase entry)
            {
                GUILayout.BeginVertical();

                GUILayout.Label("有新版本 Has New Version", Red);

                GUILayout.Label(string.Concat("最新版本 Latest Version: ", HUDVersions.ModVersion), Red);

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
                    Retry();
                }

                GUILayout.EndVertical();
            }

            public static void NotNeedUpdate(ConfigEntryBase entry)
            {
                GUIStyle style = new GUIStyle();

                style.normal.textColor = Color.green;

                style.fontStyle = FontStyle.Bold;

                GUILayout.Label("没有新版本 Not Has New Version", style);

                GUILayout.BeginVertical();

                if (GUILayout.Button("Mod File", GUILayout.ExpandWidth(true)))
                {
                    Application.OpenURL(HUDVersions.ModFileUrl);
                }

                if (GUILayout.Button("Retry", GUILayout.ExpandWidth(true)))
                {
                    Retry();
                }

                GUILayout.EndVertical();
            }

            public static void CantKnowUpdate(ConfigEntryBase entry)
            {
                GUIStyle style = new GUIStyle();

                style.normal.textColor = Color.red;

                style.fontStyle = FontStyle.Bold;

                GUILayout.Label("无法连接服务器 Can't Connect Server", style);

                if (GUILayout.Button("Retry", GUILayout.ExpandWidth(true)))
                {
                    Retry();
                }
            }

            private static async void Retry()
            {
                ServerCheck();

                while (!HUDVersions.ServerConnect.HasValue)
                    await Task.Yield();

                if ((bool)HUDVersions.ServerConnect)
                {
                    for (int i = 0; i < UpdateDatas.Count; i++)
                    {
                        UpdateData updateData = UpdateDatas[i];

                        Action<ConfigEntryBase> draw;

                        if (HUDVersions.ModVersion > updateData.ModVersion)
                        {
                            draw = NeedUpdate;
                        }
                        else
                        {
                            draw = NotNeedUpdate;
                        }

                        updateData.ModConfigFile.Remove(updateData.ModConfigEntry.Definition);

                        updateData.ModConfigEntry = updateData.ModConfigFile.Bind("主更新检查 Update Check", "Update", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1, HideDefaultButton = true, CustomDrawer = draw, HideSettingName = true }));
                    }
                }
            }
        }
    }
}
