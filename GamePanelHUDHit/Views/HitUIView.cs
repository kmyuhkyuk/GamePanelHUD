using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR
using KmyTarkovUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDCore.Utils;
using GamePanelHUDHit.Models;

#endif

namespace GamePanelHUDHit.Views
{
    public class HitUIView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool damageHUDSw;

        public bool hasArmorHit;

        public float damage;

        public float armorDamage;

        public float activeSpeed;

        public float endSpeed;

        public float deadSpeed;

        public Vector2 hitAnchoredPosition;

        public Vector2 hitSizeDelta;

        public Vector2 hitHeadSizeDelta;

        public Vector2 hitLocalScale;

        public Vector2 hitDamageAnchoredPosition;

        public Vector2 hitDamageSizeDelta;

        public Vector2 hitDamageLocalScale;

        public Vector3 hitLocalRotation;

        public Color damageColor;

        public Color armorDamageColor;

        public Color deadColor;

        public Color headColor;

        public Color damageInfoColor;

        public Color armorDamageInfoColor;

        public FontStyles damageStyles;

        public FontStyles armorDamageStyles;

        private RectTransform _leftUpRect;

        private RectTransform _leftDownRect;

        private RectTransform _rightUpRect;

        private RectTransform _rightDownRect;

        private RectTransform _leftUpHeadRect;

        private RectTransform _leftDownHeadRect;

        private RectTransform _rightUpHeadRect;

        private RectTransform _rightDownHeadRect;

        private RectTransform _damageValueRect;

        private Transform _hpTransform;

        private Transform _armorTransform;

        [SerializeField] private TMP_Text hpValue;

        [SerializeField] private TMP_Text armorValue;

        [SerializeField] private Image leftUp;

        [SerializeField] private Image leftDown;

        [SerializeField] private Image rightUp;

        [SerializeField] private Image rightDown;

        [SerializeField] private Image leftUpHead;

        [SerializeField] private Image leftDownHead;

        [SerializeField] private Image rightUpHead;

        [SerializeField] private Image rightDownHead;

        private Animator _animatorHitUI;

#if !UNITY_EDITOR

        private void Awake()
        {
            _animatorHitUI = GetComponent<Animator>();

            var parent = hpValue.transform.parent;
            _hpTransform = parent;
            _armorTransform = armorValue.transform.parent;

            _leftUpRect = leftUp.GetComponent<RectTransform>();
            _leftDownRect = leftDown.GetComponent<RectTransform>();
            _rightUpRect = rightUp.GetComponent<RectTransform>();
            _rightDownRect = rightDown.GetComponent<RectTransform>();

            _leftUpHeadRect = leftUpHead.GetComponent<RectTransform>();
            _leftDownHeadRect = leftDownHead.GetComponent<RectTransform>();
            _rightUpHeadRect = rightUpHead.GetComponent<RectTransform>();
            _rightDownHeadRect = rightDownHead.GetComponent<RectTransform>();

            _damageValueRect = parent.parent.GetComponent<RectTransform>();
        }

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var leftUpPos = new Vector2(-hitAnchoredPosition.x, hitAnchoredPosition.y);
            var leftDownPos = new Vector2(-hitAnchoredPosition.x, -hitAnchoredPosition.y);
            var rightUpPos = hitAnchoredPosition;
            var rightDownPos = new Vector2(hitAnchoredPosition.x, -hitAnchoredPosition.y);

            var leftUpRot = new Vector3(-hitLocalRotation.x, hitLocalRotation.y, hitLocalRotation.z);
            var leftDownRot = new Vector3(-hitLocalRotation.x, -hitLocalRotation.y, -hitLocalRotation.z);
            var rightUpRot = new Vector3(hitLocalRotation.x, hitLocalRotation.y, -hitLocalRotation.z);
            var rightDownRot = new Vector3(hitLocalRotation.x, -hitLocalRotation.y, hitLocalRotation.z);

            _leftUpRect.sizeDelta = hitSizeDelta;
            _leftDownRect.sizeDelta = hitSizeDelta;
            _rightUpRect.sizeDelta = hitSizeDelta;
            _rightDownRect.sizeDelta = hitSizeDelta;

            _leftUpRect.anchoredPosition = leftUpPos;
            _leftDownRect.anchoredPosition = leftDownPos;
            _rightUpRect.anchoredPosition = rightUpPos;
            _rightDownRect.anchoredPosition = rightDownPos;

