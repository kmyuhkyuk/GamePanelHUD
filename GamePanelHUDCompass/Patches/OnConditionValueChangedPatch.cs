#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System.Reflection;
using EFT.Quests;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCompass.Patches
{
    public class OnConditionValueChangedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            BindingFlags flags =  BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance;

            return RefHelp.GetEftType(x => x.GetMethod("OnConditionValueChanged", flags) != null).GetMethod("OnConditionValueChanged", flags);
        }

        [PatchPostfix]
        private static void PatchPostfix(object __instance, EQuestStatus status, Condition condition)
        {
            if (status != EQuestStatus.Started)
            {
                GamePanelHUDCompassPlugin.DestroyStatic(condition.id);
            }
        }
    }
}
#endif
