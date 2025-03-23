using UnityEngine;
#if !UNITY_EDITOR
using KmyTarkovUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDCompass.Models;
using SettingsModel = GamePanelHUDCompass.Models.SettingsModel;

#endif

namespace GamePanelHUDCompass.Views
{
    public class CompassHUDView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        [SerializeField] private CompassUIView compassUIView;

        private RectTransform _rectTransform;

        private CanvasGroup _compassGroup;

#if !UNITY_EDITOR

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            _compassGroup = compassUIView.GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var compassHUDModel = CompassHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            _compassGroup.alpha = settingsModel.KeyImmersiveCompass.Value
                ? compassHUDModel.Compass.CompassState ? 1 : 0
                : 1;

            _rectTransform.anchoredPosition = settingsModel.KeyAnchoredPosition.Value;
            _rectTransform.sizeDelta = compassHUDModel.Compass.SizeDelta;
            _rectTransform.localScale = settingsModel.KeyLocalScale.Value;

            compassUIView.gameObject.SetActive(compassHUDModel.CompassHUDSw);
            compassUIView.angleHUDSw = settingsModel.KeyAngleHUDSw.Value;

            compassUIView.angleNum = compassHUDModel.Compass.Angle;
            compassUIView.compassX = compassHUDModel.Compass.CompassX;

            compassUIView.arrowColor = settingsModel.KeyArrowColor.Value;
            compassUIView.azimuthsColor = settingsModel.KeyAzimuthsColor.Value;
            compassUIView.azimuthsAngleColor = settingsModel.KeyAzimuthsAngleColor.Value;
            compassUIView.directionColor = settingsModel.KeyDirectionColor.Value;
            compassUIView.angleColor = settingsModel.KeyAngleColor.Value;

            compassUIView.azimuthsAngleStyles = settingsModel.KeyAzimuthsAngleStyles.Value;
            compassUIView.directionStyles = settingsModel.KeyDirectionStyles.Value;
            compassUIView.angleStyles = settingsModel.KeyAngleStyles.Value;
        }

#endif
    }
}