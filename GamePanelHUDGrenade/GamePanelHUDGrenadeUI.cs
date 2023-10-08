using TMPro;
using UnityEngine;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore;

#endif

namespace GamePanelHUDGrenade
{
    public class GamePanelHUDGrenadeUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR

        private static GamePanelHUDCorePlugin.HUDCoreClass HUDCore => GamePanelHUDCorePlugin.HUDCore;

#endif

        public bool zeroWarning;

        public int grenadeAmount;

        public Color grenadeColor;

        public Color warningColor;

        public FontStyles grenadeStyles;

        [SerializeField]
#pragma warning disable CS0649
        private TMP_Text countValue;
#pragma warning restore CS0649

#if !UNITY_EDITOR

        private void Start()
        {
            HUDCore.UpdateManger.Register(this);
        }

        private void OnEnable()
        {
            HUDCore.UpdateManger.Run(this);
        }

        private void OnDisable()
        {
            HUDCore.UpdateManger.Stop(this);
        }

        public void CustomUpdate()
        {
            GrenadeUI();
        }

        private void GrenadeUI()
#endif
#if UNITY_EDITOR
        void Update()
#endif
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
    }
}