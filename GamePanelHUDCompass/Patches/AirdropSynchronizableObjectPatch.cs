using Aki.Reflection.Patching;
using EFT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GamePanelHUDCompass.Patches
{
    public class AirdropSynchronizableObjectPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(AirdropSynchronizableObject).GetMethod("Init", BindingFlags.Public | BindingFlags.Instance);
        }
    }
}
