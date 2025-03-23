#if !UNITY_EDITOR

using System;
using EFT.HealthSystem;

namespace GamePanelHUDHealth.Models
{
    internal class HealthHUDModel
    {
        private static readonly Lazy<HealthHUDModel> Lazy = new Lazy<HealthHUDModel>(() => new HealthHUDModel());

        public static HealthHUDModel Instance => Lazy.Value;

        public bool HealthHUDSw;

        public IHealthController HealthController;

        public readonly HealthModel Health = new HealthModel();

        private HealthHUDModel()
        {
        }
    }
}

#endif