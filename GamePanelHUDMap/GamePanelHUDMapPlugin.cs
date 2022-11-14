#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
using System.Threading.Tasks;

namespace GamePanelHUDMap
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDMap", "kmyuhkyuk-GamePanelHUDMap", "2.3.0")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class GamePanelHUDMapPlugin : BaseUnityPlugin, IUpdate
    {
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore
        {
            get
            {
                return GamePanelHUDCorePlugin.HUDCore;
            }
        }

        internal static readonly GamePanelHUDCorePlugin.HUDClass<MapData, SettingsData> HUD = new GamePanelHUDCorePlugin.HUDClass<MapData, SettingsData>();

        private string MapPath;

        private bool MapHUDSW;

        private bool HasMap;

        private string Infiltration;

        private readonly MapData MapDatas = new MapData();

        private readonly SettingsData SettingsDatas = new SettingsData();

        internal static Action<string> LoadMap;

        internal static Action UnloadMap;

        private readonly string ServerVersionUrl = "https://dev.sp-tarkov.com/kmyuhkyuk/GamePanelHUD/raw/branch/master/GamePanelHUDMap/MapData/Version.json";

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDMap");

            MapPath = Path.Combine(HUDCore.ModPath, "map");

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        private void Awake()
        {
            HUDCore.LoadHUD("gamepanlmaphud.bundle", "gamepanlmaphud");
        }

        public void IUpdate()
        {
            MapPlugin();
        }

        async void AutoUpdate()
        {
            UnityWebRequest www = UnityWebRequest.Get(ServerVersionUrl);

            www.SendWebRequest();

            while (!www.isDone)
                await Task.Yield();

            if (www.isHttpError || www.isNetworkError)
            {

            }
            else
            {
                MapVersionData[] serverVersionData = JsonConvert.DeserializeObject<MapVersionData[]>(www.downloadHandler.text);

                MapVersionData[] localVersionData = JsonConvert.DeserializeObject<MapVersionData[]>(new StreamReader(Path.Combine(MapPath, "Version.json")).ReadToEnd());

                for (int i = 0; i < serverVersionData.Length; i++)
                {
                    MapVersionData serverData = serverVersionData[i];

                    MapVersionData localData = localVersionData[i];

                    if (localData == null || (serverData.MapVersion > localData.MapVersion && serverData.GameVersion >= localData.GameVersion))
                    {

                    }
                }
            }
        }

        void MapPlugin()
        {
            MapHUDSW = HUDCore.AllHUDSW && HasMap && !MapDatas.IsLoadMap && HUDCore.HasPlayer;

            HUD.Set(MapDatas, SettingsDatas, MapHUDSW);

            if (HUDCore.HasPlayer)
            {
                Infiltration = HUDCore.IsYourPlayer.Infiltration;

                if (!HasMap)
                {
                    LoadMap(Path.Combine(MapPath, string.Concat(Infiltration, ".json")));

                    HasMap = true;
                }

                MapDatas.PlayerPosition = HUDCore.IsYourPlayer.Transform.Original.position;

                MapDatas.PlayerRotation = HUDCore.IsYourPlayer.CameraPosition.eulerAngles;
            }
            else
            {
                UnloadMap();

                HasMap = false;
            }
        }

        public class MapData
        {
            public Vector3 PlayerPosition;

            public Vector3 PlayerRotation;

            public bool IsLoadMap;
        }

        public class MapVersionData
        {
            public string MapName;

            public int GameVersion;

            public int MapVersion;
        }

        public class SettingsData
        {

        }
    }
}
#endif
