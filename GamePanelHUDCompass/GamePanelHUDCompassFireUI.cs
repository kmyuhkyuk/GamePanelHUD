using UnityEngine;
using UnityEngine.UI;
using System;
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
        private GamePanelHUDCorePlugin.HUDClass<GamePanelHUDCompassPlugin.CompassInfo, GamePanelHUDCompassPlugin.SettingsData> HUD
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
        private Image _RealRed;

        [SerializeField]
        private Image _VirtualRed;

        [SerializeField]
        private Image _Virtual2Red;

        private Animator Animator_Fire;

        private RectTransform RealRect;

        private RectTransform VirtualRect;

        private RectTransform Virtual2Rect;

        private RectTransform RealOutlineRect;

        private RectTransform VirtualOutlineRect;

        private RectTransform Virtual2OutlineRect;

        private RectTransform RealRedRect;

        private RectTransform VirtualRedRect;

        private RectTransform Virtual2RedRect;

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

#if !UNITY_EDITOR
        void Start()
        {
            Animator_Fire = GetComponent<Animator>();

            RealOutlineRect = _RealOutline.GetComponent<RectTransform>();
            VirtualOutlineRect = _VirtualOutline.GetComponent<RectTransform>();
            Virtual2OutlineRect = _Virtual2Outline.GetComponent<RectTransform>();

            RealRedRect = _RealRed.GetComponent<RectTransform>();
            VirtualRedRect = _VirtualRed.GetComponent<RectTransform>();
            Virtual2RedRect = _Virtual2Red.GetComponent<RectTransform>();

            RealRect = RealRedRect.parent.GetComponent<RectTransform>();
            VirtualRect = VirtualRedRect.parent.GetComponent<RectTransform>();
            Virtual2Rect = Virtual2RedRect.parent.GetComponent<RectTransform>();

            RealOutlineRect.sizeDelta = OutlineSizeDelta;
            VirtualOutlineRect.sizeDelta = OutlineSizeDelta;
            Virtual2OutlineRect.sizeDelta = OutlineSizeDelta;

            RealRedRect.sizeDelta = FireSizeDelta;
            VirtualRedRect.sizeDelta = FireSizeDelta;
            Virtual2RedRect.sizeDelta = FireSizeDelta;

            _RealOutline.color = OutlineColor;
            _VirtualOutline.color = OutlineColor;
            _Virtual2Outline.color = OutlineColor;

            _RealRed.color = FireColor;
            _VirtualRed.color = FireColor;
            _Virtual2Red.color = FireColor;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
#endif
#if UNITY_EDITOR
        void Update()
#endif
        {
            CompassInfoUI();
        }

        void CompassInfoUI()
        {
#if !UNITY_EDITOR
            Vector3 lhs = Where - HUD.Info.PlayerPosition;

            float angle = GetToAngle(lhs, HUD.Info.NorthDirection, HUD.SettingsData.KeyAngleOffset.Value);

            FireX = -(angle / 15 * 120);

            IsLeft = GetDirection(HUD.SettingsData.KeySizeDelta.Value.x, HUD.Info.CompassX, FireX, FireXLeft, FireXRight, RealDirection(lhs, HUD.Info.PlayerRight));

            RealRect.anchoredPosition = new Vector2(FireX, HUD.SettingsData.KeyCompassFireHeight.Value);
            VirtualRect.anchoredPosition = new Vector2(FireXLeft, HUD.SettingsData.KeyCompassFireHeight.Value);
            Virtual2Rect.anchoredPosition = new Vector2(FireXRight, HUD.SettingsData.KeyCompassFireHeight.Value);

            if (Active)
            {
                Animator_Fire.SetBool(AnimatorHash.Active, Active);

                Active = false;
            }
#endif
        }

#if !UNITY_EDITOR
        public void Fire()
        {
            Animator_Fire.SetTrigger(AnimatorHash.Fire);
        }

        bool? GetDirection(float panelx, float compassx, float firex, float firexleft, float firexright, bool direction)
        {
            float panelHalf = panelx / 2;

            float panelMaxX = panelHalf + compassx;

            float panelMinX = -panelHalf + compassx;

            bool realInPanel = -firex < panelMaxX && -firex > panelMinX;

            bool virtualInPanel = -firexleft < panelMaxX && -firexleft > panelMinX || -firexright < panelMaxX && -firexright > panelMinX;

            if (!realInPanel && !virtualInPanel)
            {
                return direction;
            }
            else
            {
                return default;
            }
        }

        float GetToAngle(Vector3 lhs, float northdirection, float offset)
        {
            float num = Vector3.SignedAngle(lhs, -Vector3.forward, Vector3.up) - northdirection + offset; //Why is -Vector3.forward?

            if (num >= 0)
            {
                return num;
            }
            else
            {
                return num + 360;
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
}
