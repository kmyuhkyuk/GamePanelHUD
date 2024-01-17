#if !UNITY_EDITOR

using EFT.HealthSystem;

// ReSharper disable NotAccessedField.Global

namespace GamePanelHUDHealth.Models
{
    internal class HealthModel
    {
        //Health Current float
        public ValueStruct Head;

        public ValueStruct Chest;

        public ValueStruct Stomach;

        public ValueStruct LeftArm;

        public ValueStruct RightArm;

        public ValueStruct LeftLeg;

        public ValueStruct RightLeg;

        public ValueStruct Common;

        public ValueStruct Hydration;

        public ValueStruct Energy;

        public float HealthRate;

        public float HydrationRate;

        public float EnergyRate;
    }
}

#endif