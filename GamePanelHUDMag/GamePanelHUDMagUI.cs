﻿using UnityEngine;
using TMPro;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDMag
{
    public class GamePanelHUDMagUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool WeaponNameAlways;

        public bool AmmoTypeHUDSW;

        public bool FireModeHUDSW;

        public bool ZeroWarning;

        public bool WeaponTirgger;

        public int Current;

        public int Maximum;

        public int Patron;

        public float Normalized;

        public string WeaponName;

        public string AmmoType;

        public string FireMode;

        public float WarningRate10;

        public float WarningRate100;

        public float WeaponNameSpeed;

        public float ZeroWarningSpeed;

        public string CurrentColor;

        public string MaxColor;

        public string PatronColor;

        public string WeaponNameColor;

        public string AmmoTypeColor;

        public string FireModeColor;

        public string AddZerosColor;

        public string WarningColor;

        public FontStyles CurrentStyles;

        public FontStyles MaximumStyles;

        public FontStyles PatronStyles;

        public FontStyles WeaponNameStyles;

        public FontStyles AmmoTypeStyles;

        public FontStyles FireModeStyles;

        [SerializeField]
        private TMP_Text _CurrentValue;

        [SerializeField]
        private TMP_Text _MaxValue;

        [SerializeField]
        private TMP_Text _PatronValue;

        [SerializeField]
        private TMP_Text _WeaponNameValue;

        [SerializeField]
        private TMP_Text _AmmoTypeValue;

        [SerializeField]
        private TMP_Text _FiremodeValue;

        private Animator Animator_WeaponName;

        private Animator Animator_Current;

        private readonly IStringBuilderData IStringBuilderDatas = new IStringBuilderData();

        void Start()
        {
            Animator_WeaponName = _WeaponNameValue.transform.parent.GetComponent<Animator>();
            Animator_Current = _CurrentValue.GetComponent<Animator>();

#if !UNITY_EDITOR
            GamePanelHUDCorePlugin.UpdateManger.Register(this);
#endif
        }
#if !UNITY_EDITOR

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
            MagUI();
        }

        void MagUI()
#endif
#if UNITY_EDITOR
        void Update()
#endif
        {
            //Set Current float and color and Style to String
            string addZeros = Current < 10 && Maximum > 10 ? "0" : "";

            string currentColor;
            if ((Normalized > WarningRate10 && Maximum <= 10 || Normalized >= WarningRate100 && Maximum > 10) && Current != 0)
            {
                currentColor = CurrentColor;
            }
            else
            {
                currentColor = WarningColor;
            }

            _CurrentValue.fontStyle = CurrentStyles;
            _CurrentValue.text = IStringBuilderDatas._CurrentValue.Concat("<color=", AddZerosColor, ">", addZeros, "</color>", "<color=", currentColor, ">", Current.ToString(), "</color>");

            //Set Maximum float and color and Style to String
            _MaxValue.fontStyle = MaximumStyles;
            _MaxValue.text = IStringBuilderDatas._MaxValue.Concat("<color=", MaxColor, ">", "/", Maximum.ToString(), "</color>");

            //Patron HUD display
            _PatronValue.gameObject.SetActive(Patron > 0);

            //Set Patron float and color and Style to String
            _PatronValue.fontStyle = PatronStyles;
            _PatronValue.text = IStringBuilderDatas._PatronValue.Concat("<color=", PatronColor, ">", "+", Patron.ToString(), "</color>");

            //Set Weapon Name
            _WeaponNameValue.fontStyle = WeaponNameStyles;
            _WeaponNameValue.text = IStringBuilderDatas._WeaponValue.Concat("<color=", WeaponNameColor, ">", WeaponName, "</color>");

            Animator_WeaponName.SetBool(AnimatorHash.Always, WeaponNameAlways);
            Animator_WeaponName.SetFloat(AnimatorHash.Speed, WeaponNameSpeed);

            Animator_Current.SetBool(AnimatorHash.Zero, Current == 0 && Patron == 0 && ZeroWarning);
            Animator_Current.SetFloat(AnimatorHash.Speed, ZeroWarningSpeed);

            //Fire Mode HUD display
            _FiremodeValue.gameObject.SetActive(FireModeHUDSW);

            //Set Fire Mode
            _FiremodeValue.fontStyle = FireModeStyles;
            _FiremodeValue.text = IStringBuilderDatas._FiremodeValue.Concat("<color=", FireModeColor, ">", FireMode, "</color>");

            _AmmoTypeValue.gameObject.SetActive(AmmoTypeHUDSW);

            _AmmoTypeValue.fontStyle = AmmoTypeStyles;
            _AmmoTypeValue.text = IStringBuilderDatas._AmmoValue.Concat("<color=", AmmoTypeColor, ">", AmmoType, "</color>");

            if (WeaponTirgger)
            {
                Animator_WeaponName.SetTrigger(AnimatorHash.Active);

                WeaponTirgger = false;
            }
        }

        public class IStringBuilderData
        {
            public IStringBuilder _CurrentValue = new IStringBuilder(128);
            public IStringBuilder _MaxValue = new IStringBuilder(128);
            public IStringBuilder _PatronValue = new IStringBuilder(128);
            public IStringBuilder _WeaponValue = new IStringBuilder(128);
            public IStringBuilder _FiremodeValue = new IStringBuilder(128);
            public IStringBuilder _AmmoValue = new IStringBuilder(128);
        }
    }
}
