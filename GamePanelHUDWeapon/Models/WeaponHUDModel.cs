#if !UNITY_EDITOR

using System;

namespace GamePanelHUDWeapon.Models
{
    internal class WeaponHUDModel
    {
        private static readonly Lazy<WeaponHUDModel> Lazy = new Lazy<WeaponHUDModel>(() => new WeaponHUDModel());

        public static WeaponHUDModel Instance => Lazy.Value;

        public bool WeaponHUDSw;

        public readonly WeaponModel Weapon = new WeaponModel();

        public Action WeaponTrigger;

        private WeaponHUDModel()
        {
        }
    }
}

#endif