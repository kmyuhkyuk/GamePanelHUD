#if !UNITY_EDITOR

using EFT.InventoryLogic;
using EFTApi;
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

            codes.InsertRange(callApplyDurabilityDamage.Index - 1, new[]
            {
                processor.Create(Mono.Cecil.Cil.OpCodes.Call,
                    AccessTools.PropertyGetter(typeof(ArmorModel), nameof(ArmorModel.Instance))),
                processor.Create(Mono.Cecil.Cil.OpCodes.Ldarg_1),
                processor.Create(Mono.Cecil.Cil.OpCodes.Ldobj, typeof(DamageInfo)),
                EFTVersion.AkiVersion > EFTVersion.Parse("3.4.1")
                    ? callApplyDurabilityDamage.Prev
                    : processor.Create(Mono.Cecil.Cil.OpCodes.Ldarg_3),
                EFTVersion.AkiVersion > EFTVersion.Parse("3.4.1")
                    ? processor.Create(Mono.Cecil.Cil.OpCodes.Nop)
                    : processor.Create(Mono.Cecil.Cil.OpCodes.Ldind_R4),
                processor.Create(Mono.Cecil.Cil.OpCodes.Call,
                    AccessTools.Method(typeof(ArmorModel), nameof(ArmorModel.Set)))
            });
        }
    }
}

#endif