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
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDGrenadePlugin.GrenadeAmount, GamePanelHUDGrenadePlugin.SettingsData> HUD
        {
            get
            {
                return GamePanelHUDGrenadePlugin.HUD;
            }
        }
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
            RectTransform.anchoredPosition = HUD.SettingsData.KeyAnchoredPosition.Value;
            RectTransform.sizeDelta = HUD.SettingsData.KeySizeDelta.Value;
            RectTransform.localScale = HUD.SettingsData.KeyLocalScale.Value;

            if (_FragAmount != null)
            {
                _FragAmount.gameObject.SetActive(HUD.HUDSW);
                _FragAmount.ZeroWarning = HUD.SettingsData.KeyZeroWarning.Value;

                _FragAmount.GrenadeAmount = HUD.Info.Frag;

                _FragAmount.GrenadeColor = HUD.SettingsData.KeyFragColor.Value;
                _FragAmount.WarningColor = HUD.SettingsData.KeyWarningColor.Value;

                _FragAmount.GrenadeStyles = HUD.SettingsData.KeyFragStyles.Value;
            }
            if (_StunAmount != null)
            {
                _StunAmount.gameObject.SetActive(HUD.HUDSW && !HUD.SettingsData.KeyMergeGrenade.Value);
                _StunAmount.ZeroWarning = HUD.SettingsData.KeyZeroWarning.Value;

                _StunAmount.GrenadeAmount = HUD.Info.Stun + HUD.Info.Flash;

                _StunAmount.GrenadeColor = HUD.SettingsData.KeyStunColor.Value;
                _StunAmount.WarningColor = HUD.SettingsData.KeyWarningColor.Value;

                _StunAmount.GrenadeStyles = HUD.SettingsData.KeyStunStyles.Value;
            }
            if (_SmokeAmount != null)
            {
                _SmokeAmount.gameObject.SetActive(HUD.HUDSW && !HUD.SettingsData.KeyMergeGrenade.Value);
                _SmokeAmount.ZeroWarning = HUD.SettingsData.KeyZeroWarning.Value;

                _SmokeAmount.GrenadeAmount = HUD.Info.Smoke;

                _SmokeAmount.GrenadeColor = HUD.SettingsData.KeySmokeColor.Value;
                _SmokeAmount.WarningColor = HUD.SettingsData.KeyWarningColor.Value;

                _SmokeAmount.GrenadeStyles = HUD.SettingsData.KeySmokeStyles.Value;
            }
        }
#endif
    }
}
