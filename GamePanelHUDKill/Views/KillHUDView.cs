using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if !UNITY_EDITOR
using EFT;
using EFTUtils;
using static EFTApi.EFTHelpers;
using GamePanelHUDCore.Models;
using GamePanelHUDKill.Models;
using SettingsModel = GamePanelHUDKill.Models.SettingsModel;

#endif

namespace GamePanelHUDKill.Views
{
    public class KillHUDView : MonoBehaviour
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
        private int _currentHasInfo;

        private int _waitInfo;

        private int _lastXp;

        private KillUIView _previous;

        [SerializeField] private Transform killsRoot;

        private Transform _testKillsRoot;

        [SerializeField] private ExpUIView expUIView;

        private ExpUIView _testExpUIView;

        private RectTransform _killsRect;

        private RectTransform _expRect;

        private RectTransform _testKillsRect;

        private RectTransform _testExpRect;

        private CanvasGroup _killGroup;

        private CanvasGroup _expGroup;

        private CanvasGroup _testExpGroup;

        internal static Action HasInfoMinus;

        internal static Action HasWaitInfoMinus;

#if !UNITY_EDITOR
        private void Awake()
        {
            _testKillsRoot = Instantiate(killsRoot.gameObject, killsRoot.transform.parent).transform;
            _testKillsRect = _testKillsRoot.GetComponent<RectTransform>();

            _killGroup = killsRoot.GetComponent<CanvasGroup>();

            _testExpUIView = Instantiate(expUIView.gameObject, expUIView.transform.parent).GetComponent<ExpUIView>();
            _testExpRect = _testExpUIView.GetComponent<RectTransform>();

            _expRect = expUIView.GetComponent<RectTransform>();
            _killsRect = killsRoot.GetComponent<RectTransform>();

            _expGroup = expUIView.GetComponent<CanvasGroup>();
            _testExpGroup = _testExpUIView.GetComponent<CanvasGroup>();

            HasInfoMinus = InfoMinus;
            HasWaitInfoMinus = WaitInfoMinus;

            KillHUDModel.Instance.ShowKill = ShowKill;
        }

        private void Start()
        {
            HUDCoreModel.Instance.UpdateManger.Register(this);
        }

        public void CustomUpdate()
        {
            var killHUDModel = KillHUDModel.Instance;
            var settingsModel = SettingsModel.Instance;

            #region _kill

            _killGroup.alpha = killHUDModel.KillHUDSw ? 1 : 0;

            _killsRect.anchoredPosition = settingsModel.KeyKillAnchoredPosition.Value;
            _killsRect.sizeDelta = settingsModel.KeyKillSizeDelta.Value;
            _killsRect.localScale = settingsModel.KeyKillLocalScale.Value;

            #endregion

            #region _testKills

            _testKillsRect.anchoredPosition = settingsModel.KeyKillAnchoredPosition.Value +
                                              new Vector2(killHUDModel.ScreenRect.sizeDelta.x / 3.2f, 0);
            _testKillsRect.sizeDelta = settingsModel.KeyKillSizeDelta.Value;
            _testKillsRect.localScale = settingsModel.KeyKillLocalScale.Value;

            #endregion

            #region exp

            _expGroup.alpha = killHUDModel.ExpHUDSw ? 1 : 0;

            _expRect.anchoredPosition = settingsModel.KeyKillExpAnchoredPosition.Value;
            _expRect.sizeDelta = settingsModel.KeyKillExpSizeDelta.Value;
            _expRect.localScale = settingsModel.KeyKillExpLocalScale.Value;

            expUIView.xpColor = settingsModel.KeyExpColor.Value;
            expUIView.xpStyles = settingsModel.KeyExpStyles.Value;
            expUIView.xpWaitSpeed = settingsModel.KeyExpWaitSpeed.Value;

            #endregion

            #region _testExp

            _testExpGroup.alpha = settingsModel.KeyExpHUDSw.Value ? 1 : 0;

            _testExpRect.anchoredPosition = settingsModel.KeyKillExpAnchoredPosition.Value +
                                            new Vector2(killHUDModel.ScreenRect.sizeDelta.x / 3.2f, 0);
            _testExpRect.sizeDelta = settingsModel.KeyKillExpSizeDelta.Value;
            _testExpRect.localScale = settingsModel.KeyKillExpLocalScale.Value;

            _testExpUIView.xpColor = settingsModel.KeyExpColor.Value;
            _testExpUIView.xpStyles = settingsModel.KeyExpStyles.Value;
            _testExpUIView.xpWaitSpeed = settingsModel.KeyExpWaitSpeed.Value;

            #endregion
        }

