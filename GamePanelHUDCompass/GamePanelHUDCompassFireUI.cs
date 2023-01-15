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
            float angle = GetToAngle(HUD.Info.PlayerPosition, Where, HUD.Info.NorthDirection, HUD.SettingsData.KeyAngleOffset.Value);

            float compassX = -(angle / 15 * 120);

            float height = HUD.SettingsData.KeyCompassFireHeight.Value;

            RealRect.anchoredPosition = new Vector2(compassX, height);
            VirtualRect.anchoredPosition = new Vector2(compassX - 2880, height);
            Virtual2Rect.anchoredPosition = new Vector2(compassX + 2880, height);

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
