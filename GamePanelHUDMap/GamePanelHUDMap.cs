using UnityEngine;
using EFT.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
#endif

namespace GamePanelHUDMap
{
    public class GamePanelHUDMap : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDMapPlugin.MapData, GamePanelHUDMapPlugin.SettingsData> HUD
        {
            get
            {
                return GamePanelHUDMapPlugin.HUD;
            }
        }
#endif

        private AssetBundle AssetBundle;

        private GameObject MapAsset;

        [SerializeField]
        private Transform _Map;

        [SerializeField]
        private GamePanelHUDMapUI _MapUI;

        #if !UNITY_EDITOR
        void Start()
        {
            GamePanelHUDMapPlugin.LoadMap = LoadMapAsset;
            GamePanelHUDMapPlugin.UnloadMap = UnloadMapAsset;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            MapHUD();
        }

        void MapHUD()
        {
            if (_Map != null)
            {
                _Map.gameObject.SetActive(HUD.HUDSW);
            }

            /*if (MapUI != null)
            {
                MapUI.PlayerPosition = HUD.Info.PlayerPosition;
                MapUI.PlayerAngles = HUD.Info.PlayerRotation;
            }*/
        }

        async void LoadMapAsset(string mappath)
        {
            HUD.Info.IsLoadMap = true;

            HUD.Info.IsLoadMap = false;
        }

        void UnloadMapAsset()
        {
        }
#endif
    }
}
