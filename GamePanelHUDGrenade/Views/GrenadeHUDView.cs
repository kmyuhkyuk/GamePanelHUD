using UnityEngine;
#if !UNITY_EDITOR
using SettingsModel = GamePanelHUDGrenade.Models.SettingsModel;
using EFTUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDGrenade.Models;

#endif

namespace GamePanelHUDGrenade.Views
{
    public class GrenadeHUDView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        [SerializeField] private GrenadeUIView fragUIView;

        [SerializeField] private GrenadeUIView stunUIView;

        [SerializeField] private GrenadeUIView smokeUIView;

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
            var settingsModel = SettingsModel.Instance;
            var grenadeHUDModel = GrenadeHUDModel.Instance;

            //Set RectTransform anchoredPosition and localScale
            _rectTransform.anchoredPosition = settingsModel.KeyAnchoredPosition.Value;
            _rectTransform.sizeDelta = settingsModel.KeySizeDelta.Value;
            _rectTransform.localScale = settingsModel.KeyLocalScale.Value;

            #region fragView

            fragUIView.gameObject.SetActive(GrenadeHUDModel.Instance.GrenadeHUDSw);
            fragUIView.zeroWarning = settingsModel.KeyZeroWarning.Value;

            fragUIView.grenadeAmount = grenadeHUDModel.AllAmount.Frag;

            fragUIView.grenadeColor = settingsModel.KeyFragColor.Value;
            fragUIView.warningColor = settingsModel.KeyWarningColor.Value;

            fragUIView.grenadeStyles = settingsModel.KeyFragStyles.Value;

            #endregion

            #region stunView

            stunUIView.gameObject.SetActive(grenadeHUDModel.GrenadeHUDSw && !settingsModel.KeyMergeGrenade.Value);
            stunUIView.zeroWarning = settingsModel.KeyZeroWarning.Value;

            stunUIView.grenadeAmount = grenadeHUDModel.AllAmount.Stun + grenadeHUDModel.AllAmount.Flash;

            stunUIView.grenadeColor = settingsModel.KeyStunColor.Value;
            stunUIView.warningColor = settingsModel.KeyWarningColor.Value;

            stunUIView.grenadeStyles = settingsModel.KeyStunStyles.Value;

            #endregion

            #region smokeView

            smokeUIView.gameObject.SetActive(grenadeHUDModel.GrenadeHUDSw && !settingsModel.KeyMergeGrenade.Value);
            smokeUIView.zeroWarning = settingsModel.KeyZeroWarning.Value;

            smokeUIView.grenadeAmount = grenadeHUDModel.AllAmount.Smoke;

            smokeUIView.grenadeColor = settingsModel.KeySmokeColor.Value;
            smokeUIView.warningColor = settingsModel.KeyWarningColor.Value;

            smokeUIView.grenadeStyles = settingsModel.KeySmokeStyles.Value;

            #endregion
        }

#endif
    }
}