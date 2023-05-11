using UnityEngine;
using EFT.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDMag
{
    public class GamePanelHUDMag : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDMagPlugin.WeaponData, GamePanelHUDMagPlugin.SettingsData> HUD => GamePanelHUDMagPlugin.HUD;
#endif

        [SerializeField]
        private GamePanelHUDMagUI _Mag;

#if !UNITY_EDITOR
        void Start()
        {
            GamePanelHUDMagPlugin.WeaponTrigger = WeaponTrigger;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            MagHUD();
        }

        void MagHUD()
        {
            //Set RectTransform anchoredPosition and localScale
            RectTransform.anchoredPosition = HUD.SetData.KeyAnchoredPosition.Value;
            RectTransform.localScale = HUD.SetData.KeyLocalScale.Value;

            //Set Current Maximum Patron float
            if (_Mag != null)
            {
                _Mag.gameObject.SetActive(HUD.HUDSw);
                _Mag.WeaponNameAlways = HUD.Info.WeaponNameAlways;
                _Mag.AmmoTypeHUDSw = HUD.SetData.KeyAmmoTypeHUDSw.Value;
                _Mag.FireModeHUDSw = HUD.SetData.KeyFireModeHUDSw.Value;
                _Mag.ZeroWarning = HUD.SetData.KeyZeroWarning.Value;

                _Mag.Current = HUD.Info.MagCount;
                _Mag.Maximum = HUD.Info.MagMaxCount;
                _Mag.Patron = HUD.Info.Patron;
                _Mag.Normalized = HUD.Info.Normalized;
                _Mag.WeaponName = HUD.SetData.KeyWeaponShortName.Value ? HUD.Info.WeaponShortName : HUD.Info.WeaponName;
                _Mag.AmmoType = HUD.Info.AmmoType;
                _Mag.FireMode = HUD.Info.FireMode;

                _Mag.WarningRate10 = HUD.SetData.KeyWarningRate10.Value / 100f;
                _Mag.WarningRate100 = HUD.SetData.KeyWarningRate100.Value / 100f;
                _Mag.WeaponNameSpeed = HUD.SetData.KeyWeaponNameSpeed.Value;
                _Mag.ZeroWarningSpeed = HUD.SetData.KeyZeroWarningSpeed.Value;

                _Mag.CurrentColor = HUD.SetData.KeyCurrentColor.Value;
                _Mag.MaxColor = HUD.SetData.KeyMaxColor.Value;
                _Mag.PatronColor = HUD.SetData.KeyPatronColor.Value;
                _Mag.WeaponNameColor = HUD.SetData.KeyWeaponNameColor.Value;
                _Mag.AmmoTypeColor = HUD.SetData.KeyAmmoTypeColor.Value;
                _Mag.FireModeColor = HUD.SetData.KeyFireModeColor.Value;
                _Mag.AddZerosColor = HUD.SetData.KeyAddZerosColor.Value;
                _Mag.WarningColor = HUD.SetData.KeyWarningColor.Value;

                _Mag.CurrentStyles = HUD.SetData.KeyCurrentStyles.Value;
                _Mag.MaximumStyles = HUD.SetData.KeyMaximumStyles.Value;
                _Mag.PatronStyles = HUD.SetData.KeyPatronStyles.Value;
                _Mag.WeaponNameStyles = HUD.SetData.KeyWeaponNameStyles.Value;
                _Mag.AmmoTypeStyles = HUD.SetData.KeyAmmoTypeStyles.Value;
                _Mag.FireModeStyles = HUD.SetData.KeyFireModeStyles.Value;
            }
        }

        private void WeaponTrigger()
        {
            if (_Mag != null)
            {
                _Mag.WeaponTrigger = true;
            }
        }
#endif
    }
}
