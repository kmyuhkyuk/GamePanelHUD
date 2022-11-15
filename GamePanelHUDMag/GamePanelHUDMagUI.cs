using System.Text;
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

        public string CurrentColor;

        public string MaxColor;

        public string PatronColor;

        public string WeaponNameColor;

        public string AmmoTypeColor;

        public string FireModeColor;

        public string AddZerosColor;

        public string WarningColor;

        public FontStyles CurrentStyles;

        public FontStyles MaximumStyles;

        public FontStyles PatronStyles;

        public FontStyles WeaponNameStyles;

        public FontStyles AmmoTypeStyles;

        public FontStyles FireModeStyles;

        [SerializeField]
        private TMP_Text _CurrentValue;

        [SerializeField]
        private TMP_Text _MaxValue;

        [SerializeField]
        private TMP_Text _PatronValue;

        [SerializeField]
        private TMP_Text _WeaponValue;

        [SerializeField]
        private TMP_Text _AmmoValue;

        [SerializeField]
        private TMP_Text _FiremodeValue;

        private Animator Animator_Weapon;

        private Animator Animator_Current;

        private readonly StringBuilderData StringBuilderDatas = new StringBuilderData();

#if !UNITY_EDITOR
        void Start()
        {
            Animator_Weapon = _WeaponValue.transform.parent.parent.GetComponent<Animator>();
            Animator_Current = _CurrentValue.GetComponent<Animator>();

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
            MagUI();
        }

        void MagUI()
#endif
#if UNITY_EDITOR
        void Update()
#endif
        {
            //Set Current float and color and Style to String
            string addZeros;
            if (Current < 10 && Maximum > 10)
            {
                addZeros = "0";
            }
            else
            {
                addZeros = "";
            }

            string currentColor;
            if ((Normalized > WarningRate10 && Maximum <= 10 || Normalized >= WarningRate100 && Maximum > 10) && Current != 0)
            {
                currentColor = CurrentColor;
            }
            else
            {
                currentColor = WarningColor;
            }

            _CurrentValue.fontStyle = CurrentStyles;
            _CurrentValue.text = StringBuilderDatas._CurrentValue.Chcek("<color=", AddZerosColor, ">", addZeros, "</color>", "<color=", currentColor, ">", Current, "</color>");

            //Set Maximum float and color and Style to String
            _MaxValue.fontStyle = MaximumStyles;
            _MaxValue.text = StringBuilderDatas._MaxValue.Chcek("<color=", MaxColor, ">", "/", Maximum, "</color>");

            //Patron HUD display
            _PatronValue.gameObject.SetActive(Patron > 0);

            //Set Patron float and color and Style to String
            _PatronValue.fontStyle = PatronStyles;
            _PatronValue.text = StringBuilderDatas._PatronValue.Chcek("<color=", PatronColor, ">", "+", Patron, "</color>");

            //Set Weapon Name
            _WeaponValue.fontStyle = WeaponNameStyles;
            _WeaponValue.text = StringBuilderDatas._WeaponValue.Chcek("<color=", WeaponNameColor, ">", WeaponName, "</color>");

            Animator_Weapon.SetBool(AnimatorHash.Always, WeaponNameAlways);
            Animator_Weapon.SetFloat(AnimatorHash.Speed, WeaponNameSpeed);

            Animator_Current.SetBool(AnimatorHash.Zero, Current == 0 && Patron == 0 && ZeroWarning);
            Animator_Current.SetFloat(AnimatorHash.Speed, ZeroWarningSpeed);

            //Fire Mode HUD display
            _FiremodeValue.gameObject.SetActive(FireModeHUDSW);

            //Set Fire Mode
            _FiremodeValue.fontStyle = FireModeStyles;
            _FiremodeValue.text = StringBuilderDatas._FiremodeValue.Chcek("<color=", FireModeColor, ">", FireMode, "</color>");

            _AmmoValue.gameObject.SetActive(AmmoTypeHUDSW);

            _AmmoValue.fontStyle = AmmoTypeStyles;
            _AmmoValue.text = StringBuilderDatas._AmmoValue.Chcek("<color=", AmmoTypeColor, ">", AmmoType, "</color>");

            if (WeaponTirgger)
            {
                Animator_Weapon.SetTrigger(AnimatorHash.Active);

                WeaponTirgger = false;
            }
        }

        public class StringBuilderData
        {
            public StringBuilderHelp.StringChange _CurrentValue = new StringBuilderHelp.StringChange(128);
            public StringBuilderHelp.StringChange _MaxValue = new StringBuilderHelp.StringChange(128);
            public StringBuilderHelp.StringChange _PatronValue = new StringBuilderHelp.StringChange(128);
            public StringBuilderHelp.StringChange _WeaponValue = new StringBuilderHelp.StringChange(128);
            public StringBuilderHelp.StringChange _FiremodeValue = new StringBuilderHelp.StringChange(128);
            public StringBuilderHelp.StringChange _AmmoValue = new StringBuilderHelp.StringChange(128);
        }
    }
}
