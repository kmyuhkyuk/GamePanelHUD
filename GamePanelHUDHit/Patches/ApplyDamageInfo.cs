#if !UNITY_EDITOR

using EFT;
using GamePanelHUDCore.Models;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDHit
{
    public partial class GamePanelHUDHitPlugin
    {
        private static void ApplyDamageInfo(Player __instance, object damageInfo, EBodyPart bodyPartType)
        {
            if (_DamageInfoHelper.GetPlayer(damageInfo) != HUDCoreModel.Instance.YourPlayer)
                return;

            BaseApplyDamageInfo(damageInfo, bodyPartType,
                _HealthControllerHelper.RefHealthController.GetValue(__instance));
        }
    }
}

#endif