        private void ShowKill(KillModel killModel)
        {
            var settingsModel = SettingsModel.Instance;

            var killRoot = killModel.IsTest ? _testKillsRoot : killsRoot;

            var expRoot = killModel.IsTest ? _testExpUIView : expUIView;

            var baseExp = _SessionHelper.ExperienceHelper.GetBaseExp(killModel.Exp, killModel.Side);

            var allExp = baseExp;

            var allInfo = 1;

            var allKillList = new List<KillUIView>();

            if (killModel.Distance >= settingsModel.KeyKillDistance.Value && settingsModel.KeyKillHasDistance.Value)
            {
                allKillList.Add(AddDistanceInfo(killModel, settingsModel, killRoot));
            }

            if (killModel.KillCount > 1 && settingsModel.KeyKillHasStreak.Value)
            {
                var streakXp =
                    _SessionHelper.ExperienceHelper.GetStreakExp(killModel.Exp, killModel.Side, killModel.KillCount);

                allKillList.Add(AddStreakInfo(killModel, settingsModel, killRoot, streakXp));

                allExp += streakXp;

                allInfo++;
            }

            if (killModel.Part == EBodyPart.Head && settingsModel.KeyKillHasOther.Value)
            {
                var headXp = _SessionHelper.ExperienceHelper.GetHeadExp(killModel.Exp, killModel.Side);

                allKillList.Add(AddOtherInfo(settingsModel, killRoot, _LocalizedHelper.Localized("StatsHeadshot"),
                    headXp, true));

                allExp += headXp;

                allInfo++;
            }

            allKillList.Add(AddKillInfo(killModel, settingsModel, killRoot, baseExp));

            var hasKillsCount = allKillList.Count;

            _currentHasInfo += hasKillsCount;
            _waitInfo += hasKillsCount;

            var count = hasKillsCount - 1;

            if (!settingsModel.KeyKillWaitBottom.Value)
            {
                if (_previous != null)
                {
                    _previous.after = allKillList.Last();

                    count -= 1;
                }

                allKillList.First().canDestroy = true;
            }
            else
            {
                if (_previous != null)
                {
                    _previous.after = allKillList.First();

                    allKillList.Last().after = _previous;
                }
                else
                {
                    allKillList.First().canDestroy = true;
                }
            }

            _previous = allKillList.Last();

            if (hasKillsCount > 1)
            {
                for (var i = 0; i < count; i++)
                {
                    allKillList[i].after = allKillList[i + 1];
                }
            }

            var isAnim = expRoot.IsAnim;

            if (_currentHasInfo > 0 && !isAnim)
            {
                expRoot.XpUp(allExp, _lastXp);
            }
            else if (isAnim || allInfo > 1)
            {
                expRoot.XpUp(allExp, 0);
            }
            else
            {
                _lastXp = allExp;
            }
        }

        private static KillUIView AddKillInfo(KillModel killModel,
            SettingsModel settingsModel, Transform killsRoot, int exp)
        {
            var killUI = Instantiate(KillHUDModel.Instance.KillPrefab, killsRoot).GetComponent<KillUIView>();

            var isScav = killModel.Side == EPlayerSide.Savage;

            string playerName;
            if (isScav && settingsModel.KeyKillScavEn.Value)
            {
                playerName = _LocalizedHelper.Transliterate(killModel.PlayerName);
            }
            else
            {
                playerName = killModel.PlayerName;
            }

            var weaponName = _LocalizedHelper.Localized(killModel.WeaponName);

            string sideKey;
            if (isScav && _PlayerHelper.RoleHelper.IsBossOrFollower(killModel.Role))
            {
                sideKey = _PlayerHelper.RoleHelper.GetScavRoleKey(killModel.Role);
            }
            else
            {
                sideKey = killModel.Side.ToString();
            }

            var sideName = _LocalizedHelper.Localized(sideKey);

            var nameColor = settingsModel.KeyKillNameColor.Value.ColorToHtml();
            var partColor = settingsModel.KeyKillPartColor.Value.ColorToHtml();
            var weaponColor = settingsModel.KeyKillWeaponColor.Value.ColorToHtml();
            var lvlColor = settingsModel.KeyKillLvlColor.Value.ColorToHtml();
            var levelColor = settingsModel.KeyKillLevelColor.Value.ColorToHtml();
            var bracketColor = settingsModel.KeyKillBracketColor.Value.ColorToHtml();
            var enemyDownColor = settingsModel.KeyKillEnemyDownColor.Value.ColorToHtml();

            string sideColor;
            switch (isScav)
            {
                case true when _PlayerHelper.RoleHelper.IsBoss(killModel.Role):
                    sideColor = settingsModel.KeyKillBossColor.Value.ColorToHtml();
                    break;
                case true when _PlayerHelper.RoleHelper.IsFollower(killModel.Role):
                    sideColor = settingsModel.KeyKillFollowerColor.Value.ColorToHtml();
                    break;
                default:
                    switch (killModel.Side)
                    {
                        case EPlayerSide.Usec:
                            sideColor = settingsModel.KeyKillUsecColor.Value.ColorToHtml();
                            break;
                        case EPlayerSide.Bear:
                            sideColor = settingsModel.KeyKillBearColor.Value.ColorToHtml();
                            break;
                        case EPlayerSide.Savage:
                            sideColor = settingsModel.KeyKillScavColor.Value.ColorToHtml();
                            break;
                        default:
                            sideColor = Color.black.ColorToHtml();
                            break;
                    }

                    break;
            }

            killUI.xp = exp;

            var part = settingsModel.KeyKillHasPart.Value
                ? $"<color={bracketColor}>[</color><color={partColor}>{_LocalizedHelper.Localized(killModel.Part.ToString())}</color><color={bracketColor}>]</color>"
                : string.Empty;
            var side = settingsModel.KeyKillHasSide.Value
                ? $"<color={bracketColor}>[</color><color={sideColor}>{sideName}</color><color={bracketColor}>]</color>"
                : string.Empty;
            var weapon = settingsModel.KeyKillHasWeapon.Value
                ? $"<color={bracketColor}>[</color><color={weaponColor}>{weaponName}</color><color={bracketColor}>]</color>"
                : string.Empty;
            var level = !isScav && settingsModel.KeyKillHasLevel.Value
                ? $"<color={bracketColor}>[</color><color={lvlColor}>{_LocalizedHelper.Localized("LVLKILLLIST")}.</color><color={levelColor}>{killModel.Level}</color><color={bracketColor}>]</color>"
                : string.Empty;

            killUI.text = $"<color={enemyDownColor}>{_LocalizedHelper.Localized("ENEMYDOWN")}</color>";
            killUI.text2 = $"{weapon}{side} <color={nameColor}>{playerName}</color> {level}{part}";

            killUI.textFontStyles = settingsModel.KeyKillInfoStyles.Value;

            killUI.isKillInfo = true;

            killUI.hasXp = settingsModel.KeyKillHasXp.Value;

            killUI.xpColor = settingsModel.KeyKillXpColor.Value;

            killUI.transform.SetAsFirstSibling();

            killUI.active = true;

            return killUI;
        }

