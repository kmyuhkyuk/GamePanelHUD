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
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassData, GamePanelHUDCompassPlugin.SettingsData> HUD => GamePanelHUDCompassPlugin.CompassHUD;
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
            RectTransform.anchoredPosition = HUD.SetData.KeyAnchoredPosition.Value;
            RectTransform.sizeDelta = HUD.Info.SizeDelta;
            RectTransform.localScale = HUD.SetData.KeyLocalScale.Value;

            if (_Compass != null)
            {
                _Compass.gameObject.SetActive(HUD.HUDSw);
                _Compass.AngleHUDSw = HUD.SetData.KeyAngleHUDSw.Value;

                _Compass.AngleNum = HUD.Info.Angle;
                _Compass.CompassX = HUD.Info.CompassX;

                _Compass.ArrowColor = HUD.SetData.KeyArrowColor.Value;
                _Compass.AzimuthsColor = HUD.SetData.KeyAzimuthsColor.Value;
                _Compass.DirectionColor = HUD.SetData.KeyDirectionColor.Value;
                _Compass.AngleColor = HUD.SetData.KeyAngleColor.Value;

                _Compass.AzimuthsAngleStyles = HUD.SetData.KeyAzimuthsAngleStyles.Value;
                _Compass.DirectionStyles = HUD.SetData.KeyDirectionStyles.Value;
                _Compass.AngleStyles = HUD.SetData.KeyAngleStyles.Value;
            }
        }
#endif
    }
}
