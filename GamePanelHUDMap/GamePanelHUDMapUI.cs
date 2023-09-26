using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
using UnityEngine;

namespace GamePanelHUDMap
{
    public class GamePanelHUDMapUI : MonoBehaviour
#if !UNITY_EDITOR

        , IUpdate

#endif
    {
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

        public Vector3 playerPosition;

        public Vector3 playerAngles;

        //public TexData[] TexDatas;

        [SerializeField] private Vector2 offset;

        private RectTransform _mapRect;

#if !UNITY_EDITOR

        private void Start()
        {
            _mapRect = GetComponent<RectTransform>();

            HUDCore.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            MapUI();
        }

        private void MapUI()
        {
            _mapRect.anchoredPosition = new Vector2(playerPosition.x, playerPosition.y) + offset;

            _mapRect.eulerAngles = new Vector3(0, 0, playerAngles.y);

            /*foreach (TexData data in TexDatas)
            {
                data.Image.gameObject.SetActive(PlayerPosition.y > data.MinhHigher);
            }*/
        }

#endif
    }
}