#if !UNITY_EDITOR

using EFT;
using GamePanelHUDCore.Models;
using static KmyTarkovApi.EFTHelpers;

namespace GamePanelHUDHit
{
    public partial class GamePanelHUDHitPlugin
    {
        private static void CoopApplyShot(Player __instance, DamageInfoStruct damageInfo, EBodyPart bodyPartType,
            EBodyPartColliderType colliderType)
        {
            if ((Player)damageInfo.Player?.iPlayer != HUDCoreModel.Instance.YourPlayer)
                return;

            //Clone HealthController to do local compute
            var store = _HealthControllerHelper.ObservedCoopStore(
                (NetworkHealthControllerAbstractClass)__instance.HealthController);
            var inventoryController = __instance.InventoryController;
            var skillManager = __instance.Skills;
            var coopHealthController = _HealthControllerHelper.CoopHealthControllerCreate(store, __instance,
                inventoryController, skillManager, __instance.IsAI);

            var damage = damageInfo.Damage;

            coopHealthController.DoWoundRelapse(damage, bodyPartType);

            damageInfo.BleedBlock = _PlayerHelper.CoopGetBleedBlock(__instance, colliderType);

            var didBodyDamage =
                _HealthControllerHelper.CoopApplyDamage(coopHealthController, bodyPartType, damage, damageInfo);

            damageInfo.DidBodyDamage = didBodyDamage;

            coopHealthController.BluntContusion(bodyPartType, 0);

            if (didBodyDamage >= float.Epsilon)
            {
                coopHealthController.TryApplySideEffects(damageInfo, bodyPartType, out _);
            }

            BaseApplyDamageInfo(damageInfo, bodyPartType, coopHealthController);
        }
    }
}

#endif