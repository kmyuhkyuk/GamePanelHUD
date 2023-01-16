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

        public int Who;

        public Vector3 Where;

        public GamePanelHUDCompassPlugin.CompassFireInfo.HideDirection Direction { get; private set; }

        [SerializeField]
        private Image _Real;

        [SerializeField]
        private Image _Virtual;

        [SerializeField]
        private Image _Virtual2;

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

            RealRect = _Real.GetComponent<RectTransform>();
            VirtualRect = _Virtual.GetComponent<RectTransform>();
            Virtual2Rect = _Virtual2.GetComponent<RectTransform>();

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

            Direction = GetDirection(HUD.SettingsData.KeySizeDelta.Value.x, HUD.Info.CompassX, FireX, FireXLeft, FireXRight, RealDirection(lhs, HUD.Info.PlayerRight));

            float height = HUD.SettingsData.KeyCompassFireHeight.Value;

            RealRect.anchoredPosition = new Vector2(FireX, height);
            VirtualRect.anchoredPosition = new Vector2(FireXLeft, height);
            Virtual2Rect.anchoredPosition = new Vector2(FireXRight, height);

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

        GamePanelHUDCompassPlugin.CompassFireInfo.HideDirection GetDirection(float panelx, float compassx, float firex, float firexleft, float firexright, GamePanelHUDCompassPlugin.CompassFireInfo.HideDirection direction)
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
                return GamePanelHUDCompassPlugin.CompassFireInfo.HideDirection.None;
            }
        }

        float GetToAngle(Vector3 lhs, float northdirection, float offset)
        {
            float num = Vector3.SignedAngle(lhs, -Vector3.forward, Vector3.up) - northdirection + offset;

            if (num >= 0)
            {
                return num;
            }
            else
            {
                return num + 360;
            }
        }

        GamePanelHUDCompassPlugin.CompassFireInfo.HideDirection RealDirection(Vector3 lhs, Vector3 right)
        {
            return Vector3.Dot(lhs, right) < 0 ? GamePanelHUDCompassPlugin.CompassFireInfo.HideDirection.Left : GamePanelHUDCompassPlugin.CompassFireInfo.HideDirection.Right;
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
