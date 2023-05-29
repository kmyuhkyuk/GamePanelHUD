using System.Threading.Tasks;
using GamePanelHUDCore.Utils;
using TMPro;
using UnityEngine;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif

namespace GamePanelHUDHit
{
    public class GamePanelHUDKillUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;
        private static GamePanelHUDCorePlugin.HUDClass<RectTransform, GamePanelHUDHitPlugin.SettingsData> HUD =>
            GamePanelHUDHitPlugin.KillHUD;
#endif

        public bool active;

        public bool isKillInfo;

        public bool hasXp;

        public bool canDestroy;

        public string text;

        public string text2;

        public int xp;

        public Color xpColor;

        public FontStyles textFontStyles;

        public GamePanelHUDKillUI after;

        [SerializeField] private TMP_Text textValue;

        [SerializeField] private TMP_Text xpValue;

        private Animator _animatorKillUI;

#if !UNITY_EDITOR
        private void Start()
        {
            _animatorKillUI = GetComponent<Animator>();

            textValue.fontStyle = textFontStyles;

            if (hasXp)
            {
                xpValue.fontStyle = HUD.SetData.KeyKillXpStyles.Value;
                xpValue.color = xpColor;
                xpValue.text = xp.ToString();
            }

            HUDCore.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            KillUI();
        }

        private void KillUI()
        {
            _animatorKillUI.SetFloat(AnimatorHash.Speed, HUD.SetData.KeyKillWaitSpeed.Value);

            if (active)
            {
                _animatorKillUI.SetBool(AnimatorHash.Active, active);

                TextPlay(HUD.SetData.KeyKillWriteSpeed.Value, HUD.SetData.KeyKillWrite2Speed.Value,
                    HUD.SetData.KeyKillWaitTime.Value);

                active = false;
            }

            if (canDestroy)
            {
                _animatorKillUI.SetBool(AnimatorHash.CanDestroy, true);

                canDestroy = false;
            }
        }

        private async void TextPlay(int speed, int speed2, int waitTime)
        {
            textValue.text = text;

            await TextTask(textValue, speed, false);

            if (isKillInfo)
            {
                await Task.Delay(waitTime);

                textValue.text = text2;

                await TextTask(textValue, speed2, true);
            }

            _animatorKillUI.SetBool(AnimatorHash.Complete, true);

            GamePanelHUDKill.HasWaitInfoMinus();
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

        private void Destroy()
        {
#if !UNITY_EDITOR
            if (after != null)
            {
                after.canDestroy = true;
            }

            GamePanelHUDKill.HasInfoMinus();
            HUDCore.UpdateManger.Remove(this);
            Destroy(gameObject);
#endif
        }
    }
}