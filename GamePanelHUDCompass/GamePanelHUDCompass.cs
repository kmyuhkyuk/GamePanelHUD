using UnityEngine;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore;
#endif

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompass : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;
        private static
            GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassData,
                GamePanelHUDCompassPlugin.SettingsData> HUD => GamePanelHUDCompassPlugin.CompassHUD;
#endif

        [SerializeField] private GamePanelHUDCompassUI compass;

        private RectTransform _rectTransform;

#if !UNITY_EDITOR
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();

            HUDCore.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            CompassHUD();
        }

        private void CompassHUD()
        {
            _rectTransform.anchoredPosition = HUD.SetData.KeyAnchoredPosition.Value;
            _rectTransform.sizeDelta = HUD.Info.SizeDelta;
            _rectTransform.localScale = HUD.SetData.KeyLocalScale.Value;

            if (compass != null)
            {
                compass.gameObject.SetActive(HUD.HUDSw);
                compass.angleHUDSw = HUD.SetData.KeyAngleHUDSw.Value;

                compass.angleNum = HUD.Info.Angle;
                compass.compassX = HUD.Info.CompassX;

                compass.arrowColor = HUD.SetData.KeyArrowColor.Value;
                compass.azimuthsColor = HUD.SetData.KeyAzimuthsColor.Value;
                compass.azimuthAngleColor = HUD.SetData.KeyAzimuthsAngleColor.Value;
                compass.directionColor = HUD.SetData.KeyDirectionColor.Value;
                compass.angleColor = HUD.SetData.KeyAngleColor.Value;

                compass.azimuthsAngleStyles = HUD.SetData.KeyAzimuthsAngleStyles.Value;
                compass.directionStyles = HUD.SetData.KeyDirectionStyles.Value;
                compass.angleStyles = HUD.SetData.KeyAngleStyles.Value;
            }
        }
#endif
    }
}