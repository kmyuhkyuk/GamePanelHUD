using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if !UNITY_EDITOR
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
#endif

namespace GamePanelHUDHit
{
    public class GamePanelHUDHitUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool DamageHUDSW;

        public bool HasArmorHit;

        public float Damage;

        public float ArmorDamage;

        public float ActiveSpeed;

        public float EndSpeed;

        public float DeadSpeed;

        public Vector2 HitAnchoredPosition;

        public Vector2 HitSizeDelta;

        public Vector2 HitHeadSizeDelta;

        public Vector2 HitLocalScale;

        public Vector2 HitDamageAnchoredPosition;

        public Vector2 HitDamageSizeDelta;

        public Vector2 HitDamageLocalScale;

        public Vector3 HitLocalRotation;

        public Color DamageColor;

        public Color ArmorDamageColor;

        public Color DeadColor;

        public Color HeadColor;

        public string DamageInfoColor;

        public string ArmorDamageInfoColor;

        public FontStyles DamageStyles;

        public FontStyles ArmorDamageStyles;

        private RectTransform LeftUp;

        private RectTransform LeftDown;

        private RectTransform RightUp;

        private RectTransform RightDowm;

        private RectTransform LeftUpHead;

        private RectTransform LeftDownHead;

        private RectTransform RightUpHead;

        private RectTransform RightDowmHead;

        private RectTransform DamageValue;

        private Transform _Hp;

        private Transform _Armor;

        [SerializeField]
        private TMP_Text _HpValue;

        [SerializeField]
        private TMP_Text _ArmorValue;

        [SerializeField]
        private Image _LeftUp;

        [SerializeField]
        private Image _LeftDown;

        [SerializeField]
        private Image _RightUp;

        [SerializeField]
        private Image _RightDowm;

        [SerializeField]
        private Image _LeftUpHead;

        [SerializeField]
        private Image _LeftDownHead;

        [SerializeField]
        private Image _RightUpHead;

        [SerializeField]
        private Image _RightDowmHead;

        private Animator Animator_HitUI;

        private readonly StringBuilderData StringBuilderDatas = new StringBuilderData();

#if !UNITY_EDITOR
        void Start()
        {
            Animator_HitUI = GetComponent<Animator>();

            _Hp = _HpValue.transform.parent;
            _Armor = _ArmorValue.transform.parent;

            LeftUp = _LeftUp.GetComponent<RectTransform>();
            LeftDown = _LeftDown.GetComponent<RectTransform>();
            RightUp = _RightUp.GetComponent<RectTransform>();
            RightDowm = _RightDowm.GetComponent<RectTransform>();

            LeftUpHead = _LeftUpHead.GetComponent<RectTransform>();
            LeftDownHead = _LeftDownHead.GetComponent<RectTransform>();
            RightUpHead = _RightUpHead.GetComponent<RectTransform>();
            RightDowmHead = _RightDowmHead.GetComponent<RectTransform>();

            DamageValue = _HpValue.transform.parent.parent.GetComponent<RectTransform>();

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            HitUI();
        }

        void HitUI()
        {
            LeftUp.sizeDelta = HitSizeDelta;
            LeftDown.sizeDelta = HitSizeDelta;
            RightUp.sizeDelta = HitSizeDelta;
            RightDowm.sizeDelta = HitSizeDelta;

            LeftUp.anchoredPosition = new Vector2(-HitAnchoredPosition.x, HitAnchoredPosition.y);
            LeftDown.anchoredPosition = new Vector2(-HitAnchoredPosition.x, -HitAnchoredPosition.y);
            RightUp.anchoredPosition = new Vector2(HitAnchoredPosition.x, HitAnchoredPosition.y);
            RightDowm.anchoredPosition = new Vector2(HitAnchoredPosition.x, -HitAnchoredPosition.y);

            LeftUp.localEulerAngles = new Vector3(-HitLocalRotation.x, HitLocalRotation.y, HitLocalRotation.z);
            LeftDown.localEulerAngles = new Vector3(-HitLocalRotation.x, -HitLocalRotation.y, -HitLocalRotation.z);
            RightUp.localEulerAngles = new Vector3(HitLocalRotation.x, HitLocalRotation.y, -HitLocalRotation.z);
            RightDowm.localEulerAngles = new Vector3(HitLocalRotation.x, -HitLocalRotation.y, HitLocalRotation.z);

            LeftUp.localScale = HitLocalScale;
            LeftDown.localScale = HitLocalScale;
            RightUp.localScale = HitLocalScale;
            RightDowm.localScale = HitLocalScale;

            LeftUpHead.sizeDelta = HitHeadSizeDelta;
            LeftDownHead.sizeDelta = HitHeadSizeDelta;
            RightUpHead.sizeDelta = HitHeadSizeDelta;
            RightDowmHead.sizeDelta = HitHeadSizeDelta;

            LeftUpHead.anchoredPosition = new Vector2(-HitAnchoredPosition.x, HitAnchoredPosition.y);
            LeftDownHead.anchoredPosition = new Vector2(-HitAnchoredPosition.x, -HitAnchoredPosition.y);
            RightUpHead.anchoredPosition = new Vector2(HitAnchoredPosition.x, HitAnchoredPosition.y);
            RightDowmHead.anchoredPosition = new Vector2(HitAnchoredPosition.x, -HitAnchoredPosition.y);

            LeftUpHead.localEulerAngles = new Vector3(-HitLocalRotation.x, HitLocalRotation.y, HitLocalRotation.z);
            LeftDownHead.localEulerAngles = new Vector3(-HitLocalRotation.x, -HitLocalRotation.y, -HitLocalRotation.z);
            RightUpHead.localEulerAngles = new Vector3(HitLocalRotation.x, HitLocalRotation.y, -HitLocalRotation.z);
            RightDowmHead.localEulerAngles = new Vector3(HitLocalRotation.x, -HitLocalRotation.y, HitLocalRotation.z);

            LeftUpHead.localScale = HitLocalScale;
            LeftDownHead.localScale = HitLocalScale;
            RightUpHead.localScale = HitLocalScale;
            RightDowmHead.localScale = HitLocalScale;

            DamageValue.anchoredPosition = HitDamageAnchoredPosition;
            DamageValue.sizeDelta = HitDamageSizeDelta;
            DamageValue.localScale = HitDamageLocalScale;

            _Hp.gameObject.SetActive(DamageHUDSW);

            _HpValue.fontStyle = DamageStyles;
            _HpValue.text = StringBuilderDatas._DamageValue.StringConcat("<color=", DamageInfoColor, ">", Math.Round(Damage), "</color>");

            _Armor.gameObject.SetActive(HasArmorHit && DamageHUDSW);

            _ArmorValue.fontStyle = ArmorDamageStyles;
            _ArmorValue.text = StringBuilderDatas._ArmorValue.StringConcat("<color=", ArmorDamageInfoColor, ">", ArmorDamage.ToString("F2"), "</color>");

            Animator_HitUI.SetFloat(AnimatorHash.ActiveSpeed, ActiveSpeed);
            Animator_HitUI.SetFloat(AnimatorHash.EndSpeed, EndSpeed);
            Animator_HitUI.SetFloat(AnimatorHash.DeadSpeed, DeadSpeed);
        }

