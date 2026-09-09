#if !UNITY_EDITOR

using BepInEx;
using GamePanelHUDCore;
using GamePanelHUDCore.Models;
using System;
using System.IO;
using UnityEngine;

namespace GamePanelHUDMap
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDMap", "GamePanelHUDMap", "4.1.5")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class GamePanelHUDMapPlugin : BaseUnityPlugin, KmyTarkovUtils.IUpdate
    {
        private HUDCoreModel HUDCore => HUDCoreModel.Instance;

        internal static readonly HUDClass<MapData, SettingsData> HUD = new HUDClass<MapData, SettingsData>();

        private string _mapPath;

        private bool _mapHudsw;

        private bool _hasMap;

        private string _infiltration;

        private readonly MapData _mapDatas = new MapData();

        private readonly SettingsData _settingsDatas = new SettingsData();

        internal static Action<string> LoadMap;

        internal static Action UnloadMap;

        private void Awake()
        {
            HUDCore.LoadHUD("gamepanelmaphud.bundle", "gamepanelmaphud");
        }

        private void Start()
        {
            _mapPath = Path.Combine(HUDCore.ModPath, "map");

            HUDCore.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            MapPlugin();
        }

        private void MapPlugin()
        {
            _mapHudsw = HUDCore.AllHUDSw && _hasMap && !_mapDatas.IsLoadMap && HUDCore.HasPlayer;

            HUD.Set(_mapDatas, _settingsDatas, _mapHudsw);

            if (HUDCore.HasPlayer)
            {
                _infiltration = HUDCore.YourPlayer.Infiltration;

                if (!_hasMap)
                {
                    LoadMap(Path.Combine(_mapPath, string.Concat(_infiltration, ".json")));

                    _hasMap = true;
                }

                _mapDatas.PlayerPosition = HUDCore.YourPlayer.Position;

                _mapDatas.PlayerRotation = HUDCore.YourPlayer.CameraPosition.eulerAngles;
            }
            else
            {
                UnloadMap();

                _hasMap = false;
            }
        }

        public class MapData
        {
            public Vector3 PlayerPosition;

            public Vector3 PlayerRotation;

            public bool IsLoadMap;
        }

        public class SettingsData
        {
        }

        public class HUDClass<TData, TSettings>
        {
            public TData Info { get; private set; }

            public TSettings Settings { get; private set; }

            public bool HUDSw { get; private set; }

            public void Set(TData info, TSettings settings, bool hudSw)
            {
                Info = info;
                Settings = settings;
                HUDSw = hudSw;
            }
        }
    }
}

#endif