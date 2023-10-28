#if !UNITY_EDITOR

using System.Collections.Generic;

namespace GamePanelHUDCompass.Models
{
    internal class CompassStaticModel
    {
        public HashSet<string> EquipmentAndQuestRaidItems;

        public string YourProfileId;

        public List<string> TriggerZones;

        public bool HasEquipmentAndQuestRaidItems => EquipmentAndQuestRaidItems != null;
    }
}

#endif