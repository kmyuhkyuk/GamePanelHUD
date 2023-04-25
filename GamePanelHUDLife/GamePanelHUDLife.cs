using UnityEngine;
using EFT.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDLife
{
    public class GamePanelHUDLife : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDLifePlugin.Life, GamePanelHUDLifePlugin.SettingsData> HUD 
        { 
            get
            {
                return GamePanelHUDLifePlugin.HUD;
            }
        }
#endif

        [SerializeField]
        private GamePanelHUDLifeUI _OverallHealth;

        [SerializeField]
        private GamePanelHUDLifeUI _Poisoning;

        [SerializeField]
        private GamePanelHUDLifeUI _Radiation;

        [SerializeField]
        private GamePanelHUDLifeUI _BloodPressure;

        [SerializeField]
        private GamePanelHUDLifeUI _Energy;

        [SerializeField]
        private GamePanelHUDLifeUI _Hydration;

        [SerializeField]
        private GamePanelHUDLifeUI _Temperature;

        [SerializeField]
        private GamePanelHUDLifeUI _Weight;

#if !UNITY_EDITOR
        void Start()
        {
            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            LifeHUD();
        }

        void LifeHUD()
        {
            //Set RectTransform anchoredPosition and sizeDelta and localScale
            RectTransform.anchoredPosition = HUD.SettingsData.KeyAnchoredPosition.Value;
            RectTransform.sizeDelta = HUD.SettingsData.KeySizeDelta.Value;
            RectTransform.localScale = HUD.SettingsData.KeyLocalScale.Value;

            //Set Curren and Maximum float
            if (_OverallHealth != null)
            {
                _OverallHealth.gameObject.SetActive(HUD.HUDSW);
                _OverallHealth.BuffHUDSW = HUD.SettingsData.KeyBuffSw.Value;
                _OverallHealth.ArrowAnimation = HUD.SettingsData.KeyArrowAnimation.Value;
                _OverallHealth.ArrowAnimationReverse = HUD.SettingsData.KeyArrowAnimationReverse.Value;
                _OverallHealth.IsHealth = true;
                _OverallHealth.AtMaximum = HUD.Info.Healths.Common.AtMaximum;

                _OverallHealth.Current = HUD.Info.Healths.Common.Current;
                _OverallHealth.Maximum = HUD.Info.Healths.Common.Maximum;
                _OverallHealth.BuffRate = HUD.Info.Rates.HealthRate;
                _OverallHealth.Normalized = HUD.Info.Healths.Common.Current / HUD.Info.Healths.Common.Maximum;
                _OverallHealth.WarningRate = (float)HUD.SettingsData.KeyHealthWarningRate.Value / (float)100;
                _OverallHealth.BuffSpeed = HUD.SettingsData.KeyBuffSpeed.Value;

                _OverallHealth.UpBuffArrowColor = HUD.SettingsData.KeyUpBuffArrowColor.Value;
                _OverallHealth.DownBuffArrowColor = HUD.SettingsData.KeyDownBuffArrowColor.Value;
                _OverallHealth.CurrentColor = HUD.SettingsData.KeyHealthColor.Value;
                _OverallHealth.MaxColor = HUD.SettingsData.KeyMaxColor.Value;
                _OverallHealth.AddZerosColor = HUD.SettingsData.KeyAddZerosColor.Value;
                _OverallHealth.WarningColor = HUD.SettingsData.KeyWarningColor.Value;
                _OverallHealth.UpBuffColor = HUD.SettingsData.KeyUpBuffColor.Value;
                _OverallHealth.DownBuffColor = HUD.SettingsData.KeyDownBuffColor.Value;

                _OverallHealth.CurrentStyles = HUD.SettingsData.KeyCurrentStyles.Value;
                _OverallHealth.MaximumStyles = HUD.SettingsData.KeyMaximumStyles.Value;
            }
            if (_Hydration != null)
            {
                _Hydration.gameObject.SetActive(HUD.HUDSW);
                _Hydration.BuffHUDSW = HUD.SettingsData.KeyBuffSw.Value;
                _Hydration.ArrowAnimation = HUD.SettingsData.KeyArrowAnimation.Value;
                _Hydration.ArrowAnimationReverse = HUD.SettingsData.KeyArrowAnimationReverse.Value;
                _Hydration.IsHealth = false;
                _Hydration.AtMaximum = HUD.Info.Hydrations.AtMaximum;

                _Hydration.Current = HUD.Info.Hydrations.Current;
                _Hydration.Maximum = HUD.Info.Hydrations.Maximum;
                _Hydration.BuffRate = HUD.Info.Rates.HydrationRate;
                _Hydration.Normalized = HUD.Info.Hydrations.Normalized;
                _Hydration.WarningRate = (float)HUD.SettingsData.KeyHydrationWarningRate.Value / (float)100;
                _Hydration.BuffSpeed = HUD.SettingsData.KeyBuffSpeed.Value;

                _Hydration.UpBuffArrowColor = HUD.SettingsData.KeyUpBuffArrowColor.Value;
                _Hydration.DownBuffArrowColor = HUD.SettingsData.KeyDownBuffArrowColor.Value;
                _Hydration.CurrentColor = HUD.SettingsData.KeyHydrationColor.Value;
                _Hydration.MaxColor = HUD.SettingsData.KeyMaxColor.Value;
                _Hydration.AddZerosColor = HUD.SettingsData.KeyAddZerosColor.Value;
                _Hydration.WarningColor = HUD.SettingsData.KeyWarningColor.Value;
                _Hydration.UpBuffColor = HUD.SettingsData.KeyUpBuffColor.Value;
                _Hydration.DownBuffColor = HUD.SettingsData.KeyDownBuffColor.Value;

                _Hydration.CurrentStyles = HUD.SettingsData.KeyCurrentStyles.Value;
                _Hydration.MaximumStyles = HUD.SettingsData.KeyMaximumStyles.Value;
            }
            if (_Energy != null)
            {
                _Energy.gameObject.SetActive(HUD.HUDSW);
                _Energy.BuffHUDSW = HUD.SettingsData.KeyBuffSw.Value;
                _Energy.ArrowAnimation = HUD.SettingsData.KeyArrowAnimation.Value;
                _Energy.ArrowAnimationReverse = HUD.SettingsData.KeyArrowAnimationReverse.Value;
                _Energy.IsHealth = false;
                _Energy.AtMaximum = HUD.Info.Energys.AtMaximum;

                _Energy.Current = HUD.Info.Energys.Current;
                _Energy.Maximum = HUD.Info.Energys.Maximum;
                _Energy.BuffRate = HUD.Info.Rates.EnergyRate;
                _Energy.Normalized = HUD.Info.Energys.Normalized;
                _Energy.WarningRate = (float)HUD.SettingsData.KeyEnergyWarningRate.Value / (float)100;
                _Energy.BuffSpeed = HUD.SettingsData.KeyBuffSpeed.Value;

                _Energy.UpBuffArrowColor = HUD.SettingsData.KeyUpBuffArrowColor.Value;
                _Energy.DownBuffArrowColor = HUD.SettingsData.KeyDownBuffArrowColor.Value;
                _Energy.CurrentColor = HUD.SettingsData.KeyEnergyColor.Value;
                _Energy.MaxColor = HUD.SettingsData.KeyMaxColor.Value;
                _Energy.AddZerosColor = HUD.SettingsData.KeyAddZerosColor.Value;
                _Energy.WarningColor = HUD.SettingsData.KeyWarningColor.Value;
                _Energy.UpBuffColor = HUD.SettingsData.KeyUpBuffColor.Value;
                _Energy.DownBuffColor = HUD.SettingsData.KeyDownBuffColor.Value;

                _Energy.CurrentStyles = HUD.SettingsData.KeyCurrentStyles.Value;
                _Energy.MaximumStyles = HUD.SettingsData.KeyMaximumStyles.Value;
            }
        }
#endif
    }
}
