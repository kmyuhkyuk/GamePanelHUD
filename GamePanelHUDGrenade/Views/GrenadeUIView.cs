using TMPro;
using UnityEngine;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore.Models;

#endif

namespace GamePanelHUDGrenade.Views
{
    public class GrenadeUIView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool zeroWarning;

        public int grenadeAmount;

        public Color grenadeColor;

        public Color warningColor;

        public FontStyles grenadeStyles;

#pragma warning disable CS0649
        [SerializeField] private TMP_Text countValue;
#pragma warning restore CS0649
#if !UNITY_EDITOR

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
            //Set GrenadeAmount int and color and Style to String
            Color grenadeValueColor;
            if (zeroWarning && grenadeAmount > 0 || !zeroWarning)
            {
                grenadeValueColor = grenadeColor;
            }
            else
            {
                grenadeValueColor = warningColor;
            }

            countValue.fontStyle = grenadeStyles;
            countValue.color = grenadeValueColor;
            countValue.text = grenadeAmount.ToString();
        }

#endif
    }
}