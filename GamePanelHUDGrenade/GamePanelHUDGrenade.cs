using UnityEngine;
using EFT.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDGrenade
{
    public class GamePanelHUDGrenade : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDGrenadePlugin.GrenadeAmount, GamePanelHUDGrenadePlugin.SettingsData> HUD => GamePanelHUDGrenadePlugin.HUD;
#endif

        [SerializeField]
        private GamePanelHUDGrenadeUI _FragAmount;

        [SerializeField]
        private GamePanelHUDGrenadeUI _StunAmount;

        [SerializeField]
        private GamePanelHUDGrenadeUI _SmokeAmount;

#if !UNITY_EDITOR
        void Start()
        {
            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            GrenadeHUD();
        }

        void GrenadeHUD()
        {
            //Set RectTransform anchoredPosition and localScale
            RectTransform.anchoredPosition = HUD.SetData.KeyAnchoredPosition.Value;
            RectTransform.sizeDelta = HUD.SetData.KeySizeDelta.Value;
            RectTransform.localScale = HUD.SetData.KeyLocalScale.Value;

            if (_FragAmount != null)
            {
                _FragAmount.gameObject.SetActive(HUD.HUDSw);
                _FragAmount.ZeroWarning = HUD.SetData.KeyZeroWarning.Value;

                _FragAmount.GrenadeAmount = HUD.Info.Frag;

                _FragAmount.GrenadeColor = HUD.SetData.KeyFragColor.Value;
                _FragAmount.WarningColor = HUD.SetData.KeyWarningColor.Value;

                _FragAmount.GrenadeStyles = HUD.SetData.KeyFragStyles.Value;
            }
            if (_StunAmount != null)
            {
                _StunAmount.gameObject.SetActive(HUD.HUDSw && !HUD.SetData.KeyMergeGrenade.Value);
                _StunAmount.ZeroWarning = HUD.SetData.KeyZeroWarning.Value;

                _StunAmount.GrenadeAmount = HUD.Info.Stun + HUD.Info.Flash;

                _StunAmount.GrenadeColor = HUD.SetData.KeyStunColor.Value;
                _StunAmount.WarningColor = HUD.SetData.KeyWarningColor.Value;

                _StunAmount.GrenadeStyles = HUD.SetData.KeyStunStyles.Value;
            }
            if (_SmokeAmount != null)
            {
                _SmokeAmount.gameObject.SetActive(HUD.HUDSw && !HUD.SetData.KeyMergeGrenade.Value);
                _SmokeAmount.ZeroWarning = HUD.SetData.KeyZeroWarning.Value;

                _SmokeAmount.GrenadeAmount = HUD.Info.Smoke;

                _SmokeAmount.GrenadeColor = HUD.SetData.KeySmokeColor.Value;
                _SmokeAmount.WarningColor = HUD.SetData.KeyWarningColor.Value;

                _SmokeAmount.GrenadeStyles = HUD.SetData.KeySmokeStyles.Value;
            }
        }
#endif
    }
}
