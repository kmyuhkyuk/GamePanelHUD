#if !UNITY_EDITOR

using System;
using EFT.UI;
using EFTReflection;

namespace GamePanelHUDWeapon.Models
{
    internal class ReflectionModel
    {
        private static readonly Lazy<ReflectionModel> Lazy = new Lazy<ReflectionModel>(() => new ReflectionModel());

        public static ReflectionModel Instance => Lazy.Value;

        public readonly RefHelper.FieldRef<BattleUIScreen, AmmoCountPanel> RefAmmoCountPanel;
        public readonly RefHelper.FieldRef<AmmoCountPanel, CustomTextMeshProUGUI> RefAmmoCount;

        private ReflectionModel()
        {
            RefAmmoCountPanel =
                RefHelper.FieldRef<BattleUIScreen, AmmoCountPanel>.Create("_ammoCountPanel");
            RefAmmoCount = RefHelper.FieldRef<AmmoCountPanel, CustomTextMeshProUGUI>.Create("_ammoCount");
        }
    }
}

#endif