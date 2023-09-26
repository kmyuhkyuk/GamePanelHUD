using UnityEngine;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore;
#endif

namespace GamePanelHUDLife
{
    public class GamePanelHUDLife : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;
        private static GamePanelHUDCorePlugin.HUDClass<GamePanelHUDLifePlugin.Life, GamePanelHUDLifePlugin.SettingsData>
            HUD => GamePanelHUDLifePlugin.HUD;
#endif

        [SerializeField] private GamePanelHUDLifeUI overallHealth;

        [SerializeField] private GamePanelHUDLifeUI poisoning;

        [SerializeField] private GamePanelHUDLifeUI radiation;

        [SerializeField] private GamePanelHUDLifeUI bloodPressure;

        [SerializeField] private GamePanelHUDLifeUI energy;

        [SerializeField] private GamePanelHUDLifeUI hydration;

        [SerializeField] private GamePanelHUDLifeUI temperature;

        [SerializeField] private GamePanelHUDLifeUI weight;

        private RectTransform _rectTransform;

#if !UNITY_EDITOR
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();

            HUDCore.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            LifeHUD();
        }

        private void LifeHUD()
        {
            //Set RectTransform anchoredPosition and sizeDelta and localScale
            _rectTransform.anchoredPosition = HUD.SetData.KeyAnchoredPosition.Value;
            _rectTransform.sizeDelta = HUD.SetData.KeySizeDelta.Value;
            _rectTransform.localScale = HUD.SetData.KeyLocalScale.Value;

            //Set Current and Maximum float

            #region overallHealth

            overallHealth.gameObject.SetActive(HUD.HUDSw);
            overallHealth.buffHUDSw = HUD.SetData.KeyBuffSw.Value;
            overallHealth.arrowAnimation = HUD.SetData.KeyArrowAnimation.Value;
            overallHealth.arrowAnimationReverse = HUD.SetData.KeyArrowAnimationReverse.Value;
            overallHealth.isHealth = true;
            overallHealth.atMaximum = HUD.Info.Health.Common.AtMaximum;

            overallHealth.current = HUD.Info.Health.Common.Current;
            overallHealth.maximum = HUD.Info.Health.Common.Maximum;
            overallHealth.buffRate = HUD.Info.Rates.HealthRate;
            overallHealth.normalized = HUD.Info.Health.Common.Current / HUD.Info.Health.Common.Maximum;
            overallHealth.warningRate = HUD.SetData.KeyHealthWarningRate.Value / 100f;
            overallHealth.buffSpeed = HUD.SetData.KeyBuffSpeed.Value;

            overallHealth.upBuffArrowColor = HUD.SetData.KeyUpBuffArrowColor.Value;
            overallHealth.downBuffArrowColor = HUD.SetData.KeyDownBuffArrowColor.Value;
            overallHealth.currentColor = HUD.SetData.KeyHealthColor.Value;
            overallHealth.maxColor = HUD.SetData.KeyMaxColor.Value;
            overallHealth.addZerosColor = HUD.SetData.KeyAddZerosColor.Value;
            overallHealth.warningColor = HUD.SetData.KeyWarningColor.Value;
            overallHealth.upBuffColor = HUD.SetData.KeyUpBuffColor.Value;
            overallHealth.downBuffColor = HUD.SetData.KeyDownBuffColor.Value;

            overallHealth.currentStyles = HUD.SetData.KeyCurrentStyles.Value;
            overallHealth.maximumStyles = HUD.SetData.KeyMaximumStyles.Value;

            #endregion

            #region hydration

            hydration.gameObject.SetActive(HUD.HUDSw);
            hydration.buffHUDSw = HUD.SetData.KeyBuffSw.Value;
            hydration.arrowAnimation = HUD.SetData.KeyArrowAnimation.Value;
            hydration.arrowAnimationReverse = HUD.SetData.KeyArrowAnimationReverse.Value;
            hydration.isHealth = false;
            hydration.atMaximum = HUD.Info.Hydrations.AtMaximum;

            hydration.current = HUD.Info.Hydrations.Current;
            hydration.maximum = HUD.Info.Hydrations.Maximum;
            hydration.buffRate = HUD.Info.Rates.HydrationRate;
            hydration.normalized = HUD.Info.Hydrations.Normalized;
            hydration.warningRate = HUD.SetData.KeyHydrationWarningRate.Value / 100f;
            hydration.buffSpeed = HUD.SetData.KeyBuffSpeed.Value;

            hydration.upBuffArrowColor = HUD.SetData.KeyUpBuffArrowColor.Value;
            hydration.downBuffArrowColor = HUD.SetData.KeyDownBuffArrowColor.Value;
            hydration.currentColor = HUD.SetData.KeyHydrationColor.Value;
            hydration.maxColor = HUD.SetData.KeyMaxColor.Value;
            hydration.addZerosColor = HUD.SetData.KeyAddZerosColor.Value;
            hydration.warningColor = HUD.SetData.KeyWarningColor.Value;
            hydration.upBuffColor = HUD.SetData.KeyUpBuffColor.Value;
            hydration.downBuffColor = HUD.SetData.KeyDownBuffColor.Value;

            hydration.currentStyles = HUD.SetData.KeyCurrentStyles.Value;
            hydration.maximumStyles = HUD.SetData.KeyMaximumStyles.Value;

            #endregion

            #region energy

            energy.gameObject.SetActive(HUD.HUDSw);
            energy.buffHUDSw = HUD.SetData.KeyBuffSw.Value;
            energy.arrowAnimation = HUD.SetData.KeyArrowAnimation.Value;
            energy.arrowAnimationReverse = HUD.SetData.KeyArrowAnimationReverse.Value;
            energy.isHealth = false;
            energy.atMaximum = HUD.Info.Energys.AtMaximum;

            energy.current = HUD.Info.Energys.Current;
            energy.maximum = HUD.Info.Energys.Maximum;
            energy.buffRate = HUD.Info.Rates.EnergyRate;
            energy.normalized = HUD.Info.Energys.Normalized;
            energy.warningRate = HUD.SetData.KeyEnergyWarningRate.Value / 100f;
            energy.buffSpeed = HUD.SetData.KeyBuffSpeed.Value;

            energy.upBuffArrowColor = HUD.SetData.KeyUpBuffArrowColor.Value;
            energy.downBuffArrowColor = HUD.SetData.KeyDownBuffArrowColor.Value;
            energy.currentColor = HUD.SetData.KeyEnergyColor.Value;
            energy.maxColor = HUD.SetData.KeyMaxColor.Value;
            energy.addZerosColor = HUD.SetData.KeyAddZerosColor.Value;
            energy.warningColor = HUD.SetData.KeyWarningColor.Value;
            energy.upBuffColor = HUD.SetData.KeyUpBuffColor.Value;
            energy.downBuffColor = HUD.SetData.KeyDownBuffColor.Value;

            energy.currentStyles = HUD.SetData.KeyCurrentStyles.Value;
            energy.maximumStyles = HUD.SetData.KeyMaximumStyles.Value;

            #endregion
        }
#endif
    }
}