#if !UNITY_EDITOR

using EFT;
using UnityEngine;

namespace GamePanelHUDCompass.Models
{
    internal struct FireModel
    {
        public string Who;

        public Vector3 Where;

        public float Distance;

        public WildSpawnType Role;

        public bool IsSilenced;
    }
}

#endif