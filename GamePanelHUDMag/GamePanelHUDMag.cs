using UnityEngine;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore;
#endif

namespace GamePanelHUDMag
{
    public class GamePanelHUDMag : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;
        private static
            GamePanelHUDCorePlugin.HUDClass<GamePanelHUDMagPlugin.WeaponData, GamePanelHUDMagPlugin.SettingsData> HUD =>
            GamePanelHUDMagPlugin.HUD;
#endif

        [SerializeField] private GamePanelHUDMagUI mag;

        private RectTransform _rectTransform;

#if !UNITY_EDITOR
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();

            GamePanelHUDMagPlugin.WeaponTrigger = WeaponTrigger;

            HUDCore.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            MagHUD();
        }

        private void MagHUD()
        {
            //Set RectTransform anchoredPosition and localScale
            _rectTransform.anchoredPosition = HUD.SetData.KeyAnchoredPosition.Value;
            _rectTransform.localScale = HUD.SetData.KeyLocalScale.Value;

            //Set Current Maximum Patron float
            mag.gameObject.SetActive(HUD.HUDSw);
            mag.weaponNameAlways = HUD.Info.WeaponNameAlways;
            mag.ammoTypeHUDSw = HUD.SetData.KeyAmmoTypeHUDSw.Value;
            mag.fireModeHUDSw = HUD.SetData.KeyFireModeHUDSw.Value;
            mag.zeroWarning = HUD.SetData.KeyZeroWarning.Value;

            mag.current = HUD.Info.MagCount;
            mag.maximum = HUD.Info.MagMaxCount;
            mag.patron = HUD.Info.Patron;
            mag.normalized = HUD.Info.Normalized;
            mag.weaponName = HUD.SetData.KeyWeaponShortName.Value ? HUD.Info.WeaponShortName : HUD.Info.WeaponName;
            mag.ammoType = HUD.Info.AmmoType;
            mag.fireMode = HUD.Info.FireMode;

            mag.warningRate10 = HUD.SetData.KeyWarningRate10.Value / 100f;
            mag.warningRate100 = HUD.SetData.KeyWarningRate100.Value / 100f;
            mag.weaponNameSpeed = HUD.SetData.KeyWeaponNameSpeed.Value;
            mag.zeroWarningSpeed = HUD.SetData.KeyZeroWarningSpeed.Value;

            mag.currentColor = HUD.SetData.KeyCurrentColor.Value;
            mag.maxColor = HUD.SetData.KeyMaxColor.Value;
            mag.patronColor = HUD.SetData.KeyPatronColor.Value;
            mag.weaponNameColor = HUD.SetData.KeyWeaponNameColor.Value;
            mag.ammoTypeColor = HUD.SetData.KeyAmmoTypeColor.Value;
            mag.fireModeColor = HUD.SetData.KeyFireModeColor.Value;
            mag.addZerosColor = HUD.SetData.KeyAddZerosColor.Value;
            mag.warningColor = HUD.SetData.KeyWarningColor.Value;

            mag.currentStyles = HUD.SetData.KeyCurrentStyles.Value;
            mag.maximumStyles = HUD.SetData.KeyMaximumStyles.Value;
            mag.patronStyles = HUD.SetData.KeyPatronStyles.Value;
            mag.weaponNameStyles = HUD.SetData.KeyWeaponNameStyles.Value;
            mag.ammoTypeStyles = HUD.SetData.KeyAmmoTypeStyles.Value;
            mag.fireModeStyles = HUD.SetData.KeyFireModeStyles.Value;
        }

        private void WeaponTrigger()
        {
            mag.weaponTrigger = true;
        }
#endif
    }
}