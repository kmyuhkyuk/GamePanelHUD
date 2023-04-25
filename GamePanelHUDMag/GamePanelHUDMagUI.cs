using UnityEngine;
using TMPro;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDMag
{
    public class GamePanelHUDMagUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool WeaponNameAlways;

        public bool AmmoTypeHUDSW;

        public bool FireModeHUDSW;

        public bool ZeroWarning;

        public bool WeaponTirgger;

        public int Current;

        public int Maximum;

        public int Patron;

        public float Normalized;

        public string WeaponName;

        public string AmmoType;

        public string FireMode;

        public float WarningRate10;

        public float WarningRate100;

        public float WeaponNameSpeed;

        public float ZeroWarningSpeed;

        public Color CurrentColor;

        public Color MaxColor;

        public Color PatronColor;

        public Color WeaponNameColor;

        public Color AmmoTypeColor;

        public Color FireModeColor;

        public Color AddZerosColor;

        public Color WarningColor;

        public FontStyles CurrentStyles;

        public FontStyles MaximumStyles;

        public FontStyles PatronStyles;

        public FontStyles WeaponNameStyles;

        public FontStyles AmmoTypeStyles;

        public FontStyles FireModeStyles;

        [SerializeField]
        private TMP_Text _ZerosValue;

        [SerializeField]
        private TMP_Text _CurrentValue;

        [SerializeField]
        private TMP_Text _MaxSignValue;

        [SerializeField]
        private TMP_Text _MaxValue;

        [SerializeField]
        private TMP_Text _PatronValue;

        [SerializeField]
        private TMP_Text _PatronSignValue;

        [SerializeField]
        private TMP_Text _WeaponNameValue;

        [SerializeField]
        private TMP_Text _AmmoTypeValue;

        [SerializeField]
        private TMP_Text _FiremodeValue;

        private Animator Animator_WeaponName;

        private Animator Animator_Current;

        private Transform PatronPanel;

        private Transform FiremodePanel;

        void Start()
        {
            Animator_WeaponName = _WeaponNameValue.transform.parent.GetComponent<Animator>();
            Animator_Current = _CurrentValue.GetComponent<Animator>();

            PatronPanel = _PatronValue.transform.parent;
            FiremodePanel = _FiremodeValue.transform.parent;

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
            MagUI();
        }

        void MagUI()
#endif
#if UNITY_EDITOR
        void Update()
#endif
        {
            //Set Current float and color and Style to String
            Color currentColor;
            if ((Normalized > WarningRate10 && Maximum <= 10 || Normalized >= WarningRate100 && Maximum > 10) && Current != 0)
            {
                currentColor = CurrentColor;
            }
            else
            {
                currentColor = WarningColor;
            }

            if (Current < 10 && Maximum > 10)
            {
                _ZerosValue.gameObject.SetActive(true);
                _CurrentValue.alignment = TextAlignmentOptions.TopLeft;
            }
            else
            {
                _ZerosValue.gameObject.SetActive(false);
                _CurrentValue.alignment = TextAlignmentOptions.TopRight;
            }

            _ZerosValue.fontStyle = CurrentStyles;
            _ZerosValue.color = AddZerosColor;

            _CurrentValue.fontStyle = CurrentStyles;
            _CurrentValue.color = currentColor;
            _CurrentValue.text = Current.ToString();

            _MaxSignValue.fontStyle = MaximumStyles;
            _MaxSignValue.color = MaxColor;

            //Set Maximum float and color and Style to String
            _MaxValue.fontStyle = MaximumStyles;
            _MaxValue.color = MaxColor;
            _MaxValue.text = Maximum.ToString();

            _PatronSignValue.fontStyle = PatronStyles;
            _PatronSignValue.color = PatronColor;

            //Patron HUD display
            PatronPanel.gameObject.SetActive(Patron > 0);

            //Set Patron float and color and Style to String
            _PatronValue.fontStyle = PatronStyles;
            _PatronValue.color = PatronColor;
            _PatronValue.text = Patron.ToString();

            //Set Weapon Name
            _WeaponNameValue.fontStyle = WeaponNameStyles;
            _WeaponNameValue.color = WeaponNameColor;
            _WeaponNameValue.text = WeaponName;

            Animator_WeaponName.SetBool(AnimatorHash.Always, WeaponNameAlways);
            Animator_WeaponName.SetFloat(AnimatorHash.Speed, WeaponNameSpeed);

            Animator_Current.SetBool(AnimatorHash.Zero, Current == 0 && Patron == 0 && ZeroWarning);
            Animator_Current.SetFloat(AnimatorHash.Speed, ZeroWarningSpeed);

            //Fire Mode HUD display
            FiremodePanel.gameObject.SetActive(FireModeHUDSW);

            //Set Fire Mode
            _FiremodeValue.fontStyle = FireModeStyles;
            _FiremodeValue.color = FireModeColor;
            _FiremodeValue.text = FireMode;

            _AmmoTypeValue.gameObject.SetActive(AmmoTypeHUDSW);

            _AmmoTypeValue.fontStyle = AmmoTypeStyles;
            _AmmoTypeValue.color = AmmoTypeColor;
            _AmmoTypeValue.text = AmmoType;

            if (WeaponTirgger)
            {
                Animator_WeaponName.SetTrigger(AnimatorHash.Active);

                WeaponTirgger = false;
            }
        }
    }
}
