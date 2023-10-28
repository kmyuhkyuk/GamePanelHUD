#if !UNITY_EDITOR

using EFT;
using GamePanelHUDCore.Models;
using GamePanelHUDKill.Models;
using UnityEngine;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDKill
{
    public partial class GamePanelHUDKillPlugin
    {
        private static void OnBeenKilledByAggressor(Player __instance, Player aggressor, DamageInfo damageInfo,
            EBodyPart bodyPart)
        {
            if (aggressor == HUDCoreModel.Instance.YourPlayer)
            {
                var killHUDModel = KillHUDModel.Instance;
                var settings = _PlayerHelper.RefSettings.GetValue(__instance.Profile.Info);

                var killModel = new KillModel
                {
                    PlayerName = __instance.Profile.Nickname,
                    WeaponName = damageInfo.Weapon.ShortName,
                    Part = bodyPart,
                    Distance = Vector3.Distance(aggressor.Position, __instance.Position),
                    Level = __instance.Profile.Info.Level,
                    Side = __instance.Profile.Info.Side,
                    Exp = _PlayerHelper.RefExperience.GetValue(settings),
                    Role = _PlayerHelper.RefRole.GetValue(settings),
                    KillCount = killHUDModel.KillCount++
                };

                killHUDModel.ShowKill(killModel);
            }
        }
    }
}

#endif