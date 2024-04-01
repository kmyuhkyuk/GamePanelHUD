#if !UNITY_EDITOR

using EFT;
using EFTApi;
using GamePanelHUDCore.Models;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDHit
{
    public partial class GamePanelHUDHitPlugin
    {
        private static void CoopApplyDamageInfo(Player __instance, DamageInfo damageInfo, EBodyPart bodyPartType,
            EBodyPartColliderType colliderType, float absorbed)
        {
            if (_DamageInfoHelper.GetPlayer(damageInfo) != HUDCoreModel.Instance.YourPlayer)
                return;

            //Clone HealthController to do local compute
            var store = _HealthControllerHelper.ObservedCoopStore(
                _HealthControllerHelper.RefHealthController.GetValue(__instance));
            var inventoryController = _PlayerHelper.RefInventoryController.GetValue(__instance);
            var skillManager = _PlayerHelper.RefSkills.GetValue(__instance);
            var coopHealthController = _HealthControllerHelper.CoopHealthControllerCreate(store, __instance,
                inventoryController, skillManager, __instance.IsAI);

            _HealthControllerHelper.DoWoundRelapse(coopHealthController, damageInfo.Damage, bodyPartType);

            if (EFTVersion.AkiVersion > EFTVersion.Parse("3.7.6"))
            {
                _DamageInfoHelper.RefBleedBlock?.SetValue(damageInfo,
                    _PlayerHelper.CoopGetBleedBlock(__instance, (int)colliderType));
            }
            else if (EFTVersion.AkiVersion > EFTVersion.Parse("3.4.1"))
            {
                _DamageInfoHelper.RefBleedBlock?.SetValue(damageInfo,
                    _PlayerHelper.CoopGetBleedBlock(__instance, (int)bodyPartType));
            }

            damageInfo.DidBodyDamage =
                _HealthControllerHelper.CoopApplyDamage(coopHealthController, bodyPartType, damageInfo.Damage,
                    damageInfo);

            _HealthControllerHelper.BluntContusion(coopHealthController, bodyPartType, absorbed);

            if (damageInfo.DidBodyDamage >= float.Epsilon)
            {
                _HealthControllerHelper.TryApplySideEffects(coopHealthController, damageInfo, bodyPartType, out _);
            }

            BaseApplyDamageInfo(damageInfo, bodyPartType, coopHealthController);
        }
    }
}

#endif