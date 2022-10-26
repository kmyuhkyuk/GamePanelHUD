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
        private Transform _MapUI;

        private GamePanelHUDMapUI _Map;

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

                _Map.PlayerPosition = HUD.Info.PlayerTransform.localPosition;
                _Map.PlayerAngles = HUD.Info.PlayerTransform.eulerAngles;
            }
        }

        async void LoadMapAsset(string mappath)
        {
            HUD.Info.IsLoadMap = true;

            AssetBundle = await BundleHelp.LoadAsyncBundle(mappath);

            MapAsset = (await BundleHelp.LoadAsyncAllAsset<GameObject>(AssetBundle))[0];

            GameObject mapAsset = Instantiate(MapAsset, _MapUI);

            _Map = mapAsset.GetComponent<GamePanelHUDMapUI>();

            HUD.Info.IsLoadMap = false;
        }

        void UnloadMapAsset()
        {
            if (AssetBundle != null && MapAsset != null)
            {
                Destroy(MapAsset);

                AssetBundle.Unload(true);
            }
        }
#endif
    }
}
