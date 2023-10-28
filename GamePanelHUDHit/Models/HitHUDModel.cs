#if !UNITY_EDITOR

using System;
using UnityEngine;

namespace GamePanelHUDHit.Models
{
    internal class HitHUDModel
    {
        private static readonly Lazy<HitHUDModel> Lazy = new Lazy<HitHUDModel>(() => new HitHUDModel());

        public static HitHUDModel Instance => Lazy.Value;

        public bool HitHUDSw;

        public RectTransform ScreenRect;

        public Action<HitModel> ShowHit;

        private HitHUDModel()
        {
        }
    }
}

#endif