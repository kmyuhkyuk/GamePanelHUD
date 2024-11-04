#if !UNITY_EDITOR

using System.Collections.Generic;

namespace GamePanelHUDCompass.Models
{
    internal class CompassStaticModel
    {
        public HashSet<object> EquipmentAndQuestRaidItemHashSet;

        public string YourProfileId;

        public List<string> TriggerZones;

        public bool HasEquipmentAndQuestRaidItemHashSet => EquipmentAndQuestRaidItemHashSet != null;
    }
}

#endif