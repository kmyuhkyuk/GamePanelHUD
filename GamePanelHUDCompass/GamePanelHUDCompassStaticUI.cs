/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompassStaticUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassStaticData, GamePanelHUDCompassPlugin.SettingsData> HUD
        {
            get
            {
                return GamePanelHUDCompassPlugin.CompassStaticHUD;
            }
        }
#endif
        public Sprite Icon;

        public Vector3 Where;

        public bool ToDestroy;

        [SerializeField]
        private Image _Real;

        [SerializeField]
        private Image _Virtual;

        [SerializeField]
        private Image _Virtual2;

        [SerializeField]
        private Image _Virtual3;

        private RectTransform RealRect;

        private RectTransform VirtualRect;

        private RectTransform Virtual2Rect;

        private RectTransform Virtual3Rect;

        private float IconX;

        private float IconXLeft
        {
            get
            {
                return IconX - 2880;
            }
        }

        private float IconXRight
        {
            get
            {
                return IconX + 2880;
            }
        }

        private float IconXRightRight
        {
            get
            {
                return IconX + 5760; //2880 * 2
            }
        }

#if !UNITY_EDITOR
        void Start()
        {
            _Real.sprite = Icon;
            _Virtual.sprite = Icon;
            _Virtual2.sprite = Icon;
            _Virtual3.sprite = Icon;

            RealRect = _Real.GetComponent<RectTransform>();
            VirtualRect = _Virtual.GetComponent<RectTransform>();
            Virtual2Rect = _Virtual2.GetComponent<RectTransform>();
            Virtual3Rect = _Virtual3.GetComponent<RectTransform>();
        }

        public void IUpdate()
#endif
#if UNITY_EDITOR
        void Update()
#endif
        {
            CompassStaticUI();
        }

        void CompassStaticUI()
        {
            Vector3 lhs = Where - HUD.Info.PlayerPosition;

            float angle = HUD.Info.GetToAngle(lhs, HUD.SettingsData.KeyAngleOffset.Value);

            IconX = -(angle / 15 * 120);

            RealRect.anchoredPosition = new Vector2(IconX, HUD.SettingsData.KeyCompassFireHeight.Value);
            VirtualRect.anchoredPosition = new Vector2(IconXLeft, HUD.SettingsData.KeyCompassFireHeight.Value);
            Virtual2Rect.anchoredPosition = new Vector2(IconXRight, HUD.SettingsData.KeyCompassFireHeight.Value);
            Virtual3Rect.anchoredPosition = new Vector2(IconXRightRight, HUD.SettingsData.KeyCompassFireHeight.Value);
        }
    }
}*/
