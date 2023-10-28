using TMPro;
using UnityEngine;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDCore.Utils;

#endif

namespace GamePanelHUDWeapon.Views
{
    public class WeaponUIView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool weaponNameAlways;

        public bool ammoTypeHUDSw;

        public bool fireModeHUDSw;

        public bool zeroWarning;

        public bool weaponIsTrigger;

        public int current;

        public int maximum;

        public int patron;

        public float normalized;

        public string weaponName;

        public string ammoType;

        public string fireMode;

        public float warningRate10;

        public float warningRate100;

        public float weaponNameSpeed;

        public float zeroWarningSpeed;

        public Color currentColor;

        public Color maxColor;

        public Color patronColor;

        public Color weaponNameColor;

        public Color ammoTypeColor;

        public Color fireModeColor;

        public Color addZerosColor;

        public Color warningColor;

        public FontStyles currentStyles;

        public FontStyles maximumStyles;

        public FontStyles patronStyles;

        public FontStyles weaponNameStyles;

        public FontStyles ammoTypeStyles;

        public FontStyles fireModeStyles;

        [SerializeField] private TMP_Text zerosValue;

        [SerializeField] private TMP_Text currentValue;

        [SerializeField] private TMP_Text maxSignValue;

        [SerializeField] private TMP_Text maxValue;

        [SerializeField] private TMP_Text patronSignValue;

        [SerializeField] private TMP_Text patronValue;

        [SerializeField] private TMP_Text weaponNameValue;

        [SerializeField] private TMP_Text ammoTypeValue;

        [SerializeField] private TMP_Text fireModeValue;

        private Animator _animatorWeaponName;

        private Animator _animatorCurrent;

        private Transform _patronPanelTransform;

        private Transform _fireModePanelTransform;

#if !UNITY_EDITOR
        private void Awake()
        {
            _animatorWeaponName = weaponNameValue.transform.parent.GetComponent<Animator>();
            _animatorCurrent = currentValue.GetComponent<Animator>();

            _patronPanelTransform = patronValue.transform.parent;
            _fireModePanelTransform = fireModeValue.transform.parent;
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
            Color currentValueColor;
            if (current != 0 && (normalized > warningRate10 && maximum <= 10 ||
                                 normalized >= warningRate100 && maximum > 10))
            {
                currentValueColor = currentColor;
            }
            else
            {
                currentValueColor = warningColor;
            }

            if (current < 10 && maximum > 10)
            {
                zerosValue.gameObject.SetActive(true);
                currentValue.alignment = TextAlignmentOptions.TopLeft;
            }
            else
            {
                zerosValue.gameObject.SetActive(false);
                currentValue.alignment = TextAlignmentOptions.TopRight;
            }

            zerosValue.fontStyle = currentStyles;
            zerosValue.color = addZerosColor;

            currentValue.fontStyle = currentStyles;
            currentValue.color = currentValueColor;
            currentValue.text = current.ToString();

            maxSignValue.fontStyle = maximumStyles;
            maxSignValue.color = maxColor;

            //Set Maximum float and color and Style to String
            maxValue.fontStyle = maximumStyles;
            maxValue.color = maxColor;
            maxValue.text = maximum.ToString();

            patronSignValue.fontStyle = patronStyles;
            patronSignValue.color = patronColor;

            //Patron HUD display
            _patronPanelTransform.gameObject.SetActive(patron > 0);

            //Set Patron float and color and Style to String
            patronValue.fontStyle = patronStyles;
            patronValue.color = patronColor;
            patronValue.text = patron.ToString();

            //Set Weapon Name
            weaponNameValue.fontStyle = weaponNameStyles;
            weaponNameValue.color = weaponNameColor;
            weaponNameValue.text = weaponName;

            _animatorWeaponName.SetBool(AnimatorHash.Always, weaponNameAlways);
            _animatorWeaponName.SetFloat(AnimatorHash.Speed, weaponNameSpeed);

            _animatorCurrent.SetBool(AnimatorHash.Zero, current == 0 && patron == 0 && zeroWarning);
            _animatorCurrent.SetFloat(AnimatorHash.Speed, zeroWarningSpeed);

            //Fire Mode HUD display
            _fireModePanelTransform.gameObject.SetActive(fireModeHUDSw);

            //Set Fire Mode
            fireModeValue.fontStyle = fireModeStyles;
            fireModeValue.color = fireModeColor;
            fireModeValue.text = fireMode;

            ammoTypeValue.gameObject.SetActive(ammoTypeHUDSw);

            ammoTypeValue.fontStyle = ammoTypeStyles;
            ammoTypeValue.color = ammoTypeColor;
            ammoTypeValue.text = ammoType;

            if (weaponIsTrigger)
            {
                _animatorWeaponName.SetTrigger(AnimatorHash.Active);

                weaponIsTrigger = false;
            }
        }

#endif
    }
}