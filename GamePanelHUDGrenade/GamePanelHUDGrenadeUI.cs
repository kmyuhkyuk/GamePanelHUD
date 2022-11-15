﻿using System.Text;
using UnityEngine;
using TMPro;
#if !UNITY_EDITOR
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
#endif

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

        private readonly StringBuilderData StringBuilderDatas = new StringBuilderData();

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
            _GrenadeValue.text = StringBuilderDatas._GrenadeValue.Chcek("</color>", "<color=", grenadeColor, ">", GrenadeAmount, "</color>");
        }
#endif

        public class StringBuilderData
        {
            public StringBuilderHelp.StringChange _GrenadeValue = new StringBuilderHelp.StringChange(128);
        }
    }
}
