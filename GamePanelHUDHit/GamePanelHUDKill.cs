using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using EFT;
using EFT.UI;
#if !UNITY_EDITOR
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;
#endif

namespace GamePanelHUDHit
{
    public class GamePanelHUDKill : UIElement
#if !UNITY_EDITOR
        , IUpdate
#endif
    {
#if !UNITY_EDITOR
        private GamePanelHUDHitPlugin.KillHUDClass<RectTransform, GamePanelHUDHitPlugin.SettingsData> HUD
        {
            get
            {
                return GamePanelHUDHitPlugin.KillHUD;
            }
        }
#endif

        private int NowHasInfo;

        private int WaitInfo;

        private int LastXp;

        private GamePanelHUDKillUI Previous;

        [SerializeField]
        private Transform _Kills;

        private Transform _TestKills;

        [SerializeField]
        private GamePanelHUDExpUI _Exp;

        private GamePanelHUDExpUI _TestExp;

        private RectTransform KillsRect;

        private RectTransform ExpRect;

        private RectTransform TestKillsRect;

        private RectTransform TestExpRect;

        private CanvasGroup KillGroup;

        private CanvasGroup ExpGroup;

        private CanvasGroup TestExpGroup;

        internal static Action HasInfoAdd;

        internal static Action HasInfoMinu;

        internal static Action HasWaitInfoMinu;

#if !UNITY_EDITOR
        void Start()
        {
            if (_Kills != null)
            {
                _TestKills = Instantiate(_Kills.gameObject, _Kills.transform.parent).transform;
                TestKillsRect = _TestKills.GetComponent<RectTransform>();

                KillGroup = _Kills.GetComponent<CanvasGroup>();
            }
            if (_Exp != null)
            {
                _TestExp = Instantiate(_Exp.gameObject, _Exp.transform.parent).GetComponent<GamePanelHUDExpUI>();
                TestExpRect = _TestExp.GetComponent<RectTransform>();

                ExpRect = _Exp.GetComponent<RectTransform>();
                KillsRect = _Kills.GetComponent<RectTransform>();

                ExpGroup = _Exp.GetComponent<CanvasGroup>();
                TestExpGroup = _TestExp.GetComponent<CanvasGroup>();
            }

            HasInfoAdd = InfoAdd;
            HasInfoMinu = InfoMinu;
            HasWaitInfoMinu = WaitInfoMinu;

            GamePanelHUDHitPlugin.ShowKill = ShowKill;

            GamePanelHUDCorePlugin.UpdateManger.Register(this);
        }

        public void IUpdate()
        {
            KillHUD();
        }

        void KillHUD()
        {
            if (_Kills != null)
            {
                KillGroup.alpha = HUD.HUDSW ? 1 : 0;

                KillsRect.anchoredPosition = HUD.SettingsData.KeyKillAnchoredPosition.Value;
                KillsRect.sizeDelta = HUD.SettingsData.KeyKillSizeDelta.Value;
                KillsRect.localScale = HUD.SettingsData.KeyKillLocalScale.Value;

                TestKillsRect.anchoredPosition = HUD.SettingsData.KeyKillAnchoredPosition.Value + new Vector2(HUD.Info.sizeDelta.x / 3.2f, 0);
                TestKillsRect.sizeDelta = HUD.SettingsData.KeyKillSizeDelta.Value;
                TestKillsRect.localScale = HUD.SettingsData.KeyKillLocalScale.Value;
            }
            if (_Exp != null)
            {
                ExpGroup.alpha = HUD.HUDSW2 ? 1 : 0;

                ExpRect.anchoredPosition = HUD.SettingsData.KeyKillExpAnchoredPosition.Value;
                ExpRect.sizeDelta = HUD.SettingsData.KeyKillExpSizeDelta.Value;
                ExpRect.localScale = HUD.SettingsData.KeyKillExpLocalScale.Value;

                _Exp.XPColor = HUD.SettingsData.KeyExpColor.Value.ColorToHtml();
                _Exp.XPStyles = HUD.SettingsData.KeyExpStyles.Value;
                _Exp.XPWaitSpeed = HUD.SettingsData.KeyExpWaitSpeed.Value;

                TestExpGroup.alpha = HUD.HUDSW3 ? 1 : 0;

                TestExpRect.anchoredPosition = HUD.SettingsData.KeyKillExpAnchoredPosition.Value + new Vector2(HUD.Info.sizeDelta.x / 3.2f, 0);
                TestExpRect.sizeDelta = HUD.SettingsData.KeyKillExpSizeDelta.Value;
                TestExpRect.localScale = HUD.SettingsData.KeyKillExpLocalScale.Value;

                _TestExp.XPColor = HUD.SettingsData.KeyExpColor.Value.ColorToHtml();
                _TestExp.XPStyles = HUD.SettingsData.KeyExpStyles.Value;
                _TestExp.XPWaitSpeed = HUD.SettingsData.KeyExpWaitSpeed.Value;
            }
        }

        void ShowKill(GamePanelHUDHitPlugin.KillInfo killinfo)
        {
            if (_Kills != null)
            {
                Transform kills = killinfo.IsTest ? _TestKills : _Kills;

                GamePanelHUDExpUI exp = killinfo.IsTest ? _TestExp : _Exp;

                int baseExp = ExperienceHelp.GetBaseExp(killinfo.Exp, killinfo.Side);

                int hasExp = baseExp;

                int hasInfo = 1;

                List<GamePanelHUDKillUI> hasKills = new List<GamePanelHUDKillUI>();

                if (killinfo.Distance >= HUD.SettingsData.KeyKillDistance.Value && HUD.SettingsData.KeyKillHasDistance.Value)
                {
                    hasKills.Add(AddDistanceInfo(killinfo, HUD.SettingsData, kills));
                }

                if (killinfo.Kills > 1 && HUD.SettingsData.KeyKillHasStreak.Value)
                {
                    int streakXp = ExperienceHelp.GetStreakExp(killinfo.Exp, killinfo.Side, killinfo.Kills);

                    hasKills.Add(AddStreakInfo(killinfo, HUD.SettingsData, kills, streakXp));

                    hasExp += streakXp;

                    hasInfo++;
                }

                if (killinfo.Part == EBodyPart.Head && HUD.SettingsData.KeyKillHasOther.Value)
                {
                    int headXp = ExperienceHelp.GetHeadExp(killinfo.Exp, killinfo.Side);

                    hasKills.Add(AddOtherInfo(HUD.SettingsData, kills, LocalizedHelp.Localized("StatsHeadshot", EStringCase.None), headXp, true));

                    hasExp += headXp;

                    hasInfo++;
                }

                hasKills.Add(AddKillInfo(killinfo, HUD.SettingsData, kills, baseExp));

                int hasKillsCount = hasKills.Count;

                int count = hasKillsCount - 1;

                if (!HUD.SettingsData.KeyKillWaitBottom.Value)
                {
                    if (Previous != null)
                    {
                        Previous.After = hasKills.Last();

                        count--;
                    }

                    hasKills.First().CanDestroy = true;
                }
                else
                {
                    if (Previous != null)
                    {
                        Previous.After = hasKills.First();

                        hasKills.Last().After = Previous;
                    }
                    else
                    {
                        hasKills.First().CanDestroy = true;
                    }
                }

                Previous = hasKills.Last();

                if (hasKillsCount > 1)
                {
                    for (int i = 0; i < count; i++)
                    {
                        hasKills[i].After = hasKills[i + 1];
                    }
                }

                if (NowHasInfo > 0 && !exp.IsAnim())
                {
                    exp.XpUp(hasExp, LastXp);
                }
                else if (exp.IsAnim() || hasInfo > 1)
                {
                    exp.XpUp(hasExp, 0);
                }
                else
                {
                    LastXp = hasExp;
                }
            }
        }

        GamePanelHUDKillUI AddKillInfo(GamePanelHUDHitPlugin.KillInfo killinfo, GamePanelHUDHitPlugin.SettingsData settingsdata, Transform _kills, int exp)
        {
            GameObject kill = Instantiate(GamePanelHUDHitPlugin.KillPrefab, _kills);

            GamePanelHUDKillUI _kill = kill.GetComponent<GamePanelHUDKillUI>();

            bool isScav = killinfo.Side == EPlayerSide.Savage;

            string playerName;
            string weaponName;
            string sideName;

            if (isScav && settingsdata.KeyKillScavEn.Value)
            {
                playerName = RuToEn.Transliterate(killinfo.PlayerName);
            }
            else
            {
                playerName = killinfo.PlayerName;
            }

            weaponName = LocalizedHelp.Localized(killinfo.WeaponName, EStringCase.None);

            string sideKey;
            if (isScav && RoleHelp.IsBossOrFollower(killinfo.Role))
            {
                sideKey = RoleHelp.GetScavRoleKey(killinfo.Role);
            }
            else
            {
                sideKey = killinfo.Side.ToString();
            }

            sideName = LocalizedHelp.Localized(sideKey, EStringCase.None);

            string nameColor = settingsdata.KeyKillNameColor.Value.ColorToHtml();
            string partColor = settingsdata.KeyKillPartColor.Value.ColorToHtml();
            string sideColor = settingsdata.KeyKillUsecColor.Value.ColorToHtml();
            string weaponColor = settingsdata.KeyKillWeaponColor.Value.ColorToHtml();
            string lvlColor = settingsdata.KeyKillLvlColor.Value.ColorToHtml();
            string levelColor = settingsdata.KeyKillLevelColor.Value.ColorToHtml();
            string bracketColor = settingsdata.KeyKillBracketColor.Value.ColorToHtml();
            string enemyDownColor = settingsdata.KeyKillEnemyDownColor.Value.ColorToHtml();

            if (isScav && RoleHelp.IsBoss(killinfo.Role))
            {
                sideColor = settingsdata.KeyKillBossColor.Value.ColorToHtml();
            }
            else if (isScav && RoleHelp.IsFollower(killinfo.Role))
            {
                sideColor = settingsdata.KeyKillFollowerColor.Value.ColorToHtml();
            }
            else
            {
                switch (killinfo.Side)
                {
                    case EPlayerSide.Usec:
                        sideColor = settingsdata.KeyKillUsecColor.Value.ColorToHtml();
                        break;
                    case EPlayerSide.Bear:
                        sideColor = settingsdata.KeyKillBearColor.Value.ColorToHtml();
                        break;
                    case EPlayerSide.Savage:
                        sideColor = settingsdata.KeyKillScavColor.Value.ColorToHtml();
                        break;
                }
            }

            _kill.Xp = exp;

            string part = settingsdata.KeyKillHasPart.Value ? string.Concat("<color=", bracketColor, ">", "[", "</color>", "<color=", partColor, ">", LocalizedHelp.Localized(killinfo.Part.ToString(), EStringCase.None), "</color>", "<color=", bracketColor, ">", "]", "</color>") : "";
            string side = settingsdata.KeyKillHasSide.Value ? string.Concat("<color=", bracketColor, ">", "[", "</color>", "<color=", sideColor, ">", sideName, "</color>", "<color=", bracketColor, ">", "]", "</color>") : "";
            string weapon = settingsdata.KeyKillHasWeapon.Value ? string.Concat("<color=", bracketColor, ">", "[", "</color>", "<color=", weaponColor, ">", weaponName, "</color>", "</color>", "<color=", bracketColor, ">", "]", "</color>") : "";
            string level = !isScav && settingsdata.KeyKillHasLevel.Value ? string.Concat("<color=", bracketColor, ">", "[", "</color>", "<color=", lvlColor, ">", LocalizedHelp.Localized("LVLKILLLIST", EStringCase.None), ".", "</color>", "</color>", "<color=", levelColor, ">", killinfo.Level, "</color>", "</color>", "<color=", bracketColor, ">", "]", "</color>") : "";

            _kill.Text = string.Concat("<color=", enemyDownColor, ">", LocalizedHelp.Localized("ENEMYDOWN", EStringCase.None), "</color>"); ;
            _kill.Text2 = string.Concat(weapon, side, " ", "<color=", nameColor, ">", playerName, "</color>", " ", level, part);

            _kill.TextFontStyles = settingsdata.KeyKillInfoStyles.Value;

            _kill.IsKillInfo = true;

            _kill.HasXp = settingsdata.KeyKillHasXp.Value;

            _kill.transform.SetAsFirstSibling();

            _kill.Active = true;

            return _kill;
        }

        GamePanelHUDKillUI AddDistanceInfo(GamePanelHUDHitPlugin.KillInfo killinfo, GamePanelHUDHitPlugin.SettingsData settingsdata, Transform _kills)
        {
            GameObject kill = Instantiate(GamePanelHUDHitPlugin.KillPrefab, _kills);

            GamePanelHUDKillUI _kill = kill.GetComponent<GamePanelHUDKillUI>();

            string meters = LocalizedHelp.Localized("meters", EStringCase.None);

            string metersColor = settingsdata.KeyKillMetersColor.Value.ColorToHtml();

            string distanceColor = settingsdata.KeyKillDistanceColor.Value.ColorToHtml();

            string distanceText = killinfo.Distance.ToString("F2");

            _kill.Text = string.Concat("<color=", distanceColor, ">", distanceText, " ", "<color=", metersColor, ">", meters);
            _kill.TextFontStyles = settingsdata.KeyKillDistanceStyles.Value;

            _kill.transform.SetAsFirstSibling();

            _kill.Active = true;

            return _kill;
        }

        GamePanelHUDKillUI AddStreakInfo(GamePanelHUDHitPlugin.KillInfo killinfo, GamePanelHUDHitPlugin.SettingsData settingsdata, Transform _kills, int exp)
        {
            GameObject kill = Instantiate(GamePanelHUDHitPlugin.KillPrefab, _kills);

            GamePanelHUDKillUI _kill = kill.GetComponent<GamePanelHUDKillUI>();

            string statsstreakColor = settingsdata.KeyKillStatsStreakColor.Value.ColorToHtml();
            string streakColor = settingsdata.KeyKillStreakColor.Value.ColorToHtml();
            string bracketColor = settingsdata.KeyKillBracketColor.Value.ColorToHtml();

            _kill.Text = string.Concat("<color=", statsstreakColor, ">", LocalizedHelp.Localized("StatsStreak", EStringCase.None), "</color>", " ", "<color=", bracketColor, ">", "[", "</color>", "<color=", streakColor, ">", killinfo.Kills, "</color>", "<color=", bracketColor, ">", "]", "</color>");
            _kill.TextFontStyles = settingsdata.KeyKillStreakStyles.Value;

            _kill.Xp = exp;

            _kill.HasXp = settingsdata.KeyKillHasXp.Value;

            _kill.transform.SetAsFirstSibling();

            _kill.Active = true;

            return _kill;
        }

        GamePanelHUDKillUI AddOtherInfo(GamePanelHUDHitPlugin.SettingsData settingsdata, Transform _kills, string text, int exp, bool hasxp)
        {
            GameObject kill = Instantiate(GamePanelHUDHitPlugin.KillPrefab, _kills);

            GamePanelHUDKillUI _kill = kill.GetComponent<GamePanelHUDKillUI>();

            _kill.Text = string.Concat("<color=", settingsdata.KeyKillOtherColor.Value.ColorToHtml(), ">", text, "</color>");
            _kill.TextFontStyles = settingsdata.KeyKillOtherStyles.Value;

            _kill.Xp = exp;

            _kill.HasXp = hasxp && settingsdata.KeyKillHasXp.Value;

            _kill.transform.SetAsFirstSibling();

            _kill.Active = true;

            return _kill;
        }

        void InfoAdd()
        {
            NowHasInfo++;
            WaitInfo++;
        }

        void InfoMinu()
        {
            NowHasInfo--;
        }

        void WaitInfoMinu()
        {
            WaitInfo--;

            if (WaitInfo == 0)
            {
                _Exp.XpComplete();
                _TestExp.XpComplete();
            }
        }
#endif
    }
}
