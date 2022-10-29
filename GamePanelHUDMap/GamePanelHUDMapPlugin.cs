#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using System;
using System.IO;
using UnityEngine;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;

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

        private void Start()
        {
            Logger.LogInfo("Loaded: kmyuhkyuk-GamePanelHUDMap");

            MapPath = Path.Combine(HUDCore.ModPath, "bundles", "map");

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

        void MapPlugin()
        {
            MapHUDSW = HUDCore.AllHUDSW && HasMap && !MapDatas.IsLoadMap && HUDCore.HasPlayer;

            HUD.Set(MapDatas, SettingsDatas, MapHUDSW);

            if (HUDCore.HasPlayer)
            {
                Infiltration = HUDCore.IsYourPlayer.Infiltration;

                if (!HasMap)
                {
                    LoadMap(Path.Combine(MapPath, Infiltration));

                    HasMap = true;
                }

                MapDatas.PlayerTransform = HUDCore.IsYourPlayer.Transform.Original;
            }
            else
            {
                UnloadMap();

                HasMap = false;
            }
        }


        public class MapData
        {
            public Transform PlayerTransform;

            public bool IsLoadMap;
        }

        public class SettingsData
        {

        }
    }
}
#endif
