using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if !UNITY_EDITOR
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
#endif

namespace GamePanelHUDLife
{
    public class GamePanelHUDLifeUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool ArrowAnimation;

        public bool ArrowAnimationReverse;

        public bool BuffHUDSW;

        public bool IsHealth;

        public bool AtMaximum;

        public float Current;

        public float Maximum;

        public float BuffRate;

        public float Normalized;

        public float WarningRate;

        public float BuffSpeed;

        public Color UpBuffArrowColor;

        public Color DownBuffArrowColor;

        public string CurrentColor;

        public string MaxColor;

        public string AddZerosColor;

        public string WarningColor;

        public string UpBuffColor;

        public string DownBuffColor;

        public FontStyles CurrentStyles;

        public FontStyles MaximumStyles;

        [SerializeField]
        private TMP_Text _CurrentValue;

        [SerializeField]
        private TMP_Text _MaxValue;

        [SerializeField]
        private Image _Glow;

        [SerializeField]
        private TMP_Text _BuffValue;

        [SerializeField]
        private Image _UpBuffArrow;

        [SerializeField]
        private Image _DownBuffArrow;

        private Animator Animator_UpBuffArrow;

        private Animator Animator_DownBuffArrow;

        private readonly StringBuilderData StringBuilderdDatas = new StringBuilderData();

#if !UNITY_EDITOR
        void Start()
        {
            Animator_UpBuffArrow = _UpBuffArrow.GetComponent<Animator>();
            Animator_DownBuffArrow = _DownBuffArrow.GetComponent<Animator>();

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
            LifeUI();
        }

        void LifeUI()
        {
            //Set Current float and color and Style to String
            double current;
            if (IsHealth)
            {
                current = Math.Round(Current);
            }
            else if (Normalized > 0.5)
            {
                current = Math.Floor(Current);
            }
            else
            {
                current = Math.Ceiling(Current);
            }

            string addZeros;
            if (current >= 100)
            {
                addZeros = "";

            }
            else if (current < 100 && current >= 10)
            {
                addZeros = "0";
            }
            else
            {
                addZeros = "00";
            }

            string currentColor;
            if (Normalized > WarningRate)
            {
                currentColor = CurrentColor;
            }
            else
            {
                currentColor = WarningColor;
            }

            _CurrentValue.fontStyle = CurrentStyles;
            _CurrentValue.text = StringBuilderdDatas._CurrentValue.Chcek("<color=", AddZerosColor, ">", addZeros, "</color>", "<color=", currentColor, ">", current, "</color>");

            //Set Maximum float and color and Style to String
            _MaxValue.fontStyle = MaximumStyles;
            _MaxValue.text = StringBuilderdDatas._MaxValue.Chcek("<color=", MaxColor, ">", "/", Maximum.ToString("F0"), "</color>");

            //Buff HUD display
            _BuffValue.gameObject.SetActive(BuffRate != 0 && BuffHUDSW);

            //Buff Arrow Up or Down
            _UpBuffArrow.gameObject.SetActive(BuffRate > 0);
            _DownBuffArrow.gameObject.SetActive(BuffRate < 0);

            //Buff Arrow Color
            _UpBuffArrow.color = UpBuffArrowColor;
            _DownBuffArrow.color = DownBuffArrowColor;

            string buffColor;
            //Buff Up Down Color 
            if (BuffRate > 0)
            {
                buffColor = UpBuffColor;
            }
            else
            {
                buffColor = DownBuffColor;
            }

            _BuffValue.text = StringBuilderdDatas._BuffValue.Chcek("<color=", buffColor, ">", BuffRate.ToString("F2"), "</color>");

            //Arrow Animation display
            bool Arrow = !AtMaximum && ArrowAnimation;
            Animator_UpBuffArrow.SetBool(AnimatorHash.Active, Arrow);
            Animator_DownBuffArrow.SetBool(AnimatorHash.Active, Arrow);

            //Arrow Animation Speed
            Animator_UpBuffArrow.SetFloat(AnimatorHash.Speed, BuffSpeed);
            Animator_DownBuffArrow.SetFloat(AnimatorHash.Speed, BuffSpeed);

            //Arrow Animation Reverse
            Animator_UpBuffArrow.SetBool(AnimatorHash.Reverse, ArrowAnimationReverse);
            Animator_DownBuffArrow.SetBool(AnimatorHash.Reverse, ArrowAnimationReverse);

            //Glow display
            if (_Glow != null)
            {
                _Glow.gameObject.SetActive(Normalized < WarningRate);
            }
        }
#endif

        public class StringBuilderData
        {
            public StringBuilderHelp.StringChange _CurrentValue = new StringBuilderHelp.StringChange(128);
            public StringBuilderHelp.StringChange _MaxValue = new StringBuilderHelp.StringChange(128);
            public StringBuilderHelp.StringChange _BuffValue = new StringBuilderHelp.StringChange(128);
        }
    }
}
