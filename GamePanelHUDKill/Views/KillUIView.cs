using System.Threading.Tasks;
using TMPro;
using UnityEngine;
#if !UNITY_EDITOR
using EFTUtils;
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
                _animatorKillUI.SetBool(AnimatorHash.Active, active);

                Task.Run(async () =>
                {
                    textValue.text = text;

                    await TextTask(textValue, settingsModel.KeyKillWriteSpeed.Value, false);

                    if (isKillInfo)
                    {
                        await Task.Delay(settingsModel.KeyKillWaitTime.Value);

                        textValue.text = text2;

                        await TextTask(textValue, settingsModel.KeyKillWrite2Speed.Value, true);
                    }

                    _animatorKillUI.SetBool(AnimatorHash.Complete, true);

                    KillHUDView.HasWaitInfoMinus();
                });

                active = false;
            }

            if (canDestroy)
            {
                _animatorKillUI.SetBool(AnimatorHash.CanDestroy, true);

                canDestroy = false;
            }
        }

        private async Task TextTask(TMP_Text tmpText, int speed, bool toRight)
        {
            tmpText.ForceMeshUpdate();
            var textInfo = tmpText.textInfo;
            var textCount = textInfo.characterCount;

            var complete = false;
            int current;

            if (toRight)
            {
                textValue.maxVisibleCharacters = 0;

                current = 0;

                while (!complete)
                {
                    if (current > textCount)
                    {
                        current = textCount;

                        complete = true;
                    }

                    tmpText.maxVisibleCharacters = current;
                    current++;

                    await Task.Delay(speed);
                }
            }
            else
            {
                textValue.firstVisibleCharacter = textCount;

                current = textCount;

                while (!complete)
                {
                    if (current < 0)
                    {
                        current = 0;

                        complete = true;
                    }

                    tmpText.firstVisibleCharacter = current;
                    current--;

                    await Task.Delay(speed);
                }
            }
        }

#endif

        private void OnDestroy()
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