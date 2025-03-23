using TMPro;
using UnityEngine;
#if !UNITY_EDITOR
using KmyTarkovUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDCore.Utils;

#endif

namespace GamePanelHUDKill.Views
{
    public class ExpUIView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool active;

        public int xp;

        public int waitXp;

        public Color xpColor;

        public float xpWaitSpeed;

        public FontStyles xpStyles;

#if !UNITY_EDITOR

        public bool IsAnim => !IsPassive && !IsClear;

        private bool IsPassive => _animatorExpUI.GetCurrentAnimatorStateInfo(0).shortNameHash == AnimatorHash.Passive;

        private bool IsClear => _animatorExpUI.GetCurrentAnimatorStateInfo(0).shortNameHash == AnimatorHash.Clear;

#endif

        [SerializeField] private TMP_Text xpValue;

        private Animator _animatorExpUI;

#if !UNITY_EDITOR

        private void Awake()
        {
            _animatorExpUI = GetComponent<Animator>();
        }

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            _animatorExpUI.SetFloat(AnimatorHash.Speed, xpWaitSpeed);

            if (!active || IsClear)
                return;

            xp += waitXp;

            waitXp = 0;

            xpValue.fontStyle = xpStyles;
            xpValue.color = xpColor;
            xpValue.text = xp.ToString();

            _animatorExpUI.SetTrigger(AnimatorHash.Active);

            active = false;
        }

        public void XpUp(int up, int lastXp)
        {
            if (!IsClear)
            {
                xp += up + lastXp;

                active = true;
            }
            else
            {
                waitXp += up + lastXp;

                active = true;
            }
        }

        public void XpComplete()
        {
            _animatorExpUI.SetTrigger(AnimatorHash.Complete);
        }

#endif

        // ReSharper disable once UnusedMember.Local
        private void ClearXp()
        {
            xp = 0;
        }
    }
}