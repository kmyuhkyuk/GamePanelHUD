#if !UNITY_EDITOR

using EFT;
using GamePanelHUDCore.Models;
using GamePanelHUDKill.Models;
using UnityEngine;

namespace GamePanelHUDKill
{
    public partial class GamePanelHUDKillPlugin
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        private static void OnBeenKilledByAggressor(Player __instance, Player aggressor, DamageInfoStruct damageInfo,
            EBodyPart bodyPart)
        {
            if (aggressor != HUDCoreModel.Instance.YourPlayer)
                return;

            var killHUDModel = KillHUDModel.Instance;
            var settings = __instance.Profile.Info.Settings;

            var hasMarkOfUnknown = aggressor.HasMarkOfUnknown(out var markOfUnknown);

            var killModel = new KillModel
            {
                PlayerName = __instance.Profile.Nickname,
                WeaponName = damageInfo.Weapon.ShortName,
                Part = bodyPart,
                Distance = Vector3.Distance(aggressor.Position, __instance.Position),
                Level = __instance.Profile.Info.Level,
                Side = __instance.Profile.Info.Side,
                Exp = settings.Experience,
                Role = settings.Role,
                KillCount = killHUDModel.KillCount++,
                ScavKillExpPenalty = markOfUnknown?.ScavKillExpPenalty ?? 0,
                HasMarkOfUnknown = hasMarkOfUnknown,
                IsAI = __instance.IsAI,
            };

            killHUDModel.ShowKill(killModel);
        }
    }
}

#endif