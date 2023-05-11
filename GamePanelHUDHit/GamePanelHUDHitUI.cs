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
        public bool DamageHUDSw;

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

        public Color DamageInfoColor;

        public Color ArmorDamageInfoColor;

        public FontStyles DamageStyles;

        public FontStyles ArmorDamageStyles;

        private RectTransform LeftUpRect;

        private RectTransform LeftDownRect;

        private RectTransform RightUpRect;

        private RectTransform RightDownRect;

        private RectTransform LeftUpHeadRect;

        private RectTransform LeftDownHeadRect;

        private RectTransform RightUpHeadRect;

        private RectTransform RightDownHeadRect;

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
        private Image _RightDown;

        [SerializeField]
        private Image _LeftUpHead;

        [SerializeField]
        private Image _LeftDownHead;

        [SerializeField]
        private Image _RightUpHead;

        [SerializeField]
        private Image _RightDownHead;

        private Animator Animator_HitUI;

        void Start()
        {
            Animator_HitUI = GetComponent<Animator>();

            var parent = _HpValue.transform.parent;
            _Hp = parent;
            _Armor = _ArmorValue.transform.parent;

            LeftUpRect = _LeftUp.GetComponent<RectTransform>();
            LeftDownRect = _LeftDown.GetComponent<RectTransform>();
            RightUpRect = _RightUp.GetComponent<RectTransform>();
            RightDownRect = _RightDown.GetComponent<RectTransform>();

            LeftUpHeadRect = _LeftUpHead.GetComponent<RectTransform>();
            LeftDownHeadRect = _LeftDownHead.GetComponent<RectTransform>();
            RightUpHeadRect = _RightUpHead.GetComponent<RectTransform>();
            RightDownHeadRect = _RightDownHead.GetComponent<RectTransform>();

            DamageValueRect = parent.parent.GetComponent<RectTransform>();

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
            Vector2 leftUpPos = new Vector2(-HitAnchoredPosition.x, HitAnchoredPosition.y);
            Vector2 leftDownPos = new Vector2(-HitAnchoredPosition.x, -HitAnchoredPosition.y);
            Vector2 rightUpPos = HitAnchoredPosition;
            Vector2 rightDownPos = new Vector2(HitAnchoredPosition.x, -HitAnchoredPosition.y);

            Vector3 leftUpRot = new Vector3(-HitLocalRotation.x, HitLocalRotation.y, HitLocalRotation.z);
            Vector3 leftDownRot = new Vector3(-HitLocalRotation.x, -HitLocalRotation.y, -HitLocalRotation.z);
            Vector3 rightUpRot = new Vector3(HitLocalRotation.x, HitLocalRotation.y, -HitLocalRotation.z);
            Vector3 rightDownRot = new Vector3(HitLocalRotation.x, -HitLocalRotation.y, HitLocalRotation.z);

            LeftUpRect.sizeDelta = HitSizeDelta;
            LeftDownRect.sizeDelta = HitSizeDelta;
            RightUpRect.sizeDelta = HitSizeDelta;
            RightDownRect.sizeDelta = HitSizeDelta;

            LeftUpRect.anchoredPosition = leftUpPos;
            LeftDownRect.anchoredPosition = leftDownPos;
            RightUpRect.anchoredPosition = rightUpPos;
            RightDownRect.anchoredPosition = rightDownPos;

            LeftUpRect.localEulerAngles = leftUpRot;
            LeftDownRect.localEulerAngles = leftDownRot;
            RightUpRect.localEulerAngles = rightUpRot;
            RightDownRect.localEulerAngles = rightDownRot;

            LeftUpRect.localScale = HitLocalScale;
            LeftDownRect.localScale = HitLocalScale;
            RightUpRect.localScale = HitLocalScale;
            RightDownRect.localScale = HitLocalScale;

            LeftUpHeadRect.sizeDelta = HitHeadSizeDelta;
            LeftDownHeadRect.sizeDelta = HitHeadSizeDelta;
            RightUpHeadRect.sizeDelta = HitHeadSizeDelta;
            RightDownHeadRect.sizeDelta = HitHeadSizeDelta;

            LeftUpHeadRect.anchoredPosition = leftUpPos;
            LeftDownHeadRect.anchoredPosition = leftDownPos;
            RightUpHeadRect.anchoredPosition = rightUpPos;
            RightDownHeadRect.anchoredPosition = rightDownPos;

            LeftUpHeadRect.localEulerAngles = leftUpRot;
            LeftDownHeadRect.localEulerAngles = leftDownRot;
            RightUpHeadRect.localEulerAngles = rightUpRot;
            RightDownHeadRect.localEulerAngles = rightDownRot;

            LeftUpHeadRect.localScale = HitLocalScale;
            LeftDownHeadRect.localScale = HitLocalScale;
            RightUpHeadRect.localScale = HitLocalScale;
            RightDownHeadRect.localScale = HitLocalScale;

            DamageValueRect.anchoredPosition = HitDamageAnchoredPosition;
            DamageValueRect.sizeDelta = HitDamageSizeDelta;
            DamageValueRect.localScale = HitDamageLocalScale;

            _Hp.gameObject.SetActive(DamageHUDSw);

            _HpValue.fontStyle = DamageStyles;
            _HpValue.color = DamageInfoColor;
            _HpValue.text = Damage.ToString("F0");

            _Armor.gameObject.SetActive(HasArmorHit && DamageHUDSw);

            _ArmorValue.fontStyle = ArmorDamageStyles;
            _ArmorValue.color = ArmorDamageInfoColor;
            _ArmorValue.text = ArmorDamage.ToString("F2");

            Animator_HitUI.SetFloat(AnimatorHash.ActiveSpeed, ActiveSpeed);
            Animator_HitUI.SetFloat(AnimatorHash.EndSpeed, EndSpeed);
            Animator_HitUI.SetFloat(AnimatorHash.DeadSpeed, DeadSpeed);
        }
#if !UNITY_EDITOR

        public void HitTrigger(bool isHead, GamePanelHUDHitPlugin.HitInfo hitInfo, GamePanelHUDHitPlugin.HitInfo.Direction direction)
        {
            Color hitColor;
            switch (hitInfo.HitType)
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
                default:
                    hitColor = Color.black;
                    break;
            }

            HitColor(hitColor);

            HitHead(isHead);

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

        void HitHead(bool sw)
        {
            _LeftUpHead.gameObject.SetActive(sw);
            _LeftDownHead.gameObject.SetActive(sw);
            _LeftUpHead.gameObject.SetActive(sw);
            _RightUpHead.gameObject.SetActive(sw);
            _RightDownHead.gameObject.SetActive(sw);
        }

        void HitColor(Color color)
        {
            _LeftUp.color = color;
            _LeftDown.color = color;
            _RightUp.color = color;
            _RightDown.color = color;

            _LeftUpHead.color = color;
            _LeftDownHead.color = color;
            _RightUpHead.color = color;
            _RightDownHead.color = color;
        }

        public void HitDeadTirgger()
        {
            Animator_HitUI.SetTrigger(AnimatorHash.ActiveDead);
        }
#endif
    }
}
