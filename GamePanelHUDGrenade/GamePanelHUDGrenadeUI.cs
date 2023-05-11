using UnityEngine;
using TMPro;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDGrenade
{
    public class GamePanelHUDGrenadeUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool ZeroWarning;

        public int GrenadeAmount;

        public Color GrenadeColor;

        public Color WarningColor;

        public FontStyles GrenadeStyles;

        [SerializeField]
#pragma warning disable CS0649
        private TMP_Text _GrenadeValue;
#pragma warning restore CS0649

#if !UNITY_EDITOR
        void Start()
        {
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
            GrenadeUI();
        }

        void GrenadeUI()
#endif
#if UNITY_EDITOR
        void Update()
#endif
        {
            //Set GrenadeAmount int and color and Style to String
            Color grenadeColor;
            if (ZeroWarning && GrenadeAmount > 0 || !ZeroWarning)
            {
                grenadeColor = GrenadeColor;
            }
            else
            {
                grenadeColor = WarningColor;
            }

            _GrenadeValue.fontStyle = GrenadeStyles;
            _GrenadeValue.color = grenadeColor;
            _GrenadeValue.text = GrenadeAmount.ToString();
        }
    }
}
