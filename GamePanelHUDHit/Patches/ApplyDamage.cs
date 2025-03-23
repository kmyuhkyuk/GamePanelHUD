#if !UNITY_EDITOR

using EFT.InventoryLogic;
using GamePanelHUDHit.Models;
using HarmonyLib;
using MonoMod.Cil;
using MonoMod.Utils;

namespace GamePanelHUDHit
{
    public partial class GamePanelHUDHitPlugin
    {
        private static void ApplyDamage(ILContext il)
        {
            var codes = il.Instrs;

            var cursor = new ILCursor(il);

            var processor = il.IL;

            var callApplyDurabilityDamage = cursor.GotoNext(x =>
                x.MatchCall(AccessTools.Method(typeof(ArmorComponent), "ApplyDurabilityDamage")));

            codes.InsertRange(
                //Parameters Start Index
                callApplyDurabilityDamage.Index - 2, new[]
                {
                    //Get ArmorModel Instance
                    processor.Create(Mono.Cecil.Cil.OpCodes.Call,
                        AccessTools.PropertyGetter(typeof(ArmorModel), nameof(ArmorModel.Instance))),
                    //Get DamageInfo
                    processor.Create(Mono.Cecil.Cil.OpCodes.Ldarg_1),
                    //Convert ref struct
                    processor.Create(Mono.Cecil.Cil.OpCodes.Ldobj, typeof(DamageInfoStruct)),
                    //Get ApplyDurabilityDamage first parameter
                    callApplyDurabilityDamage.Prev.Previous,
                    processor.Create(Mono.Cecil.Cil.OpCodes.Callvirt,
                        AccessTools.Method(typeof(ArmorModel), nameof(ArmorModel.Set)))
                });
        }
    }
}

#endif