/*using UnityEngine;
using UnityEngine.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCompass
{
    public class GamePanelHUDCompassFireUI : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassFireData, GamePanelHUDCompassPlugin.SettingsData> HUD
        {
            get
            {
                return GamePanelHUDCompassPlugin.CompassFireHUD;
            }
        }
#endif

        public bool Active;

        public bool IsBoss;

        public bool IsFollower;

        public bool? IsLeft { get; private set; }

        public bool DeadDestroy;

        public int Who;

        public Vector3 Where;

        public Color FireColor;

        public Color OutlineColor;

        public Vector2 FireSizeDelta;

        public Vector2 OutlineSizeDelta;

        [SerializeField]
        private Image _RealOutline;

        [SerializeField]
        private Image _VirtualOutline;

        [SerializeField]
        private Image _Virtual2Outline;

        [SerializeField]
        private Image _Virtual3Outline;

        [SerializeField]
        private Image _RealRed;

        [SerializeField]
        private Image _VirtualRed;

        [SerializeField]
        private Image _Virtual2Red;

        [SerializeField]
        private Image _Virtual3Red;

        private Animator Animator_Fire;

        private RectTransform RealRect;

        private RectTransform VirtualRect;

        private RectTransform Virtual2Rect;

        private RectTransform Virtual3Rect;

        private RectTransform RealOutlineRect;

        private RectTransform VirtualOutlineRect;

        private RectTransform Virtual2OutlineRect;

        private RectTransform Virtual3OutlineRect;

        private RectTransform RealRedRect;

        private RectTransform VirtualRedRect;

        private RectTransform Virtual2RedRect;

        private RectTransform Virtual3RedRect;

        private float FireX;

        private float FireXLeft
        {
            get
            {
                return FireX - 2880;
            }
        }

        private float FireXRight
        {
            get
            {
                return FireX + 2880;
            }
        }

        private float FireXRightRight
        {
            get
            {
                return FireX + 5760; //2880 * 2
            }
        }

#if !UNITY_EDITOR
        void Start()
        {
            Animator_Fire = GetComponent<Animator>();

            RealOutlineRect = _RealOutline.GetComponent<RectTransform>();
            VirtualOutlineRect = _VirtualOutline.GetComponent<RectTransform>();
            Virtual2OutlineRect = _Virtual2Outline.GetComponent<RectTransform>();
            Virtual3OutlineRect = _Virtual3Outline.GetComponent<RectTransform>();

            RealRedRect = _RealRed.GetComponent<RectTransform>();
            VirtualRedRect = _VirtualRed.GetComponent<RectTransform>();
            Virtual2RedRect = _Virtual2Red.GetComponent<RectTransform>();
            Virtual3RedRect = _Virtual3Red.GetComponent<RectTransform>();

            RealRect = RealRedRect.parent.GetComponent<RectTransform>();
            VirtualRect = VirtualRedRect.parent.GetComponent<RectTransform>();
            Virtual2Rect = Virtual2RedRect.parent.GetComponent<RectTransform>();
            Virtual3Rect = Virtual3RedRect.parent.GetComponent<RectTransform>();

            RealOutlineRect.sizeDelta = OutlineSizeDelta;
            VirtualOutlineRect.sizeDelta = OutlineSizeDelta;
            Virtual2OutlineRect.sizeDelta = OutlineSizeDelta;
            Virtual3OutlineRect.sizeDelta = OutlineSizeDelta;

            RealRedRect.sizeDelta = FireSizeDelta;
            VirtualRedRect.sizeDelta = FireSizeDelta;
            Virtual2RedRect.sizeDelta = FireSizeDelta;
            Virtual3RedRect.sizeDelta = FireSizeDelta;

            _RealOutline.color = OutlineColor;
            _VirtualOutline.color = OutlineColor;
            _Virtual2Outline.color = OutlineColor;
            _Virtual3Outline.color = OutlineColor;

            _RealRed.color = FireColor;
            _VirtualRed.color = FireColor;
            _Virtual2Red.color = FireColor;
            _Virtual3Red.color = FireColor;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
#endif
#if UNITY_EDITOR
        void Update()
#endif
        {
            CompassFireUI();
        }

        void CompassFireUI()
        {
#if !UNITY_EDITOR
            Vector3 lhs = Where - HUD.Info.PlayerPosition;

            float angle = HUD.Info.GetToAngle(lhs, HUD.SettingsData.KeyAngleOffset.Value);

            FireX = -(angle / 15 * 120);

            float fireXLeft = FireXLeft;
            float fireXRight = FireXRight;
            float fireXRightRight = FireXRightRight;

            IsLeft = GetDirection(HUD.Info.SizeDelta.x, HUD.Info.CompassX, FireX, fireXLeft, fireXRight, fireXRightRight, lhs, HUD.Info.PlayerRight);

            RealRect.anchoredPosition = new Vector2(FireX, HUD.SettingsData.KeyCompassFireHeight.Value);
            VirtualRect.anchoredPosition = new Vector2(fireXLeft, HUD.SettingsData.KeyCompassFireHeight.Value);
            Virtual2Rect.anchoredPosition = new Vector2(fireXRight, HUD.SettingsData.KeyCompassFireHeight.Value);
            Virtual3Rect.anchoredPosition = new Vector2(fireXRightRight, HUD.SettingsData.KeyCompassFireHeight.Value);

            Animator_Fire.SetFloat(AnimatorHash.Active, HUD.SettingsData.KeyCompassFireActiveSpeed.Value);
            Animator_Fire.SetFloat(AnimatorHash.Speed, HUD.SettingsData.KeyCompassFireWaitSpeed.Value);
            Animator_Fire.SetFloat(AnimatorHash.ToSmallSpeed, HUD.SettingsData.KeyCompassFireToSmallSpeed.Value);
            Animator_Fire.SetFloat(AnimatorHash.SmallSpeed, HUD.SettingsData.KeyCompassFireSmallWaitSpeed.Value);

            if (Active)
            {
                Animator_Fire.SetBool(AnimatorHash.Active, Active);

                Active = false;
            }

            if (DeadDestroy)
            {
                Destroy();

                DeadDestroy = false;
            }
#endif
        }

#if !UNITY_EDITOR
        public void Fire()
        {
            Animator_Fire.SetTrigger(AnimatorHash.Fire);
        }

        public bool IsDestroy()
        {
            return Animator_Fire.GetCurrentAnimatorStateInfo(0).shortNameHash == AnimatorHash.Destroy;
        }

        bool? GetDirection(float panelx, float compassx, float firex, float firexleft, float firexright, float firexrightright, Vector3 lhs, Vector3 right)
        {
            float panelHalf = panelx / 2;

            float panelMaxX = panelHalf + compassx;

            float panelMinX = -panelHalf + compassx;

            bool realInPanel = -firex < panelMaxX && -firex > panelMinX;

            bool virtualInPanel = -firexleft < panelMaxX && -firexleft > panelMinX || -firexright < panelMaxX && -firexright > panelMinX || -firexrightright < panelMaxX && -firexrightright > panelMinX;

            if (!realInPanel && !virtualInPanel)
            {
                return RealDirection(lhs, right);
            }
            else
            {
                return default;
            }
        }

        bool RealDirection(Vector3 lhs, Vector3 right)
        {
            return Vector3.Dot(lhs, right) < 0 ? true : false;
        }
#endif

        void Destroy()
        {
#if !UNITY_EDITOR
            GamePanelHUDCompassFire.Remove(Who);
            GamePanelHUDCorePlugin.UpdateManger.Remove(this);
            Destroy(gameObject);
#endif
        }
    }
}*/
