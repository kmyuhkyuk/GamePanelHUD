using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompassUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool AngleHUDSW;

        public float AngleNum;

        public float CompassX;

        public Color AzimuthsColor;

        public Color ArrowColor;

        public Color DirectionColor;

        public Color AngleColor;

        public FontStyles AzimuthsAngleStyles;

        public FontStyles DirectionStyles;

        public FontStyles AngleStyles;

        [SerializeField]
        private Image _Arrow;

        [SerializeField]
        private RectTransform _Azimuths;

        [SerializeField]
        private Transform _AzimuthsValue;

        [SerializeField]
        private TMP_Text _DirectionValue;

        [SerializeField]
        private TMP_Text _AngleValue;

        private Transform AnglePanel;

        private Image[] AzimuthsImages;

        private TMP_Text[] AzimuthsAngles;

        void Start()
        {
            AzimuthsImages = _AzimuthsValue.GetComponentsInChildren<Image>();
            AzimuthsAngles = _AzimuthsValue.GetComponentsInChildren<TMP_Text>();
            AnglePanel = _DirectionValue.transform.parent;

#if !UNITY_EDITOR
            GamePanelHUDCorePlugin.UpdateManger.Register(this);
#endif
        }

#if !UNITY_EDITOR
        void OnEnable()
        {
            GamePanelHUDCorePlugin.UpdateManger.Run(this);
        }

        void OnDisable()
        {
            GamePanelHUDCorePlugin.UpdateManger.Stop(this);
        }

        public void IUpdate()
        {
            CompassUI();
        }

        void CompassUI()
#endif
#if UNITY_EDITOR
        void Update()
#endif
        {
            _Arrow.color = ArrowColor;

            foreach (Image image in AzimuthsImages)
            {
                image.color = AzimuthsColor;
            }

            foreach (var text in AzimuthsAngles)
            {
                text.fontStyle = AzimuthsAngleStyles;
                text.color = AzimuthsColor;
            }

            _Azimuths.anchoredPosition = new Vector2(CompassX, 0);

            string direction;
            if (AngleNum >= 45 && AngleNum < 90)
            {
                direction = "NE";
            }
            else if (AngleNum >= 90 && AngleNum < 135)
            {
                direction = "E";
            }
            else if (AngleNum >= 135 && AngleNum < 180)
            {
                direction = "SE";
            }
            else if (AngleNum >= 180 && AngleNum < 225)
            {
                direction = "S";
            }
            else if (AngleNum >= 225 && AngleNum < 270)
            {
                direction = "SW";
            }
            else if (AngleNum >= 270 && AngleNum < 315)
            {
                direction = "W";
            }
            else if (AngleNum >= 315 && AngleNum < 360)
            {
                direction = "NW";
            }
            else
            {
                direction = "N";
            }

            AnglePanel.gameObject.SetActive(AngleHUDSW);

            _DirectionValue.fontStyle = DirectionStyles;
            _DirectionValue.color = DirectionColor;
            _DirectionValue.text = direction;

            _AngleValue.fontStyle = AngleStyles;
            _AngleValue.color = AngleColor;
            _AngleValue.text = ((int)AngleNum).ToString();
        }
    }
}
