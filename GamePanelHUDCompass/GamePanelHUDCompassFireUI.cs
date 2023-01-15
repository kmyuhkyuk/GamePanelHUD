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

        private float Angle;

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
            Angle = GetToAngle(HUD.Info.PlayerPosition, Where, HUD.Info.NorthDirection, HUD.SettingsData.KeyAngleOffset.Value);

            FireX = -(Angle / 15 * 120);

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

        public GamePanelHUDCompassPlugin.CompassFireInfo.HideDirection GetDirection()
        {
            float panelX = HUD.SettingsData.KeySizeDelta.Value.x;

            float panelMaxX = panelX / 2;

            float panelMinX = -panelMaxX;

            bool realInPanel = FireX < panelMaxX && FireX > panelMinX;

            bool virtualInPanel = FireXLeft < panelMaxX && FireXLeft > panelMinX || FireXRight < panelMaxX && FireXRight > panelMinX;

            if (!realInPanel && !virtualInPanel)
            {
                float right = Math.Abs(HUD.Info.Angle) - Angle;

                float left = 360 - right;

                return right > left ? GamePanelHUDCompassPlugin.CompassFireInfo.HideDirection.Left : GamePanelHUDCompassPlugin.CompassFireInfo.HideDirection.Right;
            }
            else
            {
                return GamePanelHUDCompassPlugin.CompassFireInfo.HideDirection.None;
            }
        }

        float GetToAngle(Vector3 position, Vector3 position2, float northdirection, float offset)
        {
            float num = Vector3.SignedAngle(position2 - position, -Vector3.forward, Vector3.up) - northdirection + offset; //Why is -Vector3.forward?

            if (num >= 0)
            {
                return num;
            }
            else
            {
                return num + 360;
            }
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
