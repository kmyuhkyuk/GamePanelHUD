using UnityEngine;
using EFT.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompass : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassData, GamePanelHUDCompassPlugin.SettingsData> HUD
        {
            get
            {
                return GamePanelHUDCompassPlugin.CompassHUD;
            }
        }
#endif

        [SerializeField]
        private GamePanelHUDCompassUI _Compass;

#if !UNITY_EDITOR
        void Start()
        {
            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            CompassHUD();
        }

        void CompassHUD()
        {
            RectTransform.anchoredPosition = HUD.SettingsData.KeyAnchoredPosition.Value;
            RectTransform.sizeDelta = HUD.Info.SizeDelta;
            RectTransform.localScale = HUD.SettingsData.KeyLocalScale.Value;

            if (_Compass != null)
            {
                _Compass.gameObject.SetActive(HUD.HUDSW);
                _Compass.AngleHUDSW = HUD.SettingsData.KeyAngleHUDSW.Value;

                _Compass.AngleNum = HUD.Info.Angle;
                _Compass.CompassX = HUD.Info.CompassX;

                _Compass.ArrowColor = HUD.SettingsData.KeyArrowColor.Value;
                _Compass.AzimuthsColor = HUD.SettingsData.KeyAzimuthsColor.Value;
                _Compass.DirectionColor = HUD.SettingsData.KeyDirectionColor.Value;
                _Compass.AngleColor = HUD.SettingsData.KeyAngleColor.Value;

                _Compass.AzimuthsAngleStyles = HUD.SettingsData.KeyAzimuthsAngleStyles.Value;
                _Compass.DirectionStyles = HUD.SettingsData.KeyDirectionStyles.Value;
                _Compass.AngleStyles = HUD.SettingsData.KeyAngleStyles.Value;
            }
        }
#endif
    }
}
