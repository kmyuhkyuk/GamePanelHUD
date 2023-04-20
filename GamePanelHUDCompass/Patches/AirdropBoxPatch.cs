#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCompass.Patches
{
    public class AirdropBoxPatch : ModulePatch
    {
        private static bool Is231Up = GamePanelHUDCorePlugin.HUDCoreClass.GameVersion > new Version("0.12.12.17349");

        private static readonly ReflectionData ReflectionDatas = new ReflectionData();

        private static readonly Type AirdropType;

        static AirdropBoxPatch()
        {
            if (Is231Up)
            {
                AirdropType = AppDomain.CurrentDomain.GetAssemblies().Single(x => x.ManifestModule.Name == "aki-custom.dll").GetTypes().Single(x => x.Name == "AirdropBox");

                ReflectionDatas.RefAirdropType = RefHelp.FieldRef<object, int>.Create(RefHelp.GetEftType(x => x.Name == "AirdropSynchronizableObject"), "AirdropType");
            }
        }

        protected override MethodBase GetTargetMethod()
        {
            return AirdropType.GetMethod("OnBoxLand", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(object __instance, object ___boxSync)
        {
            string nameKey;
            string descriptionKey;
            switch (ReflectionDatas.RefAirdropType.GetValue(___boxSync))
            {
                case 0:
                    nameKey = "6223349b3136504a544d1608 Name";
                    descriptionKey = "6223349b3136504a544d1608 Description";
                    break;
                case 1:
                    nameKey = "622334fa3136504a544d160c Name";
                    descriptionKey = "622334fa3136504a544d160c Description";
                    break;
                case 2:
                    nameKey = "622334c873090231d904a9fc Name";
                    descriptionKey = "622334c873090231d904a9fc Description";
                    break;
                case 3:
                    nameKey = "6223351bb5d97a7b2c635ca7 Name";
                    descriptionKey = "6223351bb5d97a7b2c635ca7 Description";
                    break;
                default:
                    nameKey = "Null";
                    descriptionKey = "Null";
                    break;
            }

            GamePanelHUDCompassPlugin.CompassStaticInfo staticInfo = new GamePanelHUDCompassPlugin.CompassStaticInfo()
            {
                Id = string.Concat("Airdrop", GamePanelHUDCompassPlugin.AirdropCount + 1),
                Where = ((MonoBehaviour)__instance).transform.position,
                NameKey = nameKey,
                DescriptionKey = descriptionKey,
                InfoType = GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Airdrop
            };

            GamePanelHUDCompassPlugin.ShowStatic(staticInfo);
        }

        public class ReflectionData
        {
            public RefHelp.FieldRef<object, int> RefAirdropType;
        }
    }
}
#endif
