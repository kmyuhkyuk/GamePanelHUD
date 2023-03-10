#if !UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using EFT;
using EFT.Interactive;

namespace GamePanelHUDCore.Utils.Zone
{
    public class ZoneHelp
    {
        private static readonly Dictionary<string, Vector3> ExperienceTriggerPoint = new Dictionary<string, Vector3>();

        private static readonly Dictionary<string, Vector3> PlaceItemTriggerPoint = new Dictionary<string, Vector3>();

        private static readonly Dictionary<string, Vector3> QuestTriggerPoint = new Dictionary<string, Vector3>();

        static ZoneHelp()
        {
            GamePanelHUDCorePlugin.HUDCoreClass.WorldDestroy += Clear;
        }

        public static void AddPoint(TriggerWithId trigger)
        {
            string id = trigger.Id;
            Vector3 pos = trigger.transform.position;

            Dictionary<string, Vector3> point;

            if (trigger is ExperienceTrigger)
            {
                point = ExperienceTriggerPoint;
            }
            else if (trigger is PlaceItemTrigger)
            {
                point = PlaceItemTriggerPoint;
            }
            else
            {
                point = QuestTriggerPoint;
            }

            if (!point.ContainsKey(id))
            {
                point.Add(id, pos);
            }
            else
            {
                point.Add(string.Concat(id, "(", trigger.transform.parent.name, ")"), pos);
            }
        }

        public static bool TryExperience(string id, out Vector3 vector3)
        {
            return ExperienceTriggerPoint.TryGetValue(id, out vector3);
        }

        public static bool TryPlaceItem(string id, out Vector3 vector3)
        {
            return PlaceItemTriggerPoint.TryGetValue(id, out vector3);
        }

        public static bool TryQuest(string id, out Vector3 vector3)
        {
            return QuestTriggerPoint.TryGetValue(id, out vector3);
        }

        private static void Clear(GameWorld world)
        {
            ExperienceTriggerPoint.Clear();
            PlaceItemTriggerPoint.Clear();
            QuestTriggerPoint.Clear();
        }
    }
}
#endif
