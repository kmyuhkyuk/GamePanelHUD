﻿using UnityEngine;
using TMPro;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDGrenade
{
    public class GamePanelHUDGrenadeUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool ZeroWarning;

        public int GrenadeAmount;

        public string GrenadeColor;

        public string WarningColor;

        public FontStyles GrenadeStyles;

        [SerializeField]
        private TMP_Text _GrenadeValue;

        private readonly IStringBuilderData IStringBuilderDatas = new IStringBuilderData();

#if !UNITY_EDITOR
        void Start()
        {
            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        void OnEnable()
        {
            GamePanelHUDCorePlugin.UpdateManger.Run(this);
        }

        void OnDisable()
        {
            GamePanelHUDCorePlugin.UpdateManger.Stop(this);
        }

        public void IUpdate()
        {
            GrenadeUI();
        }

        void GrenadeUI()
#endif
#if UNITY_EDITOR
        void Update()
#endif
        {
            //Set GrenadeAmount int and color and Style to String
            string grenadeColor;
            if (ZeroWarning && GrenadeAmount > 0 || !ZeroWarning)
            {
                grenadeColor = GrenadeColor;
            }
            else
            {
                grenadeColor = WarningColor;
            }

            _GrenadeValue.fontStyle = GrenadeStyles;
            _GrenadeValue.text = IStringBuilderDatas._GrenadeValue.Concat("<color=", grenadeColor, ">", GrenadeAmount.ToString(), "</color>");
        }

        public class IStringBuilderData
        {
            public IStringBuilder _GrenadeValue = new IStringBuilder();
        }
    }
}
