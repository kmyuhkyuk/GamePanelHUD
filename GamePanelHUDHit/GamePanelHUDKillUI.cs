using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDHit
{
    public class GamePanelHUDKillUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<RectTransform, GamePanelHUDHitPlugin.SettingsData> HUD
        {
            get
            {
                return GamePanelHUDHitPlugin.KillHUD;
            }
        }
#endif

        public bool Active;

        public bool IsKillInfo;

        public bool HasXp;

        public bool CanDestroy;

        public string Text;

        public string Text2;

        public int Xp;

        public FontStyles TextFontStyles;

        public GamePanelHUDKillUI After;

        [SerializeField]
        private TMP_Text _TextValue;

        [SerializeField]
        private TMP_Text _XpValue;

        private Animator Animator_KillUI;

        private readonly StringBuilderData StringBuilderDatas = new StringBuilderData();

#if !UNITY_EDITOR
        void Start()
        {
            Animator_KillUI = GetComponent<Animator>();

            _TextValue.fontStyle = TextFontStyles;

            _XpValue.fontStyle = HUD.SettingsData.KeyKillXpStyles.Value;

            if (HasXp)
            {
                _XpValue.text = StringBuilderDatas._XpValue.StringConcat("<color=", HUD.SettingsData.KeyKillXpColor.Value.ColorToHtml(), ">", Xp, "</color>");
            }

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            KillUI();
        }

        void KillUI()
        {
            Animator_KillUI.SetFloat(AnimatorHash.Speed, HUD.SettingsData.KeyKillWaitSpeed.Value);

            if (Active)
            {
                Animator_KillUI.SetBool(AnimatorHash.Active, Active);

                TextPlay(HUD.SettingsData.KeyKillWriteSpeed.Value, HUD.SettingsData.KeyKillWrite2Speed.Value, HUD.SettingsData.KeyKillWaitTime.Value);

                Active = false;
            }

            if (CanDestroy)
            {
                Animator_KillUI.SetBool(AnimatorHash.CanDestroy, true);

                CanDestroy = false;
            }
        }

        async void TextPlay(int speed, int speed2, int waittime)
        {
            _TextValue.text = Text;

            await TextTask(_TextValue, speed, false);

            if (IsKillInfo)
            {
                await Task.Delay(waittime);

                _TextValue.text = Text2;

                await TextTask(_TextValue, speed2, true);
            }

            Animator_KillUI.SetBool(AnimatorHash.Complete, true);

            GamePanelHUDKill.HasWaitInfoMinu();
        }

        async Task TextTask(TMP_Text text, int speed, bool isletftoright)
        {
            text.ForceMeshUpdate();
            TMP_TextInfo textInfo = text.textInfo;
            int textCount = textInfo.characterCount;

            bool complete = false;
            int current;

            if (isletftoright)
            {
                _TextValue.maxVisibleCharacters = 0;

                current = 0;

                while (!complete)
                {
                    if (current > textCount)
                    {
                        current = textCount;

                        complete = true;
                    }

                    text.maxVisibleCharacters = current;
                    current++;

                    await Task.Delay(speed);
                }
            }
            else
            {
                _TextValue.firstVisibleCharacter = textCount;

                current = textCount;

                while (!complete)
                {
                    if (current < 0)
                    {
                        current = 0;

                        complete = true;
                    }

                    text.firstVisibleCharacter = current;
                    current -= 1;

                    await Task.Delay(speed);
                }
            }
        }
#endif

        public class StringBuilderData
        {
            public StringBuilder _XpValue = new StringBuilder(128);
        }

        void Destroy()
        {
#if !UNITY_EDITOR
            if (After != null)
            {
                After.CanDestroy = true;
            }

            GamePanelHUDKill.HasInfoMinu();
            GamePanelHUDCorePlugin.UpdateManger.Remove(this);
            Destroy(gameObject);
#endif
        }
    }
}
