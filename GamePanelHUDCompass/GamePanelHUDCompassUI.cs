using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR

using EFTUtils;
using GamePanelHUDCore;

#endif

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompassUI : MonoBehaviour
#if !UNITY_EDITOR

        , IUpdate

#endif
    {
#if !UNITY_EDITOR

        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

#endif

        public bool angleHUDSw;

        public float angleNum;

        public float compassX;

        public Color azimuthsColor;

        public Color azimuthAngleColor;

        public Color arrowColor;

        public Color directionColor;

        public Color angleColor;

        public FontStyles azimuthsAngleStyles;

        public FontStyles directionStyles;

        public FontStyles angleStyles;

        [SerializeField] private Image arrow;

        [SerializeField] private RectTransform azimuthsRoot;

        [SerializeField] private Transform azimuthsValueRoot;

        [SerializeField] private TMP_Text directionValue;

        [SerializeField] private TMP_Text angleValue;

        private Transform _anglePanelTransform;

        private Image[] _azimuthsImages;

        private TMP_Text[] _azimuthsAngles;

        private void Start()
        {
            _azimuthsImages = azimuthsValueRoot.GetComponentsInChildren<Image>();
            _azimuthsAngles = azimuthsValueRoot.GetComponentsInChildren<TMP_Text>();
            _anglePanelTransform = directionValue.transform.parent;

#if !UNITY_EDITOR

            HUDCore.UpdateManger.Register(this);

#endif
        }

#if !UNITY_EDITOR

        private void OnEnable()
        {
            HUDCore.UpdateManger.Run(this);
        }

        private void OnDisable()
        {
            HUDCore.UpdateManger.Stop(this);
        }

        public void CustomUpdate()
        {
            CompassUI();
        }

        private void CompassUI()

#endif
#if UNITY_EDITOR
        void Update()

#endif
        {
            arrow.color = arrowColor;

            foreach (var image in _azimuthsImages)
            {
                image.color = azimuthsColor;
            }

            foreach (var text in _azimuthsAngles)
            {
                text.fontStyle = azimuthsAngleStyles;
                text.color = azimuthAngleColor;
            }

            azimuthsRoot.anchoredPosition = new Vector2(compassX, 0);

            string direction;
            if (angleNum >= 45 && angleNum < 90)
            {
                direction = "NE";
            }
            else if (angleNum >= 90 && angleNum < 135)
            {
                direction = "E";
            }
            else if (angleNum >= 135 && angleNum < 180)
            {
                direction = "SE";
            }
            else if (angleNum >= 180 && angleNum < 225)
            {
                direction = "S";
            }
            else if (angleNum >= 225 && angleNum < 270)
            {
                direction = "SW";
            }
            else if (angleNum >= 270 && angleNum < 315)
            {
                direction = "W";
            }
            else if (angleNum >= 315 && angleNum < 360)
            {
                direction = "NW";
            }
            else
            {
                direction = "N";
            }

            _anglePanelTransform.gameObject.SetActive(angleHUDSw);

            directionValue.fontStyle = directionStyles;
            directionValue.color = directionColor;
            directionValue.text = direction;

            angleValue.fontStyle = angleStyles;
            angleValue.color = angleColor;
            angleValue.text = ((int)angleNum).ToString();
        }
    }
}