using System;
using UnityEngine;
using EFT.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
#endif
using GamePanelHUDCore.Utils;

namespace GamePanelHUDHit
{
    public class GamePanelHUDHit : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDCorePlugin.HUDClass<RectTransform, GamePanelHUDHitPlugin.SettingsData> HUD
        {
            get
            {
                return GamePanelHUDHitPlugin.HitHUD;
            }
        }
#endif

        [SerializeField]
        private GamePanelHUDHitUI _Hit;

        private GamePanelHUDHitUI _TestHit;

        private CanvasGroup HitGroup;

#if !UNITY_EDITOR
        void Start()
        {
            if (_Hit != null)
            {
                _TestHit = Instantiate(_Hit, transform);

                HitGroup = _Hit.GetComponent<CanvasGroup>();
            }

            GamePanelHUDHitPlugin.ShowHit = ShowHit;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            HitHUD();
        }

        void HitHUD()
        {
            if (_Hit != null)
            {
                HitGroup.alpha = HUD.HUDSW ? 1 : 0;

                HitSet(_Hit);
                HitSet(_TestHit);
            }
        }

        void HitSet(GamePanelHUDHitUI _hit)
        {
            _hit.DamageHUDSW = HUD.SettingsData.KeyHitDamageHUDSW.Value;

            _hit.HitAnchoredPosition = HUD.SettingsData.KeyHitAnchoredPosition.Value;
            _hit.HitLocalRotation = HUD.SettingsData.KeyHitLocalRotation.Value;
            _hit.HitSizeDelta = HUD.SettingsData.KeyHitSizeDelta.Value;
            _hit.HitLocalScale = HUD.SettingsData.KeyHitLocalScale.Value;
            _hit.HitHeadSizeDelta = HUD.SettingsData.KeyHitHeadSizeDelta.Value;

            _hit.HitDamageAnchoredPosition = HUD.SettingsData.KeyHitDamageAnchoredPosition.Value;
            _hit.HitDamageSizeDelta = HUD.SettingsData.KeyHitDamageSizeDelta.Value;
            _hit.HitDamageLocalScale = HUD.SettingsData.KeyHitDamageLocalScale.Value;

            _hit.ActiveSpeed = HUD.SettingsData.KeyHitActiveSpeed.Value;
            _hit.EndSpeed = HUD.SettingsData.KeyHitEndSpeed.Value;
            _hit.DeadSpeed = HUD.SettingsData.KeyHitDeadSpeed.Value;

            _hit.DamageColor = HUD.SettingsData.KeyHitDamageColor.Value;
            _hit.ArmorDamageColor = HUD.SettingsData.KeyHitArmorDamageColor.Value;
            _hit.DeadColor = HUD.SettingsData.KeyHitDeadColor.Value;
            _hit.HeadColor = HUD.SettingsData.KeyHitHeadColor.Value;
            _hit.DamageInfoColor = HUD.SettingsData.KeyHitDamageInfoColor.Value;
            _hit.ArmorDamageInfoColor = HUD.SettingsData.KeyHitArmorDamageInfoColor.Value;

            _hit.DamageStyles = HUD.SettingsData.KeyHitDamageStyles.Value;
            _hit.ArmorDamageStyles = HUD.SettingsData.KeyHitArmorDamageStyles.Value;
        }

        void ShowHit(GamePanelHUDHitPlugin.HitInfo hitinfo)
        {
            if (_Hit != null)
            {
                GamePanelHUDHitPlugin.HitInfo.Direction direction;

                if (hitinfo.HitDirection.x < HUD.SettingsData.KeyHitDirectionLeft.Value)
                {
                    direction = GamePanelHUDHitPlugin.HitInfo.Direction.Left;
                }
                else if (hitinfo.HitDirection.x > HUD.SettingsData.KeyHitDirectionRight.Value)
                {
                    direction = GamePanelHUDHitPlugin.HitInfo.Direction.Right;
                }
                else
                {
                    direction = GamePanelHUDHitPlugin.HitInfo.Direction.Center;
                }

                if (!hitinfo.IsTest)
                {
                    Camera cam = Camera.main;

                    Vector3 pos = cam.WorldToScreenPoint(hitinfo.HitPoint);

                    Vector2 screenPos = new Vector2(pos.x - (cam.pixelWidth * 0.5f), pos.y - (cam.pixelHeight * 0.5f));

                    _Hit.transform.localPosition = new Vector2((int)Math.Round(screenPos.x), (int)Math.Round(screenPos.y));

                    Hit(hitinfo, direction, HUD.SettingsData, _Hit);
                }
                else
                {
                    _TestHit.transform.localPosition = new Vector2(HUD.Info.sizeDelta.x / 3.2f, 0);

                    Hit(hitinfo, direction, HUD.SettingsData, _TestHit);
                }
            }
        }

        void Hit(GamePanelHUDHitPlugin.HitInfo hitinfo, GamePanelHUDHitPlugin.HitInfo.Direction direction, GamePanelHUDHitPlugin.SettingsData settingsdata, GamePanelHUDHitUI _hit)
        {
            _hit.Damage = hitinfo.Damage;
            _hit.ArmorDamage = hitinfo.ArmorDamage;

            bool isHead = hitinfo.DamagePart == EBodyPart.Head;

            if (settingsdata.KeyHitHasHead.Value && isHead && hitinfo.HitType != GamePanelHUDHitPlugin.HitInfo.Hit.Dead)
            {
                hitinfo.HitType = GamePanelHUDHitPlugin.HitInfo.Hit.Head;
            }

            _hit.HasArmorHit = hitinfo.HasArmorHit;

            _hit.IUpdate();

            if (hitinfo.HitType == GamePanelHUDHitPlugin.HitInfo.Hit.Dead)
            {
                _hit.HitDeadTirgger();
            }

            if (!settingsdata.KeyHitHasDirection.Value)
            {
                direction = GamePanelHUDHitPlugin.HitInfo.Direction.Center;
            }

            _hit.HitTirgger(isHead, hitinfo, direction);
        }
#endif
    }
}
