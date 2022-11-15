using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        //public TexData[] TexDatas;

        [SerializeField]
        private Vector2 Offset;

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

            MapRect.eulerAngles = new Vector3(0, 0, PlayerAngles.y);

            /*foreach (TexData data in TexDatas)
            {
                data.Image.gameObject.SetActive(PlayerPosition.y > data.MinhHigher);
            }*/
        }
#endif
    }
}
