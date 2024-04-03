#if !UNITY_EDITOR

using EFT.Interactive;
using UnityEngine;
using static EFTApi.EFTHelpers;

namespace GamePanelHUDCompass
{
    public partial class GamePanelHUDCompassPlugin
    {
        // ReSharper disable once SuggestBaseTypeForParameter
        private static void OnBoxLand(MonoBehaviour __instance, object ___boxSync, LootableContainer ___container)
        {
            string nameKey;
            string descriptionKey;
            switch (_AirdropSynchronizableObjectHelper.RefAirdropType?.GetValue(___boxSync))
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

            BaseOnBoxLand(__instance.transform.position, nameKey, descriptionKey, ___container);
        }
    }
}

#endif