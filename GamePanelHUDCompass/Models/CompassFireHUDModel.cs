#if !UNITY_EDITOR

using System;
using UnityEngine;

namespace GamePanelHUDCompass.Models
{
    internal class CompassFireHUDModel
    {
        private static readonly Lazy<CompassFireHUDModel> Lazy =
            new Lazy<CompassFireHUDModel>(() => new CompassFireHUDModel());

        public static CompassFireHUDModel Instance => Lazy.Value;

        public bool CompassFireHUDSw;

        public GameObject FirePrefab;

        public readonly CompassFireModel CompassFire = new CompassFireModel();

        public Action<FireModel> ShowFire;

        public Action<string> DestroyFire;

        private CompassFireHUDModel()
        {
        }
    }
}

#endif