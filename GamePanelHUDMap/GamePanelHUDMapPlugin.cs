#if !UNITY_EDITOR

using System;
using System.IO;
using BepInEx;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
using UnityEngine;

namespace GamePanelHUDMap
{
    [BepInPlugin("com.kmyuhkyuk.GamePanelHUDMap", "GamePanelHUDMap", "3.2.0")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class GamePanelHUDMapPlugin : BaseUnityPlugin, IUpdate
    {
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore
        {
            get { return GamePanelHUDCorePlugin.HUDCore; }
        }

        internal static readonly GamePanelHUDCorePlugin.HUDClass<MapData, SettingsData> HUD =
            new GamePanelHUDCorePlugin.HUDClass<MapData, SettingsData>();

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
    }
}

#endif