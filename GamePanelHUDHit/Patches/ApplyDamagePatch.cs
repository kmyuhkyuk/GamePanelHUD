#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System.Reflection;
using EFT.InventoryLogic;
using GamePanelHUDCore;

namespace GamePanelHUDHit.Patches
{
    public class ApplyDamagePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ArmorComponent).GetMethod("ApplyDamage", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(DamageInfo damageInfo, float armorDamage)
        {
            if (damageInfo.Player == GamePanelHUDCorePlugin.HUDCore.IsYourPlayer)
            {
                GamePanelHUDHitPlugin.Armor.SetActiva(armorDamage);
            }
        }
    }
}
#endif
