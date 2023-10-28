#if !UNITY_EDITOR

namespace GamePanelHUDWeapon.Models
{
    internal class WeaponModel
    {
        public int Patron;

        public int MagCount;

        public int MagMaxCount;

        public float Normalized;

        public string WeaponName;

        public string WeaponShortName;

        public string AmmoType;

        public string FireMode;

        public bool WeaponNameAlways;
    }
}

#endif