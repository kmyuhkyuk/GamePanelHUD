#if !UNITY_EDITOR
using EFT;
using UnityEngine;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDHit
{
    public partial class GamePanelHUDHitPlugin
    {
        private static void OnBeenKilledByAggressor(Player __instance, Player aggressor, DamageInfo damageInfo,
            EBodyPart bodyPart)
        {
            if (aggressor == HUDCore.YourPlayer)
            {
                var settings = _PlayerHelper.RefSettings.GetValue(__instance.Profile.Info);

                var info = new KillInfo
                {
                    PlayerName = __instance.Profile.Nickname,
                    WeaponName = damageInfo.Weapon.ShortName,
                    Part = bodyPart,
                    Distance = Vector3.Distance(aggressor.Position, __instance.Position),
                    Level = __instance.Profile.Info.Level,
                    Side = __instance.Profile.Info.Side,
                    Exp = _PlayerHelper.RefExperience.GetValue(settings),
                    Role = _PlayerHelper.RefRole.GetValue(settings),
                    Kills = _kills++
                };

                ShowKill(info);
            }
        }
    }
}
#endif