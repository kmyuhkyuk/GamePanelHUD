using UnityEngine;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDHealth.Models;
using SettingsModel = GamePanelHUDHealth.Models.SettingsModel;

#endif

namespace GamePanelHUDHealth.Views
{
    public class HealthHUDView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        [SerializeField] private HealthUIView overallHealth;

        //[SerializeField] private GamePanelHUDHealthUI poisoning;

        //[SerializeField] private GamePanelHUDHealthUI radiation;

        //[SerializeField] private GamePanelHUDHealthUI bloodPressure;

        [SerializeField] private HealthUIView energy;

        [SerializeField] private HealthUIView hydration;

        //[SerializeField] private GamePanelHUDHealthUI temperature;

        //[SerializeField] private GamePanelHUDHealthUI weight;

        private RectTransform _rectTransform;

#if !UNITY_EDITOR
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var healthHUDModel = HealthHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            //Set RectTransform anchoredPosition and sizeDelta and localScale
            _rectTransform.anchoredPosition = settingsModel.KeyAnchoredPosition.Value;
            _rectTransform.sizeDelta = settingsModel.KeySizeDelta.Value;
            _rectTransform.localScale = settingsModel.KeyLocalScale.Value;

            //Set Current and Maximum float

            #region overallHealth

            overallHealth.gameObject.SetActive(healthHUDModel.HealthHUDSw);
            overallHealth.buffHUDSw = settingsModel.KeyBuffSw.Value;
            overallHealth.arrowAnimation = settingsModel.KeyArrowAnimation.Value;
            overallHealth.arrowAnimationReverse = settingsModel.KeyArrowAnimationReverse.Value;
            overallHealth.isHealth = true;
            overallHealth.atMaximum = healthHUDModel.Health.Common.AtMaximum;

            overallHealth.current = healthHUDModel.Health.Common.Current;
            overallHealth.maximum = healthHUDModel.Health.Common.Maximum;
            overallHealth.buffRate = healthHUDModel.Health.HealthRate;
            overallHealth.normalized = healthHUDModel.Health.Common.Current / healthHUDModel.Health.Common.Maximum;
            overallHealth.warningRate = settingsModel.KeyHealthWarningRate.Value / 100f;
            overallHealth.buffSpeed = settingsModel.KeyBuffSpeed.Value;

            overallHealth.upBuffArrowColor = settingsModel.KeyUpBuffArrowColor.Value;
            overallHealth.downBuffArrowColor = settingsModel.KeyDownBuffArrowColor.Value;
            overallHealth.currentColor = settingsModel.KeyHealthColor.Value;
            overallHealth.maxColor = settingsModel.KeyMaxColor.Value;
            overallHealth.addZerosColor = settingsModel.KeyAddZerosColor.Value;
            overallHealth.warningColor = settingsModel.KeyWarningColor.Value;
            overallHealth.upBuffColor = settingsModel.KeyUpBuffColor.Value;
            overallHealth.downBuffColor = settingsModel.KeyDownBuffColor.Value;

            overallHealth.currentStyles = settingsModel.KeyCurrentStyles.Value;
            overallHealth.maximumStyles = settingsModel.KeyMaximumStyles.Value;

            #endregion

            #region hydration

            hydration.gameObject.SetActive(healthHUDModel.HealthHUDSw);
            hydration.buffHUDSw = settingsModel.KeyBuffSw.Value;
            hydration.arrowAnimation = settingsModel.KeyArrowAnimation.Value;
            hydration.arrowAnimationReverse = settingsModel.KeyArrowAnimationReverse.Value;
            hydration.isHealth = false;
            hydration.atMaximum = healthHUDModel.Health.Hydration.AtMaximum;

            hydration.current = healthHUDModel.Health.Hydration.Current;
            hydration.maximum = healthHUDModel.Health.Hydration.Maximum;
            hydration.buffRate = healthHUDModel.Health.HydrationRate;
            hydration.normalized = healthHUDModel.Health.Hydration.Normalized;
            hydration.warningRate = settingsModel.KeyHydrationWarningRate.Value / 100f;
            hydration.buffSpeed = settingsModel.KeyBuffSpeed.Value;

            hydration.upBuffArrowColor = settingsModel.KeyUpBuffArrowColor.Value;
            hydration.downBuffArrowColor = settingsModel.KeyDownBuffArrowColor.Value;
            hydration.currentColor = settingsModel.KeyHydrationColor.Value;
            hydration.maxColor = settingsModel.KeyMaxColor.Value;
            hydration.addZerosColor = settingsModel.KeyAddZerosColor.Value;
            hydration.warningColor = settingsModel.KeyWarningColor.Value;
            hydration.upBuffColor = settingsModel.KeyUpBuffColor.Value;
            hydration.downBuffColor = settingsModel.KeyDownBuffColor.Value;

            hydration.currentStyles = settingsModel.KeyCurrentStyles.Value;
            hydration.maximumStyles = settingsModel.KeyMaximumStyles.Value;

            #endregion

            #region energy

            energy.gameObject.SetActive(healthHUDModel.HealthHUDSw);
            energy.buffHUDSw = settingsModel.KeyBuffSw.Value;
            energy.arrowAnimation = settingsModel.KeyArrowAnimation.Value;
            energy.arrowAnimationReverse = settingsModel.KeyArrowAnimationReverse.Value;
            energy.isHealth = false;
            energy.atMaximum = healthHUDModel.Health.Energy.AtMaximum;

            energy.current = healthHUDModel.Health.Energy.Current;
            energy.maximum = healthHUDModel.Health.Energy.Maximum;
            energy.buffRate = healthHUDModel.Health.EnergyRate;
            energy.normalized = healthHUDModel.Health.Energy.Normalized;
            energy.warningRate = settingsModel.KeyEnergyWarningRate.Value / 100f;
            energy.buffSpeed = settingsModel.KeyBuffSpeed.Value;

            energy.upBuffArrowColor = settingsModel.KeyUpBuffArrowColor.Value;
            energy.downBuffArrowColor = settingsModel.KeyDownBuffArrowColor.Value;
            energy.currentColor = settingsModel.KeyEnergyColor.Value;
            energy.maxColor = settingsModel.KeyMaxColor.Value;
            energy.addZerosColor = settingsModel.KeyAddZerosColor.Value;
            energy.warningColor = settingsModel.KeyWarningColor.Value;
            energy.upBuffColor = settingsModel.KeyUpBuffColor.Value;
            energy.downBuffColor = settingsModel.KeyDownBuffColor.Value;

            energy.currentStyles = settingsModel.KeyCurrentStyles.Value;
            energy.maximumStyles = settingsModel.KeyMaximumStyles.Value;

            #endregion
        }

#endif
    }
}