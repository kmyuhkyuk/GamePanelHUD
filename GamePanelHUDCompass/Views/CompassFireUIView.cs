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

        [SerializeField] private Image virtualLeftOutline;

        [SerializeField] private Image virtualRightOutline;

        [SerializeField] private Image realRed;

        [SerializeField] private Image virtualLeftRed;

        [SerializeField] private Image virtualRightRed;

        private Animator _animatorFire;

        private RectTransform _realRect;

        private RectTransform _virtualLeftRect;

        private RectTransform _virtualRightRect;

        private RectTransform _realOutlineRect;

        private RectTransform _virtualLeftOutlineRect;

        private RectTransform _virtualRightOutlineRect;

        private RectTransform _realRedRect;

        private RectTransform _virtualLeftRedRect;

        private RectTransform _virtualRightRedRect;

        private float _angle;

        private float FireX => -_angle * 8f + 8 * 360;

        private float FireXLeft => FireX - 8 * 360;

        private float FireXRight => FireX + 8 * 360;

#if !UNITY_EDITOR
        private void Awake()
        {
            _animatorFire = GetComponent<Animator>();

            _realOutlineRect = realOutline.GetComponent<RectTransform>();
            _virtualLeftOutlineRect = virtualLeftOutline.GetComponent<RectTransform>();
            _virtualRightOutlineRect = virtualRightOutline.GetComponent<RectTransform>();

            _realRedRect = realRed.GetComponent<RectTransform>();
            _virtualLeftRedRect = virtualLeftRed.GetComponent<RectTransform>();
            _virtualRightRedRect = virtualRightRed.GetComponent<RectTransform>();

            _realRect = _realRedRect.parent.GetComponent<RectTransform>();
            _virtualLeftRect = _virtualLeftRedRect.parent.GetComponent<RectTransform>();
            _virtualRightRect = _virtualRightRedRect.parent.GetComponent<RectTransform>();
        }

        private void Start()
        {
            _realOutlineRect.sizeDelta = outlineSizeDelta;
            _virtualLeftOutlineRect.sizeDelta = outlineSizeDelta;
            _virtualRightOutlineRect.sizeDelta = outlineSizeDelta;

            _realRedRect.sizeDelta = fireSizeDelta;
            _virtualLeftRedRect.sizeDelta = fireSizeDelta;
            _virtualRightRedRect.sizeDelta = fireSizeDelta;

            realOutline.color = outlineColor;
            virtualLeftOutline.color = outlineColor;
            virtualRightOutline.color = outlineColor;

            realRed.color = fireColor;
            virtualLeftRed.color = fireColor;
            virtualRightRed.color = fireColor;

            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var compassHUDModel = CompassHUDModel.Instance;
            var compassFireHUDModel = CompassFireHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            var lhs = where - compassFireHUDModel.CompassFire.CameraPosition;

            _angle = compassFireHUDModel.CompassFire.GetToAngle(lhs);

            var fireX = FireX;
            var fireXLeft = FireXLeft;
            var fireXRight = FireXRight;

            Direction = GetDirection(compassHUDModel.Compass.SizeDelta.x, compassHUDModel.Compass.CompassX, fireX,
                fireXLeft, fireXRight, lhs, compassFireHUDModel.CompassFire.PlayerRight);

            var height = settingsModel.KeyCompassFireHeight.Value;
            _realRect.anchoredPosition = new Vector2(fireX + compassHUDModel.Compass.CompassX, height);
            _virtualLeftRect.anchoredPosition = new Vector2(fireXLeft + compassHUDModel.Compass.CompassX, height);
            _virtualRightRect.anchoredPosition = new Vector2(fireXRight + compassHUDModel.Compass.CompassX, height);

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
            Vector3 lhs, Vector3 right)
        {
            var panelHalf = panelX / 2;

            var panelMaxX = panelHalf + compassX;

            var panelMinX = -panelHalf + compassX;

            var realInPanel = -fireX < panelMaxX && -fireX > panelMinX;

            var virtualInPanel = -fireXLeft < panelMaxX && -fireXLeft > panelMinX ||
                                 -fireXRight < panelMaxX && -fireXRight > panelMinX;

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