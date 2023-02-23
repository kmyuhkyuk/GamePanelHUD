#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using EFT;
using EFT.Interactive;
using GamePanelHUDCore.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GamePanelHUDCompass.Patches
{
    public class AirdropSynchronizableObjectPatch : ModulePatch
    {
        private static readonly MethodBase AirdropSynchronizableObjectBase;

        static AirdropSynchronizableObjectPatch()
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            AirdropSynchronizableObjectBase = RefHelp.GetEftMethod(x => x.Name == "AirdropSynchronizableObject", flags, x => x.Name == "Init");
        }

        protected override MethodBase GetTargetMethod()
        {
            return AirdropSynchronizableObjectBase;
        }

        [PatchPostfix]
        private static void PatchPostfix(object __instance, Vector3 position)
        {
            GamePanelHUDCompassPlugin.CompassStaticInfo staticInfo = new GamePanelHUDCompassPlugin.CompassStaticInfo()
            {
                Where = position,
                QuestType = GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Airdrop
            };

            GamePanelHUDCompassPlugin.ShowStatic(staticInfo);
        }
    }
}
#endif
