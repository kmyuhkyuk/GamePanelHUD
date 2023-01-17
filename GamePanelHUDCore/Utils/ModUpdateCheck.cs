#if !UNITY_EDITOR
using BepInEx.Configuration;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace GamePanelHUDCore.Utils
{
    public class ModUpdateCheck
    {
        private static readonly HUDVersion HUDVersions = new HUDVersion();

        private static readonly List<UpdateData> UpdateDatas = new List<UpdateData>();

        private const string UpdateDataUrl = "https://dev.sp-tarkov.com/kmyuhkyuk/GamePanelHUD/raw/branch/master/Version.json";

        private const string Section = "主更新检查 Update Check";

        public static async void ServerCheck()
        {
            HUDVersions.ServerConnect = default;

            UnityWebRequest www = UnityWebRequest.Get(UpdateDataUrl);

            www.SendWebRequest();

            while (!www.isDone)
                await Task.Yield();

            if (www.isHttpError || www.isNetworkError)
            {
                HUDVersions.Set(false, new Version(), new Version(), new Version(), null, null);
            }
            else
            {
                HUDVersion hudVersion = JsonConvert.DeserializeObject<HUDVersion>(www.downloadHandler.text);

                HUDVersions.Set(true, hudVersion.ModVersion, hudVersion.FirstGameVersion, hudVersion.LastGameVersion, hudVersion.ModFileUrl, hudVersion.ModDownloadUrl);
            }
        }

        public static async void DrawNeedUpdate(ConfigFile config, Version version)
        {
            Action<ConfigEntryBase> draw;

            while (!HUDVersions.ServerConnect.HasValue)
                await Task.Yield();

            bool serverConnect = (bool)HUDVersions.ServerConnect;

            if (!serverConnect)
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

            ConfigEntry<string> configEntry = config.Bind(Section, "Update", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 2, HideDefaultButton = true, CustomDrawer = draw, HideSettingName = true }));

            ConfigEntry<string> warn = default;

            if (serverConnect)
            {
                Version gameVersion = GamePanelHUDCorePlugin.HUDCoreClass.GameVersion;

                if (HUDVersions.FirstGameVersion > gameVersion)
                {
                    warn = config.Bind(Section, "First Update", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1, HideDefaultButton = true, CustomDrawer = Draw.FirstUpdate, HideSettingName = true }));
                }
                else if (HUDVersions.LastGameVersion < gameVersion)
                {
                    warn = config.Bind(Section, "Last Update", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1, HideDefaultButton = true, CustomDrawer = Draw.LastUpdate, HideSettingName = true }));
                }
            }

            UpdateDatas.Add(new UpdateData(config, configEntry, warn, version));
        }

        public class HUDVersion
        {
            [NonSerialized]
            public bool? ServerConnect;

            public Version ModVersion;

            //First Support Game Version
            public Version FirstGameVersion;

            //Last Support Game Version
            public Version LastGameVersion;

            public string ModFileUrl;

            public string ModDownloadUrl;

            public void Set(bool connect, Version version, Version firstversion, Version lastversion, string fileurl, string downloadurl)
            {
                ServerConnect = connect;
                ModVersion = version;
                FirstGameVersion = firstversion;
                LastGameVersion = lastversion;
                ModFileUrl = fileurl;
                ModDownloadUrl = downloadurl;
            }
        }

        public class UpdateData
        {
            public ConfigFile ModConfigFile;
            
            public ConfigEntry<string> ModConfigEntry;

            public ConfigEntry<string> ModWarn;

            public Version ModVersion;

            public UpdateData(ConfigFile configfile, ConfigEntry<string> configentry, ConfigEntry<string> warn, Version version)
            {
                ModConfigFile = configfile;
                ModConfigEntry = configentry;
                ModWarn = warn;
                ModVersion = version;
            }
        }

        public class Draw
        {
            private static readonly GUIStyle Red = new GUIStyle();

            private static readonly GUIStyle Yellow = new GUIStyle();

            private static readonly GUIStyle Green = new GUIStyle();

            static Draw()
            {
                Red.normal.textColor = Color.red;
                Red.fontStyle = FontStyle.Bold;

                Yellow.normal.textColor = Color.yellow;
                Yellow.fontStyle = FontStyle.Bold;

                Green.normal.textColor = Color.green;
                Green.fontStyle = FontStyle.Bold;
            }

            public static void NeedUpdate(ConfigEntryBase entry)
            {
                GUILayout.BeginVertical();

                GUILayout.Label("有新版本 Have Latest version", Yellow);

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
                    Retry();
                }

                GUILayout.EndVertical();
            }

            public static void NotNeedUpdate(ConfigEntryBase entry)
            {
                GUILayout.Label("已经是最新版本 Already is latest version", Green);

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
                GUILayout.Label("无法连接服务器 Can't Connect Server", Red);

                if (GUILayout.Button("Retry", GUILayout.ExpandWidth(true)))
                {
                    Retry();
                }
            }

            public static void FirstUpdate(ConfigEntryBase entry)
            {
                GUILayout.BeginVertical();

                GUILayout.Label("游戏版本低于模组支持版本, 它可能不会工作", Red);
                GUILayout.Label("Game Version Below Mod Support Version, It Maybe Can't Work", Red);

                GUILayout.EndVertical();
            }

            public static void LastUpdate(ConfigEntryBase entry)
            {
                GUILayout.BeginVertical();

                GUILayout.Label("游戏版本高于模组支持版本, 它可能会有些问题", Yellow);
                GUILayout.Label("Game Version Higher than Mod Support Version, It Maybe Will Be Problems", Yellow);

                GUILayout.EndVertical();
            }

            private static async void Retry()
            {
                ServerCheck();

                while (!HUDVersions.ServerConnect.HasValue)
                    await Task.Yield();

                if ((bool)HUDVersions.ServerConnect)
                {
                    foreach (UpdateData data in UpdateDatas)
                    {
                        Action<ConfigEntryBase> draw;
                        if (HUDVersions.ModVersion > data.ModVersion)
                        {
                            draw = NeedUpdate;
                        }
                        else
                        {
                            draw = NotNeedUpdate;
                        }

                        data.ModConfigFile.Remove(data.ModConfigEntry.Definition);
                        data.ModConfigEntry = data.ModConfigFile.Bind(Section, "Update", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 2, HideDefaultButton = true, CustomDrawer = draw, HideSettingName = true }));

                        if (data.ModWarn != null)
                        {
                            data.ModConfigFile.Remove(data.ModWarn.Definition);
                        }

                        ConfigEntry<string> warn = default;

                        Version gameVersion = GamePanelHUDCorePlugin.HUDCoreClass.GameVersion;

                        if (HUDVersions.FirstGameVersion > gameVersion)
                        {
                            warn = data.ModConfigFile.Bind(Section, "First Update", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1, HideDefaultButton = true, CustomDrawer = FirstUpdate, HideSettingName = true }));
                        }
                        else if (HUDVersions.LastGameVersion < gameVersion)
                        {
                            warn = data.ModConfigFile.Bind(Section, "Last Update", "", new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1, HideDefaultButton = true, CustomDrawer = LastUpdate, HideSettingName = true }));
                        }

                        data.ModWarn = warn;
                    }
                }
            }
        }
    }
}
#endif
