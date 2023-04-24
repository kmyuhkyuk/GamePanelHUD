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

        private Transform AnglePanel;

        private Image[] AzimuthsImages;

        private TMP_Text[] AzimuthsAngles;

        private string[] AngleTexts;

        private readonly IStringBuilderData IStringBuilderDatas = new IStringBuilderData();

        void Start()
        {
            AzimuthsImages = _AzimuthsValue.GetComponentsInChildren<Image>();
            AzimuthsAngles = _AzimuthsValue.GetComponentsInChildren<TMP_Text>();
            AnglePanel = _DirectionValue.transform.parent;

            AngleTexts = AzimuthsAngles.Select(x => x.text).ToArray();

            List<IStringBuilder> changes = new List<IStringBuilder>();

            for (int i = 0; i < AngleTexts.Length; i++)
            {
                changes.Add(new IStringBuilder());
            }

            IStringBuilderDatas._AzimuthsAngle = changes.ToArray();

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

            for (int i = 0; i < AzimuthsAngles.Length; i++)
            {
                TMP_Text _azimuthsAngle = AzimuthsAngles[i];

                _azimuthsAngle.fontStyle = AzimuthsAngleStyles;
                _azimuthsAngle.text = IStringBuilderDatas._AzimuthsAngle[i].Concat("<color=", AzimuthsAngleColor, ">", AngleTexts[i], "</color>");
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
            _DirectionValue.text = IStringBuilderDatas._DirectionValue.Concat("<color=", DirectionColor, ">", direction, "</color>");

            _AngleValue.fontStyle = AngleStyles;
            _AngleValue.text = IStringBuilderDatas._AngleValue.Concat("<color=", AngleColor, ">", ((int)AngleNum).ToString(), "</color>");
        }

        public class IStringBuilderData
        {
            public IStringBuilder[] _AzimuthsAngle;
            public IStringBuilder _DirectionValue = new IStringBuilder();
            public IStringBuilder _AngleValue = new IStringBuilder();
        }
    }
}
