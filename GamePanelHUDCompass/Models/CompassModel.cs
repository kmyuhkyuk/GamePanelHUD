#if !UNITY_EDITOR

using UnityEngine;

namespace GamePanelHUDCompass.Models
{
    internal class CompassModel
    {
        public float Angle;

        public Vector2 SizeDelta;

        public bool CompassState;

        public float CompassX => -Angle * 8f;
    }
}

#endif