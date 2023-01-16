using System;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

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

        private RectTransform LeftUpRect;

        private RectTransform LeftDownRect;

        private RectTransform RightUpRect;

        private RectTransform RightDowmRect;

        private RectTransform LeftUpHeadRect;

        private RectTransform LeftDownHeadRect;

        private RectTransform RightUpHeadRect;

        private RectTransform RightDowmHeadRect;

        private RectTransform DamageValueRect;

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

        void Start()
        {
            Animator_HitUI = GetComponent<Animator>();

            _Hp = _HpValue.transform.parent;
            _Armor = _ArmorValue.transform.parent;

            LeftUpRect = _LeftUp.GetComponent<RectTransform>();
            LeftDownRect = _LeftDown.GetComponent<RectTransform>();
            RightUpRect = _RightUp.GetComponent<RectTransform>();
            RightDowmRect = _RightDowm.GetComponent<RectTransform>();

            LeftUpHeadRect = _LeftUpHead.GetComponent<RectTransform>();
            LeftDownHeadRect = _LeftDownHead.GetComponent<RectTransform>();
            RightUpHeadRect = _RightUpHead.GetComponent<RectTransform>();
            RightDowmHeadRect = _RightDowmHead.GetComponent<RectTransform>();

            DamageValueRect = _HpValue.transform.parent.parent.GetComponent<RectTransform>();

#if !UNITY_EDITOR
            GamePanelHUDCorePlugin.UpdateManger.Register(this);
#endif
        }
#if !UNITY_EDITOR

        public void IUpdate()
        {
            HitUI();
        }

        void HitUI()
#endif
#if UNITY_EDITOR
        void Update()
#endif
        {
            LeftUpRect.sizeDelta = HitSizeDelta;
            LeftDownRect.sizeDelta = HitSizeDelta;
            RightUpRect.sizeDelta = HitSizeDelta;
            RightDowmRect.sizeDelta = HitSizeDelta;

            Vector2 leftUpPos = new Vector2(-HitAnchoredPosition.x, HitAnchoredPosition.y);
            Vector2 leftDownPos = new Vector2(-HitAnchoredPosition.x, -HitAnchoredPosition.y);
            Vector2 rightUpPos = HitAnchoredPosition;
            Vector2 rightDownPos = new Vector2(HitAnchoredPosition.x, -HitAnchoredPosition.y);

            Vector3 leftUpRot = new Vector3(-HitLocalRotation.x, HitLocalRotation.y, HitLocalRotation.z);
            Vector3 leftDownRot = new Vector3(-HitLocalRotation.x, -HitLocalRotation.y, -HitLocalRotation.z);
            Vector3 rightUpRot = new Vector3(HitLocalRotation.x, HitLocalRotation.y, -HitLocalRotation.z);
            Vector3 rightDownRot = new Vector3(HitLocalRotation.x, -HitLocalRotation.y, HitLocalRotation.z);

            LeftUpRect.anchoredPosition = leftUpPos;
            LeftDownRect.anchoredPosition = leftDownPos;
            RightUpRect.anchoredPosition = rightUpPos;
            RightDowmRect.anchoredPosition = rightDownPos;

            LeftUpRect.localEulerAngles = leftUpRot;
            LeftDownRect.localEulerAngles = leftDownRot;
            RightUpRect.localEulerAngles = rightUpRot;
            RightDowmRect.localEulerAngles = rightDownRot;

            LeftUpRect.localScale = HitLocalScale;
            LeftDownRect.localScale = HitLocalScale;
            RightUpRect.localScale = HitLocalScale;
            RightDowmRect.localScale = HitLocalScale;

            LeftUpHeadRect.sizeDelta = HitHeadSizeDelta;
            LeftDownHeadRect.sizeDelta = HitHeadSizeDelta;
            RightUpHeadRect.sizeDelta = HitHeadSizeDelta;
            RightDowmHeadRect.sizeDelta = HitHeadSizeDelta;

            LeftUpHeadRect.anchoredPosition = leftUpPos;
            LeftDownHeadRect.anchoredPosition = leftDownPos;
            RightUpHeadRect.anchoredPosition = rightUpPos;
            RightDowmHeadRect.anchoredPosition = rightDownPos;

            LeftUpHeadRect.localEulerAngles = leftUpRot;
            LeftDownHeadRect.localEulerAngles = leftDownRot;
            RightUpHeadRect.localEulerAngles = rightUpRot;
            RightDowmHeadRect.localEulerAngles = rightDownRot;

            LeftUpHeadRect.localScale = HitLocalScale;
            LeftDownHeadRect.localScale = HitLocalScale;
            RightUpHeadRect.localScale = HitLocalScale;
            RightDowmHeadRect.localScale = HitLocalScale;

            DamageValueRect.anchoredPosition = HitDamageAnchoredPosition;
            DamageValueRect.sizeDelta = HitDamageSizeDelta;
            DamageValueRect.localScale = HitDamageLocalScale;

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
#if !UNITY_EDITOR

        public void HitTirgger(bool ishead, GamePanelHUDHitPlugin.HitInfo hitinfo, GamePanelHUDHitPlugin.HitInfo.Direction direction)
        {
            Color hitColor = DamageColor;

            switch (hitinfo.HitType)
            {
                case GamePanelHUDHitPlugin.HitInfo.Hit.OnlyHp:
                    hitColor = DamageColor;
                    break;
                case GamePanelHUDHitPlugin.HitInfo.Hit.HasArmorHit:
                    hitColor = ArmorDamageColor;
                    break;
                case GamePanelHUDHitPlugin.HitInfo.Hit.Dead:
                    hitColor = DeadColor;
                    break;
                case GamePanelHUDHitPlugin.HitInfo.Hit.Head:
                    hitColor = HeadColor;
                    break;
            }

            _LeftUp.color = hitColor;
            _LeftDown.color = hitColor;
            _RightUp.color = hitColor;
            _RightDowm.color = hitColor;

            _LeftUpHead.color = hitColor;
            _LeftDownHead.color = hitColor;
            _RightUpHead.color = hitColor;
            _RightDowmHead.color = hitColor;

            _LeftUpHead.gameObject.SetActive(ishead);
            _LeftDownHead.gameObject.SetActive(ishead);
            _LeftUpHead.gameObject.SetActive(ishead);
            _RightUpHead.gameObject.SetActive(ishead);
            _RightDowmHead.gameObject.SetActive(ishead);

            switch (direction)
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
