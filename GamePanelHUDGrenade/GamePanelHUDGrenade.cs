using UnityEngine;
#if !UNITY_EDITOR

using EFTUtils;
using GamePanelHUDCore;

#endif

namespace GamePanelHUDGrenade
{
    public class GamePanelHUDGrenade : MonoBehaviour
#if !UNITY_EDITOR

        , IUpdate

#endif
    {
#if !UNITY_EDITOR

        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;
        private static
            GamePanelHUDCorePlugin.HUDClass<GamePanelHUDGrenadePlugin.GrenadeAmount,
                GamePanelHUDGrenadePlugin.SettingsData> HUD => GamePanelHUDGrenadePlugin.HUD;

#endif

        [SerializeField] private GamePanelHUDGrenadeUI fragAmount;

        [SerializeField] private GamePanelHUDGrenadeUI stunAmount;

        [SerializeField] private GamePanelHUDGrenadeUI smokeAmount;

        private RectTransform _rectTransform;

#if !UNITY_EDITOR

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();

            HUDCore.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            GrenadeHUD();
        }

        private void GrenadeHUD()
        {
            //Set RectTransform anchoredPosition and localScale
            _rectTransform.anchoredPosition = HUD.SetData.KeyAnchoredPosition.Value;
            _rectTransform.sizeDelta = HUD.SetData.KeySizeDelta.Value;
            _rectTransform.localScale = HUD.SetData.KeyLocalScale.Value;

            #region fragAmount

            fragAmount.gameObject.SetActive(HUD.HUDSw);
            fragAmount.zeroWarning = HUD.SetData.KeyZeroWarning.Value;

            fragAmount.grenadeAmount = HUD.Info.Frag;

            fragAmount.grenadeColor = HUD.SetData.KeyFragColor.Value;
            fragAmount.warningColor = HUD.SetData.KeyWarningColor.Value;

            fragAmount.grenadeStyles = HUD.SetData.KeyFragStyles.Value;

            #endregion

            #region stunAmount

            stunAmount.gameObject.SetActive(HUD.HUDSw && !HUD.SetData.KeyMergeGrenade.Value);
            stunAmount.zeroWarning = HUD.SetData.KeyZeroWarning.Value;

            stunAmount.grenadeAmount = HUD.Info.Stun + HUD.Info.Flash;

            stunAmount.grenadeColor = HUD.SetData.KeyStunColor.Value;
            stunAmount.warningColor = HUD.SetData.KeyWarningColor.Value;

            stunAmount.grenadeStyles = HUD.SetData.KeyStunStyles.Value;

            #endregion

            #region smokeAmount

            smokeAmount.gameObject.SetActive(HUD.HUDSw && !HUD.SetData.KeyMergeGrenade.Value);
            smokeAmount.zeroWarning = HUD.SetData.KeyZeroWarning.Value;

            smokeAmount.grenadeAmount = HUD.Info.Smoke;

            smokeAmount.grenadeColor = HUD.SetData.KeySmokeColor.Value;
            smokeAmount.warningColor = HUD.SetData.KeyWarningColor.Value;

            smokeAmount.grenadeStyles = HUD.SetData.KeySmokeStyles.Value;

            #endregion
        }

#endif
    }
}