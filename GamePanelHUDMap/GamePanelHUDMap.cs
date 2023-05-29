using UnityEngine;
#if !UNITY_EDITOR
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
#endif

namespace GamePanelHUDMap
{
    public class GamePanelHUDMap : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDMapPlugin.MapData, GamePanelHUDMapPlugin.SettingsData>
            HUD => GamePanelHUDMapPlugin.HUD;
#endif

        private AssetBundle _assetBundle;

        private GameObject _mapAsset;

        [SerializeField] private Transform map;

        [SerializeField] private GamePanelHUDMapUI mapUI;

#if !UNITY_EDITOR
        private void Start()
        {
            GamePanelHUDMapPlugin.LoadMap = LoadMapAsset;
            GamePanelHUDMapPlugin.UnloadMap = UnloadMapAsset;

            HUDCore.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            MapHUD();
        }

        private void MapHUD()
        {
            if (map != null)
            {
                map.gameObject.SetActive(HUD.HUDSw);
            }

            /*if (MapUI != null)
            {
                MapUI.PlayerPosition = HUD.Info.PlayerPosition;
                MapUI.PlayerAngles = HUD.Info.PlayerRotation;
            }*/
        }

        private async void LoadMapAsset(string mappath)
        {
            HUD.Info.IsLoadMap = true;

            HUD.Info.IsLoadMap = false;
        }

        private void UnloadMapAsset()
        {
        }
#endif
    }
}