        private static KillUIView AddDistanceInfo(KillModel killModel,
            SettingsModel settingsModel, Transform killsRoot)
        {
            var killUI = Instantiate(KillHUDModel.Instance.KillPrefab, killsRoot).GetComponent<KillUIView>();

            var meters = _LocalizedHelper.Localized("meters");

            var metersColor = settingsModel.KeyKillMetersColor.Value.ColorToHtml();

            var distanceColor = settingsModel.KeyKillDistanceColor.Value.ColorToHtml();

            var distanceText = killModel.Distance.ToString("F2");

            killUI.text = $"<color={distanceColor}>{distanceText}</color> <color={metersColor}>{meters}</color>";
            killUI.textFontStyles = settingsModel.KeyKillDistanceStyles.Value;

            killUI.transform.SetAsFirstSibling();

            killUI.active = true;

            return killUI;
        }

        private static KillUIView AddStreakInfo(KillModel killModel,
            SettingsModel settingsModel, Transform killsRoot, int exp)
        {
            var killUI = Instantiate(KillHUDModel.Instance.KillPrefab, killsRoot).GetComponent<KillUIView>();

            var statsStreakColor = settingsModel.KeyKillStatsStreakColor.Value.ColorToHtml();
            var streakColor = settingsModel.KeyKillStreakColor.Value.ColorToHtml();
            var bracketColor = settingsModel.KeyKillBracketColor.Value.ColorToHtml();
            killUI.text =
                $"<color={statsStreakColor}>{_LocalizedHelper.Localized("StatsStreak")}</color> <color={bracketColor}>[</color><color={streakColor}>{killModel.KillCount}</color><color={bracketColor}>]</color>";
            killUI.textFontStyles = settingsModel.KeyKillStreakStyles.Value;

            killUI.xp = exp;

            killUI.hasXp = settingsModel.KeyKillHasXp.Value;

            killUI.xpColor = settingsModel.KeyKillXpColor.Value;

            killUI.transform.SetAsFirstSibling();

            killUI.active = true;

            return killUI;
        }

        private static KillUIView AddOtherInfo(SettingsModel settingsModel, Transform killsRoot,
            string text, int exp, bool hasXp)
        {
            var killUI = Instantiate(KillHUDModel.Instance.KillPrefab, killsRoot).GetComponent<KillUIView>();

            var otherColor = settingsModel.KeyKillOtherColor.Value.ColorToHtml();

            killUI.text = $"<color={otherColor}>{text}</color>";
            killUI.textFontStyles = settingsModel.KeyKillOtherStyles.Value;

            killUI.xp = exp;

            killUI.hasXp = hasXp && settingsModel.KeyKillHasXp.Value;

            killUI.xpColor = settingsModel.KeyKillXpColor.Value;

            killUI.transform.SetAsFirstSibling();

            killUI.active = true;

            return killUI;
        }

        private void InfoMinus()
        {
            _currentHasInfo -= 1;
        }

        private void WaitInfoMinus()
        {
            _waitInfo -= 1;

            if (_waitInfo == 0)
            {
                expUIView.XpComplete();
                _testExpUIView.XpComplete();
            }
        }

#endif
    }
}