        public void HitTirgger(bool ishead, GamePanelHUDHitPlugin.HitInfo hitinfo)
        {
            switch (hitinfo.HitType)
            {
                case GamePanelHUDHitPlugin.HitInfo.Hit.OnlyHp:
                    _LeftUp.color = DamageColor;
                    _LeftDown.color = DamageColor;
                    _RightUp.color = DamageColor;
                    _RightDowm.color = DamageColor;

                    _LeftUpHead.color = DamageColor;
                    _LeftDownHead.color = DamageColor;
                    _RightUpHead.color = DamageColor;
                    _RightDowmHead.color = DamageColor;
                    break;
                case GamePanelHUDHitPlugin.HitInfo.Hit.HasArmorHit:
                    _LeftUp.color = ArmorDamageColor;
                    _LeftDown.color = ArmorDamageColor;
                    _RightUp.color = ArmorDamageColor;
                    _RightDowm.color = ArmorDamageColor;

                    _LeftUpHead.color = ArmorDamageColor;
                    _LeftDownHead.color = ArmorDamageColor;
                    _RightUpHead.color = ArmorDamageColor;
                    _RightDowmHead.color = ArmorDamageColor;
                    break;
                case GamePanelHUDHitPlugin.HitInfo.Hit.Dead:
                    _LeftUp.color = DeadColor;
                    _LeftDown.color = DeadColor;
                    _RightUp.color = DeadColor;
                    _RightDowm.color = DeadColor;

                    _LeftUpHead.color = DeadColor;
                    _LeftDownHead.color = DeadColor;
                    _RightUpHead.color = DeadColor;
                    _RightDowmHead.color = DeadColor;
                    break;
                case GamePanelHUDHitPlugin.HitInfo.Hit.Head:
                    _LeftUp.color = HeadColor;
                    _LeftDown.color = HeadColor;
                    _RightUp.color = HeadColor;
                    _RightDowm.color = HeadColor;

                    _LeftUpHead.color = HeadColor;
                    _LeftDownHead.color = HeadColor;
                    _RightUpHead.color = HeadColor;
                    _RightDowmHead.color = HeadColor;
                    break;
            }

            _LeftUpHead.gameObject.SetActive(ishead);
            _LeftDownHead.gameObject.SetActive(ishead);
            _LeftUpHead.gameObject.SetActive(ishead);
            _RightUpHead.gameObject.SetActive(ishead);
            _RightDowmHead.gameObject.SetActive(ishead);

            switch (hitinfo.HitDirectionType)
            {
                case GamePanelHUDHitPlugin.HitInfo.Direction.Center:
                    Animator_HitUI.SetTrigger(AnimatorHash.Active);
                    break;
                case GamePanelHUDHitPlugin.HitInfo.Direction.Left:
                    Animator_HitUI.SetTrigger(AnimatorHash.ActiveLeft);
                    break;
                case GamePanelHUDHitPlugin.HitInfo.Direction.Right:
                    Animator_HitUI.SetTrigger(AnimatorHash.ActiveRight);
                    break;
            }
        }

        public void HitDeadTirgger()
        {
            Animator_HitUI.SetTrigger(AnimatorHash.ActiveDead);
        }
#endif

        public class StringBuilderData
        {
            public StringBuilder _DamageValue = new StringBuilder(128);
            public StringBuilder _ArmorValue = new StringBuilder(128);
        }
    }
}
