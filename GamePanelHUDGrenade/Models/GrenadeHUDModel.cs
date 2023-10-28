#if !UNITY_EDITOR

using System;

namespace GamePanelHUDGrenade.Models
{
    internal class GrenadeHUDModel
    {
        private static readonly Lazy<GrenadeHUDModel> Lazy = new Lazy<GrenadeHUDModel>(() => new GrenadeHUDModel());

        public static GrenadeHUDModel Instance => Lazy.Value;

        public bool GrenadeHUDSw;

        public readonly GrenadeAmountModel RigAmount = new GrenadeAmountModel();

        public readonly GrenadeAmountModel PocketAmount = new GrenadeAmountModel();

        public readonly GrenadeAmountModel AllAmount = new GrenadeAmountModel();

        private GrenadeHUDModel()
        {
        }
    }
}

#endif