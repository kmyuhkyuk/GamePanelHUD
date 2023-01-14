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
            LeftUp.sizeDelta = HitSizeDelta;
            LeftDown.sizeDelta = HitSizeDelta;
            RightUp.sizeDelta = HitSizeDelta;
            RightDowm.sizeDelta = HitSizeDelta;

            Vector2 leftUpPos = new Vector2(-HitAnchoredPosition.x, HitAnchoredPosition.y);
            Vector2 leftDownPos = new Vector2(-HitAnchoredPosition.x, -HitAnchoredPosition.y);
            Vector2 rightUpPos = new Vector2(HitAnchoredPosition.x, HitAnchoredPosition.y);
            Vector2 rightDownPos = new Vector2(HitAnchoredPosition.x, -HitAnchoredPosition.y);

            Vector3 leftUpRot = new Vector3(-HitLocalRotation.x, HitLocalRotation.y, HitLocalRotation.z);
            Vector3 leftDownRot = new Vector3(-HitLocalRotation.x, -HitLocalRotation.y, -HitLocalRotation.z);
            Vector3 rightUpRot = new Vector3(HitLocalRotation.x, HitLocalRotation.y, -HitLocalRotation.z);
            Vector3 rightDownRot = new Vector3(HitLocalRotation.x, -HitLocalRotation.y, HitLocalRotation.z);

            LeftUp.anchoredPosition = leftUpPos;
            LeftDown.anchoredPosition = leftDownPos;
            RightUp.anchoredPosition = rightUpPos;
            RightDowm.anchoredPosition = rightDownPos;

            LeftUp.localEulerAngles = leftUpRot;
            LeftDown.localEulerAngles = leftDownRot;
            RightUp.localEulerAngles = rightUpRot;
            RightDowm.localEulerAngles = rightDownRot;

            LeftUp.localScale = HitLocalScale;
            LeftDown.localScale = HitLocalScale;
            RightUp.localScale = HitLocalScale;
            RightDowm.localScale = HitLocalScale;

            LeftUpHead.sizeDelta = HitHeadSizeDelta;
            LeftDownHead.sizeDelta = HitHeadSizeDelta;
            RightUpHead.sizeDelta = HitHeadSizeDelta;
            RightDowmHead.sizeDelta = HitHeadSizeDelta;

            LeftUpHead.anchoredPosition = leftUpPos;
            LeftDownHead.anchoredPosition = leftDownPos;
            RightUpHead.anchoredPosition = rightUpPos;
            RightDowmHead.anchoredPosition = rightDownPos;

            LeftUpHead.localEulerAngles = leftUpRot;
            LeftDownHead.localEulerAngles = leftDownRot;
            RightUpHead.localEulerAngles = rightUpRot;
            RightDowmHead.localEulerAngles = rightDownRot;

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
