#if !UNITY_EDITOR
using EFT.Interactive;
using UnityEngine;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        private static void OnBoxLand(MonoBehaviour __instance, object ___boxSync)
        {
            var looTable = __instance.GetComponentInChildren<LootableContainer>();

            var controller = _GameWorldHelper.LootableContainerHelper.RefItemOwner.GetValue(looTable);

            var item = _GameWorldHelper.LootableContainerHelper.RefRootItem.GetValue(controller);

            Airdrops.Add(_GameWorldHelper.SearchableItemClassHelper.RefAllSearchersIds.GetValue(item));

            var count = Airdrops.Count;

            string nameKey;
            string descriptionKey;
            switch (_AirdropHelper.AirdropSynchronizableObjectHelper.RefAirdropType.GetValue(___boxSync))
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

            var staticInfo = new CompassStaticInfo
            {
                Id = $"Airdrop{count}",
                Where = __instance.transform.position,
                NameKey = nameKey,
                DescriptionKey = descriptionKey,
                ExIndex = count - 1,
                InfoType = CompassStaticInfo.Type.Airdrop
            };

            ShowStatic(staticInfo);
        }
    }
}
#endif