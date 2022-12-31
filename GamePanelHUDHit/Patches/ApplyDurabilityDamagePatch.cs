using Aki.Reflection.Patching;
using System.Reflection;
using EFT.InventoryLogic;

namespace GamePanelHUDHit.Patches
{
    public class ApplyDurabilityDamagePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ArmorComponent).GetMethod("ApplyDurabilityDamage", BindingFlags.Public | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(ArmorComponent __instance, float armorDamage)
        {
            if (__instance == GamePanelHUDHitPlugin.Armor.Component)
            {
                GamePanelHUDHitPlugin.Armor.SetActiva(armorDamage);
            }
        }
    }
}
