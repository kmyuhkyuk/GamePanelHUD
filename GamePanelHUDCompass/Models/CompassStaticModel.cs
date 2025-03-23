#if !UNITY_EDITOR

using System.Collections.Generic;
using EFT;

namespace GamePanelHUDCompass.Models
{
    internal class CompassStaticModel
    {
        public HashSet<MongoID> EquipmentAndQuestRaidItemHashSet;

        public List<string> TriggerZones;

        public bool HasEquipmentAndQuestRaidItemHashSet => EquipmentAndQuestRaidItemHashSet != null;
    }
}

#endif