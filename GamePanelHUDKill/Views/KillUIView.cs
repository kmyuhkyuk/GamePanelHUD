using TMPro;
using UnityEngine;
#if !UNITY_EDITOR
using System.Collections;
using KmyTarkovUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDCore.Utils;
using SettingsModel = GamePanelHUDKill.Models.SettingsModel;

#endif

namespace GamePanelHUDKill.Views
{
    public class KillUIView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool active;

        public bool isKillInfo;

        public bool hasXp;

        public bool canDestroy;

        public string text;

        public string text2;

        public int xp;

        public Color xpColor;

        public FontStyles textFontStyles;

        public KillUIView after;

        [SerializeField] private TMP_Text textValue;

        [SerializeField] private TMP_Text xpValue;

        private Animator _animatorKillUI;

#if !UNITY_EDITOR

        private void Awake()
        {
            _animatorKillUI = GetComponent<Animator>();
        }

        private void Start()
        {
            textValue.fontStyle = textFontStyles;

            if (hasXp)
            {
                xpValue.fontStyle = SettingsModel.Instance.KeyKillXpStyles.Value;
                xpValue.color = xpColor;
                xpValue.text = xp.ToString();
            }

            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var settingsModel = SettingsModel.Instance;

            _animatorKillUI.SetFloat(AnimatorHash.Speed, settingsModel.KeyKillWaitSpeed.Value);

            if (active)
            {
                _animatorKillUI.SetBool(AnimatorHash.Active, true);

                StartCoroutine(TextPlay());

                active = false;
            }

            if (!canDestroy)
                return;

            _animatorKillUI.SetBool(AnimatorHash.CanDestroy, true);

            canDestroy = false;
        }

        private IEnumerator TextPlay()
        {
            var settingsModel = SettingsModel.Instance;

            textValue.text = text;

            yield return TextAnimToLeft(textValue, settingsModel.KeyKillWriteWaitTime.Value);

            if (isKillInfo)
            {
                yield return new WaitForSecondsRealtime(settingsModel.KeyKillWaitTime.Value);

                textValue.text = text2;

                yield return TextAnimToRight(textValue, settingsModel.KeyKillWrite2WaitTime.Value);
            }

            _animatorKillUI.SetBool(AnimatorHash.Complete, true);

            KillHUDView.HasWaitInfoMinus();
        }

        private static IEnumerator TextAnimToRight(TMP_Text tmpText, float waitTime)
        {
            tmpText.ForceMeshUpdate();
            var textInfo = tmpText.textInfo;
            var textCount = textInfo.characterCount;

            tmpText.maxVisibleCharacters = 0;

            var complete = false;
            var current = 0;

            while (!complete)
            {
                if (current > textCount)
                {
                    current = textCount;

                    complete = true;
                }

                tmpText.maxVisibleCharacters = current;
                current++;

                yield return new WaitForSecondsRealtime(waitTime);
            }
        }

        private static IEnumerator TextAnimToLeft(TMP_Text tmpText, float waitTime)
        {
            tmpText.ForceMeshUpdate();
            var textInfo = tmpText.textInfo;
            var textCount = textInfo.characterCount;

            tmpText.firstVisibleCharacter = textCount;

            var complete = false;
            var current = textCount;

            while (!complete)
            {
                if (current < 0)
                {
                    current = 0;

                    complete = true;
                }

                tmpText.firstVisibleCharacter = current;
                current--;

                yield return new WaitForSecondsRealtime(waitTime);
            }
        }

#endif

        // ReSharper disable once UnusedMember.Local
        private void Destroy()
        {
#if !UNITY_EDITOR

            if (after != null)
            {
                after.canDestroy = true;
            }

            KillHUDView.HasInfoMinus();
            HUDCoreModel.Instance.UpdateManger.Remove(this);
            Destroy(gameObject);

#endif
        }
    }
}