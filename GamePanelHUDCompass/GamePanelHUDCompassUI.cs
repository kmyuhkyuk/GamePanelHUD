using System.Text;
using System.Linq;
using System.Collections.Generic;
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

        public string AzimuthsAngleColor;

        public string DirectionColor;

        public string AngleColor;

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

        private Transform _AnglePanel;

        private Image[] _AzimuthsImage;

        private TMP_Text[] _AzimuthsAngle;

        private string[] AngleTexts;

        private readonly StringBuilderData StringBuilderDatas = new StringBuilderData();

        void Start()
        {
            _AzimuthsImage = _AzimuthsValue.GetComponentsInChildren<Image>();
            _AzimuthsAngle = _AzimuthsValue.GetComponentsInChildren<TMP_Text>();
            _AnglePanel = _DirectionValue.transform.parent;

            AngleTexts = _AzimuthsAngle.Select(x => x.text).ToArray();

            List<StringBuilder> changes = new List<StringBuilder>();

            for (int i = 0; i < AngleTexts.Length; i++)
            {
                changes.Add(new StringBuilder(128));
            }

            StringBuilderDatas._AzimuthsAngle = changes.ToArray();

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

            foreach (Image image in _AzimuthsImage)
            {
                image.color = AzimuthsColor;
            }

            for (int i = 0; i < _AzimuthsAngle.Length; i++)
            {
                TMP_Text _azimuthsAngle = _AzimuthsAngle[i];

                _azimuthsAngle.fontStyle = AzimuthsAngleStyles;
                _azimuthsAngle.text = StringBuilderDatas._AzimuthsAngle[i].StringConcat("<color=", AzimuthsAngleColor, ">", AngleTexts[i], "</color>");
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

            _AnglePanel.gameObject.SetActive(AngleHUDSW);

            _DirectionValue.fontStyle = DirectionStyles;
            _DirectionValue.text = StringBuilderDatas._DirectionValue.StringConcat("<color=", DirectionColor, ">", direction, "</color>");

            _AngleValue.fontStyle = AngleStyles;
            _AngleValue.text = StringBuilderDatas._AngleValue.StringConcat("<color=", AngleColor, ">", (int)AngleNum, "</color>");
        }

        public class StringBuilderData
        {
            public StringBuilder[] _AzimuthsAngle;
            public StringBuilder _DirectionValue = new StringBuilder(128);
            public StringBuilder _AngleValue = new StringBuilder(128);
        }
    }
}
