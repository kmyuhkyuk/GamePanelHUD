#if !UNITY_EDITOR

using System;
using UnityEngine;

namespace GamePanelHUDCompass.Models
{
    internal class CompassStaticHUDModel
    {
        private static readonly Lazy<CompassStaticHUDModel> Lazy =
            new Lazy<CompassStaticHUDModel>(() => new CompassStaticHUDModel());

        public static CompassStaticHUDModel Instance => Lazy.Value;

        public bool CompassStaticHUDSw;

        public GameObject StaticPrefab;

        public bool CompassStaticCacheBool;

        public int AirdropCount;

        public readonly CompassStaticModel CompassStatic = new CompassStaticModel();

        public Action<StaticModel> ShowStatic;

        public Action<string> DestroyStatic;

        private CompassStaticHUDModel()
        {
        }
    }
}

#endif