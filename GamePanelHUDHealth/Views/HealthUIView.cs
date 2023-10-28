using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDCore.Utils;

#endif

namespace GamePanelHUDHealth.Views
{
    public class HealthUIView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool arrowAnimation;

        public bool arrowAnimationReverse;

        public bool buffHUDSw;

        public bool isHealth;

        public bool atMaximum;

        public float current;

        public float maximum;

        public float buffRate;

        public float normalized;

        public float warningRate;

        public float buffSpeed;

        public Color upBuffArrowColor;

        public Color downBuffArrowColor;

        public Color currentColor;

        public Color maxColor;

        public Color addZerosColor;

        public Color warningColor;

        public Color upBuffColor;

        public Color downBuffColor;

        public FontStyles currentStyles;

        public FontStyles maximumStyles;

        [SerializeField] private TMP_Text zerosValue;

        [SerializeField] private TMP_Text currentValue;

        [SerializeField] private TMP_Text maxSignValue;

        [SerializeField] private TMP_Text maxValue;

        [SerializeField] private Image glow;

        [SerializeField] private TMP_Text buffValue;

        [SerializeField] private Image upBuffArrow;

        [SerializeField] private Image downBuffArrow;

        private Animator _animatorUpBuffArrow;

        private Animator _animatorDownBuffArrow;

#if !UNITY_EDITOR
        private void Awake()
        {
            _animatorUpBuffArrow = upBuffArrow.GetComponent<Animator>();
            _animatorDownBuffArrow = downBuffArrow.GetComponent<Animator>();
        }

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        private void OnEnable()
        {
            HUDCoreModel.Instance.UpdateManger.Run(this);
        }

        private void OnDisable()
        {
            HUDCoreModel.Instance.UpdateManger.Stop(this);
        }

        public void CustomUpdate()
        {
            //Set Current float and color and Style to String
            double currentApprox;
            if (isHealth)
            {
                currentApprox = Math.Round(current);
            }
            else if (normalized > 0.5)
            {
                currentApprox = Math.Floor(current);
            }
            else
            {
                currentApprox = Math.Ceiling(current);
            }

            string addZeros;
            if (currentApprox >= 100)
            {
                addZeros = string.Empty;

                zerosValue.gameObject.SetActive(false);
                currentValue.alignment = TextAlignmentOptions.Top;
            }
            else if (currentApprox < 100 && currentApprox >= 10)
            {
                addZeros = "0";

                zerosValue.gameObject.SetActive(true);
                currentValue.alignment = TextAlignmentOptions.TopLeft;
            }
            else
            {
                addZeros = "00";

                zerosValue.gameObject.SetActive(true);
                currentValue.alignment = TextAlignmentOptions.TopLeft;
            }

            zerosValue.fontStyle = currentStyles;
            zerosValue.color = addZerosColor;
            zerosValue.text = addZeros;

            var currentValueColor = normalized > warningRate ? currentColor : warningColor;

            currentValue.fontStyle = currentStyles;
            currentValue.color = currentValueColor;
            currentValue.text = currentApprox.ToString("F0");

            maxSignValue.fontStyle = maximumStyles;
            maxSignValue.color = maxColor;

            //Set Maximum float and color and Style to String
            maxValue.fontStyle = maximumStyles;
            maxValue.color = maxColor;
            maxValue.text = maximum.ToString("F0");

            //Buff HUD display
            buffValue.gameObject.SetActive(buffRate != 0 && buffHUDSw);

            //Buff Arrow Up or Down
            upBuffArrow.gameObject.SetActive(buffRate > 0);
            downBuffArrow.gameObject.SetActive(buffRate < 0);

            //Buff Arrow Color
            upBuffArrow.color = upBuffArrowColor;
            downBuffArrow.color = downBuffArrowColor;

            //Buff Up Down Color 
            var buffColor = buffRate > 0 ? upBuffColor : downBuffColor;

            buffValue.color = buffColor;
            buffValue.text = buffRate.ToString("F2");

            //Arrow Animation display
            var arrow = !atMaximum && arrowAnimation;
            _animatorUpBuffArrow.SetBool(AnimatorHash.Active, arrow);
            _animatorDownBuffArrow.SetBool(AnimatorHash.Active, arrow);

            //Arrow Animation Speed
            _animatorUpBuffArrow.SetFloat(AnimatorHash.Speed, buffSpeed);
            _animatorDownBuffArrow.SetFloat(AnimatorHash.Speed, buffSpeed);

            //Arrow Animation Reverse
            _animatorUpBuffArrow.SetBool(AnimatorHash.Reverse, arrowAnimationReverse);
            _animatorDownBuffArrow.SetBool(AnimatorHash.Reverse, arrowAnimationReverse);

            //Glow display
            if (glow != null)
            {
                glow.gameObject.SetActive(normalized < warningRate);
            }
        }

#endif
    }
}