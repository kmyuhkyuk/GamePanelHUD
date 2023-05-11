#if !UNITY_EDITOR
using Aki.Reflection.Patching;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using EFT.Interactive;
using EFT.InventoryLogic;
using GamePanelHUDCore;
using GamePanelHUDCore.Utils;

namespace GamePanelHUDCompass.Patches
{
    public class AirdropBoxPatch : ModulePatch
    {
        private static readonly bool Is350Up = GamePanelHUDCorePlugin.HUDCoreClass.GameVersion > new Version("0.13.0.21734");

        private static readonly ReflectionData RefData = new ReflectionData();

        private static readonly Type AirdropType;

        static AirdropBoxPatch()
        {
            if (Is350Up)
            {
                AirdropType = AppDomain.CurrentDomain.GetAssemblies().Single(x => x.ManifestModule.Name == "aki-custom.dll").GetTypes().Single(x => x.Name == "AirdropBox");

                RefData.RefAirdropType = RefHelp.FieldRef<object, int>.Create(RefHelp.GetEftType(x => x.Name == "AirdropSynchronizableObject"), "AirdropType");
                RefData.RefItemOwner = RefHelp.FieldRef<LootableContainer, object>.Create("ItemOwner");
                RefData.RefAllSearchersIds = RefHelp.FieldRef<Item, List<string>>.Create(RefHelp.GetEftType(x => x.GetMethod("AddNewSearcher", BindingFlags.DeclaredOnly |BindingFlags.Public | BindingFlags.Instance) != null), "_allSearchersIds");

                RefData.RefRootItem = RefHelp.PropertyRef<object, Item>.Create(RefData.RefItemOwner.FieldType, "RootItem");

            }
        }

        protected override MethodBase GetTargetMethod()
        {
            return AirdropType.GetMethod("OnBoxLand", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [PatchPostfix]
        private static void PatchPostfix(MonoBehaviour __instance, object ___boxSync)
        {
            LootableContainer looTable = __instance.GetComponentInChildren<LootableContainer>();

            object controller = RefData.RefItemOwner.GetValue(looTable);

            Item item = RefData.RefRootItem.GetValue(controller);

            GamePanelHUDCompassPlugin.Airdrops.Add(RefData.RefAllSearchersIds.GetValue(item));

            int count = GamePanelHUDCompassPlugin.Airdrops.Count;

            string nameKey;
            string descriptionKey;
            switch (RefData.RefAirdropType.GetValue(___boxSync))
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
                    nameKey = "Unknown";
                    descriptionKey = "Unknown";
                    break;
            }

            GamePanelHUDCompassPlugin.CompassStaticInfo staticInfo = new GamePanelHUDCompassPlugin.CompassStaticInfo()
            {
                Id = string.Concat("Airdrop", count),
                Where = __instance.transform.position,
                NameKey = nameKey,
                DescriptionKey = descriptionKey,
                ExIndex = count - 1,
                InfoType = GamePanelHUDCompassPlugin.CompassStaticInfo.Type.Airdrop
            };

            GamePanelHUDCompassPlugin.ShowStatic(staticInfo);
        }

        public class ReflectionData
        {
            public RefHelp.FieldRef<object, int> RefAirdropType;

            public RefHelp.FieldRef<LootableContainer, object> RefItemOwner;
            public RefHelp.FieldRef<Item, List<string>> RefAllSearchersIds;

            public RefHelp.PropertyRef<object, Item> RefRootItem;
        }
    }
}
#endif
