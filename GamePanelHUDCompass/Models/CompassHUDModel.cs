#if !UNITY_EDITOR

using System;
using UnityEngine;

namespace GamePanelHUDCompass.Models
{
    internal class CompassHUDModel
    {
        private static readonly Lazy<CompassHUDModel> Lazy = new Lazy<CompassHUDModel>(() => new CompassHUDModel());

        public static CompassHUDModel Instance => Lazy.Value;

        public bool CompassHUDSw;

        public Transform CamTransform;

        public RectTransform ScreenRect;

        public readonly CompassModel Compass = new CompassModel();

        public Action<bool> SetCompassState;

        private CompassHUDModel()
        {
        }
    }
}

#endif