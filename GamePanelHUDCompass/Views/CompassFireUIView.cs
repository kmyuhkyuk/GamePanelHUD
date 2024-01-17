using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR
using EFTUtils;
using GamePanelHUDCore.Models;
using GamePanelHUDCore.Utils;
using GamePanelHUDCompass.Models;
using SettingsModel = GamePanelHUDCompass.Models.SettingsModel;

#endif

namespace GamePanelHUDCompass.Views
{
    public class CompassFireUIView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        public bool active;

        public bool isBoss;

        public bool isFollower;

        public int Direction { get; private set; }

        public string who;

        public Vector3 where;

        public Color fireColor;

        public Color outlineColor;

        public Vector2 fireSizeDelta;

        public Vector2 outlineSizeDelta;

        [SerializeField] private Image realOutline;

        [SerializeField] private Image virtualOutline;

        [SerializeField] private Image virtual2Outline;

        [SerializeField] private Image virtual3Outline;

        [SerializeField] private Image realRed;

        [SerializeField] private Image virtualRed;

        [SerializeField] private Image virtual2Red;

        [SerializeField] private Image virtual3Red;

        private Animator _animatorFire;

        private RectTransform _realRect;

        private RectTransform _virtualRect;

        private RectTransform _virtual2Rect;

        private RectTransform _virtual3Rect;

        private RectTransform _realOutlineRect;

        private RectTransform _virtualOutlineRect;

        private RectTransform _virtual2OutlineRect;

        private RectTransform _virtual3OutlineRect;

        private RectTransform _realRedRect;

        private RectTransform _virtualRedRect;

        private RectTransform _virtual2RedRect;

        private RectTransform _virtual3RedRect;

        private float _fireX;

        private float FireXLeft => _fireX - 2880;

        private float FireXRight => _fireX + 2880;

        private float FireXRightRight => _fireX + 5760; //2880 * 2

#if !UNITY_EDITOR
        private void Awake()
        {
            _animatorFire = GetComponent<Animator>();

            _realOutlineRect = realOutline.GetComponent<RectTransform>();
            _virtualOutlineRect = virtualOutline.GetComponent<RectTransform>();
            _virtual2OutlineRect = virtual2Outline.GetComponent<RectTransform>();
            _virtual3OutlineRect = virtual3Outline.GetComponent<RectTransform>();

            _realRedRect = realRed.GetComponent<RectTransform>();
            _virtualRedRect = virtualRed.GetComponent<RectTransform>();
            _virtual2RedRect = virtual2Red.GetComponent<RectTransform>();
            _virtual3RedRect = virtual3Red.GetComponent<RectTransform>();

            _realRect = _realRedRect.parent.GetComponent<RectTransform>();
            _virtualRect = _virtualRedRect.parent.GetComponent<RectTransform>();
            _virtual2Rect = _virtual2RedRect.parent.GetComponent<RectTransform>();
            _virtual3Rect = _virtual3RedRect.parent.GetComponent<RectTransform>();

            _realOutlineRect.sizeDelta = outlineSizeDelta;
            _virtualOutlineRect.sizeDelta = outlineSizeDelta;
            _virtual2OutlineRect.sizeDelta = outlineSizeDelta;
            _virtual3OutlineRect.sizeDelta = outlineSizeDelta;

            _realRedRect.sizeDelta = fireSizeDelta;
            _virtualRedRect.sizeDelta = fireSizeDelta;
            _virtual2RedRect.sizeDelta = fireSizeDelta;
            _virtual3RedRect.sizeDelta = fireSizeDelta;

            realOutline.color = outlineColor;
            virtualOutline.color = outlineColor;
            virtual2Outline.color = outlineColor;
            virtual3Outline.color = outlineColor;

            realRed.color = fireColor;
            virtualRed.color = fireColor;
            virtual2Red.color = fireColor;
            virtual3Red.color = fireColor;
        }

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var compassHUDModel = CompassHUDModel.Instance;
            var compassFireHUDModel = CompassFireHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            var lhs = where - compassFireHUDModel.CompassFire.CameraPosition;

            var angle = compassFireHUDModel.CompassFire.GetToAngle(lhs);

            _fireX = -(angle / 15 * 120);

            var fireXLeft = FireXLeft;
            var fireXRight = FireXRight;
            var fireXRightRight = FireXRightRight;

            Direction = GetDirection(compassHUDModel.Compass.SizeDelta.x, compassHUDModel.Compass.CompassX, _fireX,
                fireXLeft, fireXRight,
                fireXRightRight, lhs, compassFireHUDModel.CompassFire.PlayerRight);

            var height = settingsModel.KeyCompassFireHeight.Value;
            _realRect.anchoredPosition = new Vector2(_fireX, height);
            _virtualRect.anchoredPosition = new Vector2(fireXLeft, height);
            _virtual2Rect.anchoredPosition = new Vector2(fireXRight, height);
            _virtual3Rect.anchoredPosition = new Vector2(fireXRightRight, height);

            _animatorFire.SetFloat(AnimatorHash.Active, settingsModel.KeyCompassFireActiveSpeed.Value);
            _animatorFire.SetFloat(AnimatorHash.Speed, settingsModel.KeyCompassFireWaitSpeed.Value);
            _animatorFire.SetFloat(AnimatorHash.ToSmallSpeed, settingsModel.KeyCompassFireToSmallSpeed.Value);
            _animatorFire.SetFloat(AnimatorHash.SmallSpeed, settingsModel.KeyCompassFireSmallWaitSpeed.Value);

            if (!active)
                return;

            _animatorFire.SetBool(AnimatorHash.Active, true);

            active = false;
        }

        public void Fire()
        {
            _animatorFire.SetTrigger(AnimatorHash.Fire);
        }

        private static int GetDirection(float panelX, float compassX, float fireX, float fireXLeft, float fireXRight,
            float fireXRightRight, Vector3 lhs, Vector3 right)
        {
            var panelHalf = panelX / 2;

            var panelMaxX = panelHalf + compassX;

            var panelMinX = -panelHalf + compassX;

            var realInPanel = -fireX < panelMaxX && -fireX > panelMinX;

            var virtualInPanel = -fireXLeft < panelMaxX && -fireXLeft > panelMinX ||
                                 -fireXRight < panelMaxX && -fireXRight > panelMinX ||
                                 -fireXRightRight < panelMaxX && -fireXRightRight > panelMinX;

            if (!realInPanel && !virtualInPanel)
                return RealDirection(lhs, right) ? 1 : -1;

            return 0;
        }

        private static bool RealDirection(Vector3 lhs, Vector3 right)
        {
            return Vector3.Dot(lhs, right) < 0;
        }

#endif
        // ReSharper disable once UnusedMember.Local
        private void ToDestroy()
        {
            CompassFireHUDView.Remove(who);
        }

#if !UNITY_EDITOR

        public void Destroy()
        {
            HUDCoreModel.Instance.UpdateManger.Remove(this);
            Destroy(gameObject);
        }
#endif
    }
}