            _leftUpRect.localEulerAngles = leftUpRot;
            _leftDownRect.localEulerAngles = leftDownRot;
            _rightUpRect.localEulerAngles = rightUpRot;
            _rightDownRect.localEulerAngles = rightDownRot;

            _leftUpRect.localScale = hitLocalScale;
            _leftDownRect.localScale = hitLocalScale;
            _rightUpRect.localScale = hitLocalScale;
            _rightDownRect.localScale = hitLocalScale;

            _leftUpHeadRect.sizeDelta = hitHeadSizeDelta;
            _leftDownHeadRect.sizeDelta = hitHeadSizeDelta;
            _rightUpHeadRect.sizeDelta = hitHeadSizeDelta;
            _rightDownHeadRect.sizeDelta = hitHeadSizeDelta;

            _leftUpHeadRect.anchoredPosition = leftUpPos;
            _leftDownHeadRect.anchoredPosition = leftDownPos;
            _rightUpHeadRect.anchoredPosition = rightUpPos;
            _rightDownHeadRect.anchoredPosition = rightDownPos;

            _leftUpHeadRect.localEulerAngles = leftUpRot;
            _leftDownHeadRect.localEulerAngles = leftDownRot;
            _rightUpHeadRect.localEulerAngles = rightUpRot;
            _rightDownHeadRect.localEulerAngles = rightDownRot;

            _leftUpHeadRect.localScale = hitLocalScale;
            _leftDownHeadRect.localScale = hitLocalScale;
            _rightUpHeadRect.localScale = hitLocalScale;
            _rightDownHeadRect.localScale = hitLocalScale;

            _damageValueRect.anchoredPosition = hitDamageAnchoredPosition;
            _damageValueRect.sizeDelta = hitDamageSizeDelta;
            _damageValueRect.localScale = hitDamageLocalScale;

            _hpTransform.gameObject.SetActive(damageHUDSw);

            hpValue.fontStyle = damageStyles;
            hpValue.color = damageInfoColor;
            hpValue.text = damage.ToString("F0");

            _armorTransform.gameObject.SetActive(hasArmorHit && damageHUDSw);

            armorValue.fontStyle = armorDamageStyles;
            armorValue.color = armorDamageInfoColor;
            armorValue.text = armorDamage.ToString("F2");

            _animatorHitUI.SetFloat(AnimatorHash.ActiveSpeed, activeSpeed);
            _animatorHitUI.SetFloat(AnimatorHash.EndSpeed, endSpeed);
            _animatorHitUI.SetFloat(AnimatorHash.DeadSpeed, deadSpeed);
        }

        internal void HitTrigger(bool isHead, HitModel hitModel,
            HitModel.Direction direction)
        {
            Color hitColor;
            switch (hitModel.HitType)
            {
                case HitModel.Hit.OnlyHp:
                    hitColor = damageColor;
                    break;
                case HitModel.Hit.HasArmorHit:
                    hitColor = armorDamageColor;
                    break;
                case HitModel.Hit.Dead:
                    hitColor = deadColor;
                    break;
                case HitModel.Hit.Head:
                    hitColor = headColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hitModel.HitType), hitModel.HitType, null);
            }

            HitColor(hitColor);

            HitHead(isHead);

            switch (direction)
            {
                case HitModel.Direction.Center:
                    _animatorHitUI.SetTrigger(AnimatorHash.Active);
                    break;
                case HitModel.Direction.Left:
                    _animatorHitUI.SetTrigger(AnimatorHash.ActiveLeft);
                    break;
                case HitModel.Direction.Right:
                    _animatorHitUI.SetTrigger(AnimatorHash.ActiveRight);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private void HitHead(bool sw)
        {
            leftUpHead.gameObject.SetActive(sw);
            leftDownHead.gameObject.SetActive(sw);
            leftUpHead.gameObject.SetActive(sw);
            rightUpHead.gameObject.SetActive(sw);
            rightDownHead.gameObject.SetActive(sw);
        }

        private void HitColor(Color color)
        {
            leftUp.color = color;
            leftDown.color = color;
            rightUp.color = color;
            rightDown.color = color;

            leftUpHead.color = color;
            leftDownHead.color = color;
            rightUpHead.color = color;
            rightDownHead.color = color;
        }

        public void HitDeadTrigger()
        {
            _animatorHitUI.SetTrigger(AnimatorHash.ActiveDead);
        }

#endif
    }
}