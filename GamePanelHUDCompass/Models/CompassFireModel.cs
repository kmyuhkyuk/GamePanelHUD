#if !UNITY_EDITOR

using UnityEngine;

namespace GamePanelHUDCompass.Models
{
    internal class CompassFireModel
    {
        public Vector3 NorthVector;

        public Vector3 CameraPosition;

        public Vector3 PlayerRight;

        public float GetToAngle(Vector3 lhs)
        {
            lhs.y = 0;

            var num = Vector3.SignedAngle(lhs, NorthVector, Vector3.up);

            if (num >= 0)
                return num;

            return num + 360;
        }
    }
}

#endif