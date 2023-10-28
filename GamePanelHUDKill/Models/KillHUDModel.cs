#if !UNITY_EDITOR

using System;
using UnityEngine;

namespace GamePanelHUDKill.Models
{
    internal class KillHUDModel
    {
        private static readonly Lazy<KillHUDModel> Lazy = new Lazy<KillHUDModel>(() => new KillHUDModel());

        public static KillHUDModel Instance => Lazy.Value;

        public bool KillHUDSw;

        public bool ExpHUDSw;

        public int KillCount;

        public RectTransform ScreenRect;

        public GameObject KillPrefab;

        public Action<KillModel> ShowKill;

        private KillHUDModel()
        {
        }
    }
}

#endif