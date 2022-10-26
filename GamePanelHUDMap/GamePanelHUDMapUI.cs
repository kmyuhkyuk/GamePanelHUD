using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GamePanelHUDMap
{
    public class GamePanelHUDMapUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public Vector3 PlayerPosition;

        public Vector3 PlayerAngles;

        [SerializeField]
        private Vector2 Offset;

        [SerializeField]
        private TexData[] TexDatas;

        private RectTransform MapRect;

#if !UNITY_EDITOR
        void Start()
        {
            MapRect = GetComponent<RectTransform>();

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            MapUI();
        }

        void MapUI()
        {
            MapRect.anchoredPosition = new Vector2(PlayerPosition.x, PlayerPosition.y) + Offset;

            MapRect.eulerAngles = PlayerAngles;

            foreach (TexData data in TexDatas)
            {
                data.Image.gameObject.SetActive(PlayerPosition.y > data.MinhHigher);
            }
        }
#endif

        [Serializable]
        public struct TexData
        {
            public float MinhHigher;

            public RawImage Image;
        }
    }
}
