using GamePanelHUDCore.Utils;
using TMPro;
using UnityEngine;
#if !UNITY_EDITOR

using EFTUtils;
using GamePanelHUDCore;

#endif

namespace GamePanelHUDHit
{
    public class GamePanelHUDExpUI : MonoBehaviour
#if !UNITY_EDITOR

        , IUpdate

#endif
    {
#if !UNITY_EDITOR

        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

#endif

        public bool active;

        public int xp;

        public int waitXp;

        public Color xpColor;

        public float xpWaitSpeed;

        public FontStyles xpStyles;

        public bool IsAnim => !IsPassive && !IsClear;

        public bool IsPassive => _animatorExpUI.GetCurrentAnimatorStateInfo(0).shortNameHash == AnimatorHash.Passive;

        public bool IsClear => _animatorExpUI.GetCurrentAnimatorStateInfo(0).shortNameHash == AnimatorHash.Clear;

        [SerializeField] private TMP_Text xpValue;

        private Animator _animatorExpUI;

        private void Start()
        {
            _animatorExpUI = GetComponent<Animator>();

#if !UNITY_EDITOR

            HUDCore.UpdateManger.Register(this);

#endif
        }
#if !UNITY_EDITOR


        public void CustomUpdate()
        {
            ExpUI();
        }

        private void ExpUI()

#endif
#if UNITY_EDITOR
        void Update()

#endif
        {
            _animatorExpUI.SetFloat(AnimatorHash.Speed, xpWaitSpeed);

            if (active && !IsClear)
            {
                xp += waitXp;

                waitXp = 0;

                xpValue.fontStyle = xpStyles;
                xpValue.color = xpColor;
                xpValue.text = xp.ToString();

                _animatorExpUI.SetTrigger(AnimatorHash.Active);

                active = false;
            }
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

        private void ClearXp()
        {
            xp = 0;
        }
    }
}