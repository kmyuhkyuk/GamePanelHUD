using UnityEngine;
#if !UNITY_EDITOR
using KmyTarkovUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDWeapon.Models;
using SettingsModel = GamePanelHUDWeapon.Models.SettingsModel;

#endif

namespace GamePanelHUDWeapon.Views
{
    public class WeaponHUDView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        [SerializeField] private WeaponUIView weaponUIView;

        private RectTransform _rectTransform;

#if !UNITY_EDITOR

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();

            WeaponHUDModel.Instance.WeaponTrigger = WeaponTrigger;
        }

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var weaponHUDModel = WeaponHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            //Set RectTransform anchoredPosition and localScale
            _rectTransform.anchoredPosition = settingsModel.KeyAnchoredPosition.Value;
            _rectTransform.localScale = settingsModel.KeyLocalScale.Value;

            //Set Current Maximum Patron float
            weaponUIView.gameObject.SetActive(weaponHUDModel.WeaponHUDSw);
            weaponUIView.weaponNameAlways = weaponHUDModel.Weapon.WeaponNameAlways;
            weaponUIView.ammoTypeHUDSw = settingsModel.KeyAmmoTypeHUDSw.Value;
            weaponUIView.fireModeHUDSw = settingsModel.KeyFireModeHUDSw.Value;
            weaponUIView.zeroWarning = settingsModel.KeyZeroWarning.Value;

            weaponUIView.current = weaponHUDModel.Weapon.MagCount;
            weaponUIView.maximum = weaponHUDModel.Weapon.MagMaxCount;
            weaponUIView.patron = weaponHUDModel.Weapon.Patron;
            weaponUIView.normalized = weaponHUDModel.Weapon.Normalized;
            weaponUIView.weaponName = settingsModel.KeyWeaponShortName.Value
                ? weaponHUDModel.Weapon.WeaponShortName
                : weaponHUDModel.Weapon.WeaponName;
            weaponUIView.ammoType = weaponHUDModel.Weapon.AmmoType;
            weaponUIView.fireMode = weaponHUDModel.Weapon.FireMode;

            weaponUIView.warningRate10 = settingsModel.KeyWarningRate10.Value / 100f;
            weaponUIView.warningRate100 = settingsModel.KeyWarningRate100.Value / 100f;
            weaponUIView.weaponNameSpeed = settingsModel.KeyWeaponNameSpeed.Value;
            weaponUIView.zeroWarningSpeed = settingsModel.KeyZeroWarningSpeed.Value;

            weaponUIView.currentColor = settingsModel.KeyCurrentColor.Value;
            weaponUIView.maxColor = settingsModel.KeyMaxColor.Value;
            weaponUIView.patronColor = settingsModel.KeyPatronColor.Value;
            weaponUIView.weaponNameColor = settingsModel.KeyWeaponNameColor.Value;
            weaponUIView.ammoTypeColor = settingsModel.KeyAmmoTypeColor.Value;
            weaponUIView.fireModeColor = settingsModel.KeyFireModeColor.Value;
            weaponUIView.addZerosColor = settingsModel.KeyAddZerosColor.Value;
            weaponUIView.warningColor = settingsModel.KeyWarningColor.Value;

            weaponUIView.currentStyles = settingsModel.KeyCurrentStyles.Value;
            weaponUIView.maximumStyles = settingsModel.KeyMaximumStyles.Value;
            weaponUIView.patronStyles = settingsModel.KeyPatronStyles.Value;
            weaponUIView.weaponNameStyles = settingsModel.KeyWeaponNameStyles.Value;
            weaponUIView.ammoTypeStyles = settingsModel.KeyAmmoTypeStyles.Value;
            weaponUIView.fireModeStyles = settingsModel.KeyFireModeStyles.Value;
        }

        private void WeaponTrigger()
        {
            weaponUIView.weaponIsTrigger = true;
        }

#endif
    }
}