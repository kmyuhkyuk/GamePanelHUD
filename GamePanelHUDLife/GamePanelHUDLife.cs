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
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDLifePlugin.Life, GamePanelHUDLifePlugin.SettingsData> HUD => GamePanelHUDLifePlugin.HUD;
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
            RectTransform.anchoredPosition = HUD.SetData.KeyAnchoredPosition.Value;
            RectTransform.sizeDelta = HUD.SetData.KeySizeDelta.Value;
            RectTransform.localScale = HUD.SetData.KeyLocalScale.Value;

            //Set Current and Maximum float
            if (_OverallHealth != null)
            {
                _OverallHealth.gameObject.SetActive(HUD.HUDSw);
                _OverallHealth.BuffHUDSw = HUD.SetData.KeyBuffSw.Value;
                _OverallHealth.ArrowAnimation = HUD.SetData.KeyArrowAnimation.Value;
                _OverallHealth.ArrowAnimationReverse = HUD.SetData.KeyArrowAnimationReverse.Value;
                _OverallHealth.IsHealth = true;
                _OverallHealth.AtMaximum = HUD.Info.Healths.Common.AtMaximum;

                _OverallHealth.Current = HUD.Info.Healths.Common.Current;
                _OverallHealth.Maximum = HUD.Info.Healths.Common.Maximum;
                _OverallHealth.BuffRate = HUD.Info.Rates.HealthRate;
                _OverallHealth.Normalized = HUD.Info.Healths.Common.Current / HUD.Info.Healths.Common.Maximum;
                _OverallHealth.WarningRate = HUD.SetData.KeyHealthWarningRate.Value / 100f;
                _OverallHealth.BuffSpeed = HUD.SetData.KeyBuffSpeed.Value;

                _OverallHealth.UpBuffArrowColor = HUD.SetData.KeyUpBuffArrowColor.Value;
                _OverallHealth.DownBuffArrowColor = HUD.SetData.KeyDownBuffArrowColor.Value;
                _OverallHealth.CurrentColor = HUD.SetData.KeyHealthColor.Value;
                _OverallHealth.MaxColor = HUD.SetData.KeyMaxColor.Value;
                _OverallHealth.AddZerosColor = HUD.SetData.KeyAddZerosColor.Value;
                _OverallHealth.WarningColor = HUD.SetData.KeyWarningColor.Value;
                _OverallHealth.UpBuffColor = HUD.SetData.KeyUpBuffColor.Value;
                _OverallHealth.DownBuffColor = HUD.SetData.KeyDownBuffColor.Value;

                _OverallHealth.CurrentStyles = HUD.SetData.KeyCurrentStyles.Value;
                _OverallHealth.MaximumStyles = HUD.SetData.KeyMaximumStyles.Value;
            }
            if (_Hydration != null)
            {
                _Hydration.gameObject.SetActive(HUD.HUDSw);
                _Hydration.BuffHUDSw = HUD.SetData.KeyBuffSw.Value;
                _Hydration.ArrowAnimation = HUD.SetData.KeyArrowAnimation.Value;
                _Hydration.ArrowAnimationReverse = HUD.SetData.KeyArrowAnimationReverse.Value;
                _Hydration.IsHealth = false;
                _Hydration.AtMaximum = HUD.Info.Hydrations.AtMaximum;

                _Hydration.Current = HUD.Info.Hydrations.Current;
                _Hydration.Maximum = HUD.Info.Hydrations.Maximum;
                _Hydration.BuffRate = HUD.Info.Rates.HydrationRate;
                _Hydration.Normalized = HUD.Info.Hydrations.Normalized;
                _Hydration.WarningRate = HUD.SetData.KeyHydrationWarningRate.Value / 100f;
                _Hydration.BuffSpeed = HUD.SetData.KeyBuffSpeed.Value;

                _Hydration.UpBuffArrowColor = HUD.SetData.KeyUpBuffArrowColor.Value;
                _Hydration.DownBuffArrowColor = HUD.SetData.KeyDownBuffArrowColor.Value;
                _Hydration.CurrentColor = HUD.SetData.KeyHydrationColor.Value;
                _Hydration.MaxColor = HUD.SetData.KeyMaxColor.Value;
                _Hydration.AddZerosColor = HUD.SetData.KeyAddZerosColor.Value;
                _Hydration.WarningColor = HUD.SetData.KeyWarningColor.Value;
                _Hydration.UpBuffColor = HUD.SetData.KeyUpBuffColor.Value;
                _Hydration.DownBuffColor = HUD.SetData.KeyDownBuffColor.Value;

                _Hydration.CurrentStyles = HUD.SetData.KeyCurrentStyles.Value;
                _Hydration.MaximumStyles = HUD.SetData.KeyMaximumStyles.Value;
            }
            if (_Energy != null)
            {
                _Energy.gameObject.SetActive(HUD.HUDSw);
                _Energy.BuffHUDSw = HUD.SetData.KeyBuffSw.Value;
                _Energy.ArrowAnimation = HUD.SetData.KeyArrowAnimation.Value;
                _Energy.ArrowAnimationReverse = HUD.SetData.KeyArrowAnimationReverse.Value;
                _Energy.IsHealth = false;
                _Energy.AtMaximum = HUD.Info.Energys.AtMaximum;

                _Energy.Current = HUD.Info.Energys.Current;
                _Energy.Maximum = HUD.Info.Energys.Maximum;
                _Energy.BuffRate = HUD.Info.Rates.EnergyRate;
                _Energy.Normalized = HUD.Info.Energys.Normalized;
                _Energy.WarningRate = HUD.SetData.KeyEnergyWarningRate.Value / 100f;
                _Energy.BuffSpeed = HUD.SetData.KeyBuffSpeed.Value;

                _Energy.UpBuffArrowColor = HUD.SetData.KeyUpBuffArrowColor.Value;
                _Energy.DownBuffArrowColor = HUD.SetData.KeyDownBuffArrowColor.Value;
                _Energy.CurrentColor = HUD.SetData.KeyEnergyColor.Value;
                _Energy.MaxColor = HUD.SetData.KeyMaxColor.Value;
                _Energy.AddZerosColor = HUD.SetData.KeyAddZerosColor.Value;
                _Energy.WarningColor = HUD.SetData.KeyWarningColor.Value;
                _Energy.UpBuffColor = HUD.SetData.KeyUpBuffColor.Value;
                _Energy.DownBuffColor = HUD.SetData.KeyDownBuffColor.Value;

                _Energy.CurrentStyles = HUD.SetData.KeyCurrentStyles.Value;
                _Energy.MaximumStyles = HUD.SetData.KeyMaximumStyles.Value;
            }
        }
#endif
    }
}
