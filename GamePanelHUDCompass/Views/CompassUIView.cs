using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore.Models;

#endif

namespace GamePanelHUDCompass.Views
{
    public class CompassUIView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool angleHUDSw;

        public float angleNum;

        public float compassX;

        public Color azimuthsColor;

        public Color azimuthsAngleColor;

        public Color arrowColor;

        public Color directionColor;

        public Color angleColor;

        public FontStyles azimuthsAngleStyles;

        public FontStyles directionStyles;

        public FontStyles angleStyles;

        [SerializeField] private Image arrow;

        [SerializeField] private RectTransform azimuthsRoot;

        [SerializeField] private TMP_Text directionValue;

        [SerializeField] private TMP_Text angleValue;

        [SerializeField] private Image azimuths;

        [SerializeField] private TMP_Text azimuthsAngle;

        [SerializeField] private Image virtualLeftAzimuths;

        [SerializeField] private TMP_Text virtualLeftAzimuthsAngle;

        [SerializeField] private Image virtualRightAzimuths;

        [SerializeField] private TMP_Text virtualRightAzimuthsAngle;

        private Transform _anglePanelTransform;

        private static readonly int MaskSoftnessX = Shader.PropertyToID("_MaskSoftnessX");

#if !UNITY_EDITOR
        private void Awake()
        {
            azimuthsAngle.fontMaterial.SetFloat(MaskSoftnessX, 100);
            virtualLeftAzimuthsAngle.fontMaterial.SetFloat(MaskSoftnessX, 100);
            virtualRightAzimuthsAngle.fontMaterial.SetFloat(MaskSoftnessX, 100);

            _anglePanelTransform = directionValue.transform.parent;
        }

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        private void OnEnable()
        {
            HUDCoreModel.Instance.UpdateManger.Run(this);
        }

        private void OnDisable()
        {
            HUDCoreModel.Instance.UpdateManger.Stop(this);
        }

        public void CustomUpdate()
        {
            arrow.color = arrowColor;

            azimuths.color = azimuthsColor;

            azimuthsAngle.fontStyle = azimuthsAngleStyles;
            azimuthsAngle.color = azimuthsAngleColor;

            virtualLeftAzimuths.color = azimuthsColor;

            virtualLeftAzimuthsAngle.fontStyle = azimuthsAngleStyles;
            virtualLeftAzimuthsAngle.color = azimuthsAngleColor;

            virtualRightAzimuths.color = azimuthsColor;

            virtualRightAzimuthsAngle.fontStyle = azimuthsAngleStyles;
            virtualRightAzimuthsAngle.color = azimuthsAngleColor;

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

#endif
    }
}