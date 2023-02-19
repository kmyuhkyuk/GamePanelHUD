using UnityEngine;
using EFT.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
#endif

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompass : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassInfo, GamePanelHUDCompassPlugin.SettingsData> HUD
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
                _Compass.AzimuthsAngleColor = HUD.SettingsData.KeyAzimuthsAngleColor.Value.ColorToHtml();
                _Compass.DirectionColor = HUD.SettingsData.KeyDirectionColor.Value.ColorToHtml();
                _Compass.AngleColor = HUD.SettingsData.KeyAngleColor.Value.ColorToHtml();

                _Compass.AzimuthsAngleStyles = HUD.SettingsData.KeyAzimuthsAngleStyles.Value;
                _Compass.DirectionStyles = HUD.SettingsData.KeyDirectionStyles.Value;
                _Compass.AngleStyles = HUD.SettingsData.KeyAngleStyles.Value;
            }
        }
#endif
    }
}
