﻿using UnityEngine;
using TMPro;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDHit
{
    public class GamePanelHUDExpUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool Active;

        public int XP;

        public int WaitXP;

        public string XPColor;

        public float XPWaitSpeed;

        public FontStyles XPStyles;

        [SerializeField]
        private TMP_Text _XpValue;

        private Animator Animator_ExpUI;

        private readonly IStringBuilderData IStringBuilderDatas = new IStringBuilderData();

        void Start()
        {
            Animator_ExpUI = GetComponent<Animator>();

#if !UNITY_EDITOR
            GamePanelHUDCorePlugin.UpdateManger.Register(this);
#endif
        }
#if !UNITY_EDITOR

        public void IUpdate()
        {
            ExpUI();
        }

        void ExpUI()
#endif
#if UNITY_EDITOR
        void Update()
#endif
        {
            _XpValue.fontStyle = XPStyles;

            Animator_ExpUI.SetFloat(AnimatorHash.Speed, XPWaitSpeed);

            if (Active && !IsClear())
            {
                XP += WaitXP;

                WaitXP = 0;

                _XpValue.text = IStringBuilderDatas._XpValue.Concat("<color=", XPColor, ">", XP.ToString(), "</color>");

                Animator_ExpUI.SetTrigger(AnimatorHash.Active);

                Active = false;
            }
        }

        public void XpUp(int up, int lastxp)
        {
            if (!IsClear())
            {
                XP += up + lastxp;

                Active = true;
            }
            else
            {
                WaitXP += up + lastxp;

                Active = true;
            }
        }

        public void XpComplete()
        {
            Animator_ExpUI.SetTrigger(AnimatorHash.Complete);
        }

        public bool IsAnim()
        {
            return !IsPassive() && !IsClear();
        }

        public bool IsPassive()
        {
            return Animator_ExpUI.GetCurrentAnimatorStateInfo(0).shortNameHash == AnimatorHash.Passive;
        }

        public bool IsClear()
        {
            return Animator_ExpUI.GetCurrentAnimatorStateInfo(0).shortNameHash == AnimatorHash.Clear;
        }

        public class IStringBuilderData
        {
            public IStringBuilder _XpValue = new IStringBuilder(128);
        }

        void ClearXp()
        {
            XP = 0;
        }
    }
}
