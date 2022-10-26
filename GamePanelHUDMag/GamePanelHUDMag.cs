using UnityEngine;
using EFT.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
#endif

namespace GamePanelHUDMag
{
    public class GamePanelHUDMag : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDMagPlugin.WeaponData, GamePanelHUDMagPlugin.SettingsData> HUD
        {
            get
            {
                return GamePanelHUDMagPlugin.HUD;
            }
        }
#endif

        [SerializeField]
        private GamePanelHUDMagUI _Mag;

#if !UNITY_EDITOR
        void Start()
        {
            GamePanelHUDMagPlugin.WeaponTirgger = WeaponTirgger;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            MagHUD();
        }

        void MagHUD()
        {
            //Set RectTransform anchoredPosition and localScale
            RectTransform.anchoredPosition = HUD.SettingsData.KeyAnchoredPosition.Value;
            RectTransform.localScale = HUD.SettingsData.KeyLocalScale.Value;

            //Set Curren Maximum Patron float
            if (_Mag != null)
            {
                _Mag.gameObject.SetActive(HUD.HUDSW);
                _Mag.WeaponNameAlways = HUD.Info.WeaponNameAlways;
                _Mag.AmmoTypeHUDSW = HUD.SettingsData.KeyAmmoTypeHUDSW.Value;
                _Mag.FireModeHUDSW = HUD.SettingsData.KeyFireModeHUDSW.Value;
                _Mag.ZeroWarning = HUD.SettingsData.KeyZeroWarning.Value;

                _Mag.Current = HUD.Info.MagCount;
                _Mag.Maximum = HUD.Info.MagMaxCount;
                _Mag.Patron = HUD.Info.Patron;
                _Mag.Normalized = HUD.Info.Normalized;
                _Mag.WeaponName = HUD.SettingsData.KeyWeaponShortName.Value ? HUD.Info.WeaponShortName : HUD.Info.WeaponName;
                _Mag.AmmoType = HUD.Info.AmmonType;
                _Mag.FireMode = HUD.Info.FireMode;

                _Mag.WarningRate10 = (float)HUD.SettingsData.KeyWarningRate10.Value / (float)100;
                _Mag.WarningRate100 = (float)HUD.SettingsData.KeyWarningRate100.Value / (float)100;
                _Mag.WeaponNameSpeed = HUD.SettingsData.KeyWeaponNameSpeed.Value;
                _Mag.ZeroWarningSpeed = HUD.SettingsData.KeyZeroWarningSpeed.Value;

                _Mag.CurrentColor = HUD.SettingsData.KeyCurrentColor.Value.ColorToHtml();
                _Mag.MaxColor = HUD.SettingsData.KeyMaxColor.Value.ColorToHtml();
                _Mag.PatronColor = HUD.SettingsData.KeyPatronColor.Value.ColorToHtml();
                _Mag.WeaponNameColor = HUD.SettingsData.KeyWeaponNameColor.Value.ColorToHtml();
                _Mag.AmmoTypeColor = HUD.SettingsData.KeyAmmonTypeColor.Value.ColorToHtml();
                _Mag.FireModeColor = HUD.SettingsData.KeyFireModeColor.Value.ColorToHtml();
                _Mag.AddZerosColor = HUD.SettingsData.KeyAddZerosColor.Value.ColorToHtml();
                _Mag.WarningColor = HUD.SettingsData.KeyWarningColor.Value.ColorToHtml();

                _Mag.CurrentStyles = HUD.SettingsData.KeyCurrentStyles.Value;
                _Mag.MaximumStyles = HUD.SettingsData.KeyMaximumStyles.Value;
                _Mag.PatronStyles = HUD.SettingsData.KeyPatronStyles.Value;
                _Mag.WeaponNameStyles = HUD.SettingsData.KeyWeaponNameStyles.Value;
                _Mag.AmmoTypeStyles = HUD.SettingsData.KeyAmmonTypeStyles.Value;
                _Mag.FireModeStyles = HUD.SettingsData.KeyFireModeStyles.Value;
            }
        }

        void WeaponTirgger()
        {
            _Mag.WeaponTirgger = true;
        }
#endif
    }
}
