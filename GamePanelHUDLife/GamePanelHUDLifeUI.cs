using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

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

        public Color CurrentColor;

        public Color MaxColor;

        public Color AddZerosColor;

        public Color WarningColor;

        public Color UpBuffColor;

        public Color DownBuffColor;

        public FontStyles CurrentStyles;

        public FontStyles MaximumStyles;

        [SerializeField]
        private TMP_Text _ZerosValue;

        [SerializeField]
        private TMP_Text _CurrentValue;

        [SerializeField]
        private TMP_Text _MaxSignValue;

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

        void Start()
        {
            Animator_UpBuffArrow = _UpBuffArrow.GetComponent<Animator>();
            Animator_DownBuffArrow = _DownBuffArrow.GetComponent<Animator>();

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
            LifeUI();
        }

        void LifeUI()
#endif
#if UNITY_EDITOR
        void Update()
#endif
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

                _ZerosValue.gameObject.SetActive(false);
                _CurrentValue.alignment = TextAlignmentOptions.Top;
            }
            else if (current < 100 && current >= 10)
            {
                addZeros = "0";

                _ZerosValue.gameObject.SetActive(true);
                _CurrentValue.alignment = TextAlignmentOptions.TopLeft;
            }
            else
            {
                addZeros = "00";

                _ZerosValue.gameObject.SetActive(true);
                _CurrentValue.alignment = TextAlignmentOptions.TopLeft;
            }

            _ZerosValue.fontStyle = CurrentStyles;
            _ZerosValue.color = AddZerosColor;
            _ZerosValue.text = addZeros;

            Color currentColor = Normalized > WarningRate ? CurrentColor : WarningColor;

            _CurrentValue.fontStyle = CurrentStyles;
            _CurrentValue.color = currentColor;
            _CurrentValue.text = current.ToString();

            _MaxSignValue.fontStyle = MaximumStyles;
            _MaxSignValue.color = MaxColor;

            //Set Maximum float and color and Style to String
            _MaxValue.fontStyle = MaximumStyles;
            _MaxValue.color = MaxColor;
            _MaxValue.text = Maximum.ToString("F0");

            //Buff HUD display
            _BuffValue.gameObject.SetActive(BuffRate != 0 && BuffHUDSW);

            //Buff Arrow Up or Down
            _UpBuffArrow.gameObject.SetActive(BuffRate > 0);
            _DownBuffArrow.gameObject.SetActive(BuffRate < 0);

            //Buff Arrow Color
            _UpBuffArrow.color = UpBuffArrowColor;
            _DownBuffArrow.color = DownBuffArrowColor;

            //Buff Up Down Color 
            Color buffColor = BuffRate > 0 ? UpBuffColor : DownBuffColor;

            _BuffValue.color = buffColor;
            _BuffValue.text = BuffRate.ToString("F2");

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
    }
}
