using UnityEngine;
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

            RealRect.anchoredPosition = new Vector2(-(angle / 15 * 120), 8);
            VirtualRect.anchoredPosition = new Vector2(-(angle / 15 * 120) - 2880, 8);
            Virtual2Rect.anchoredPosition = new Vector2(-(angle / 15 * 120) + 2880, 8);
#endif
        }

#if !UNITY_EDITOR
        public void Fire()
        {
            Animator_Fire.SetTrigger(AnimatorHash.Fire);
        }

        float GetToAngle(Vector3 position, Vector3 position2, float northdirection, float offset)
        {
            return Vector3.Angle(position, position2) - northdirection + offset;
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
