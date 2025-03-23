#if !UNITY_EDITOR

using System;
using EFT;
using GamePanelHUDCore.Models;

namespace GamePanelHUDHit.Models
{
    internal class ArmorModel
    {
        private static readonly Lazy<ArmorModel> Lazy = new Lazy<ArmorModel>(() => new ArmorModel());

        public static ArmorModel Instance => Lazy.Value;

        public bool Activate;

        public float Damage;

        private ArmorModel()
        {
        }

        public void Set(DamageInfoStruct damageInfo, float armorDamage)
        {
            if ((Player)damageInfo.Player?.iPlayer != HUDCoreModel.Instance.YourPlayer)
                return;

            Damage = armorDamage;
            Activate = true;
        }

        public void Reset()
        {
            Damage = 0;
            Activate = false;
        }
    }
}

#endif