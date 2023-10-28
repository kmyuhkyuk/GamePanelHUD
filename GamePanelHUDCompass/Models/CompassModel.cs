#if !UNITY_EDITOR

using UnityEngine;

namespace GamePanelHUDCompass.Models
{
    internal class CompassModel
    {
        public float Angle;

        public Vector2 SizeDelta;

        public float CompassX => -(Angle / 15 * 120);
    }
}

#endif