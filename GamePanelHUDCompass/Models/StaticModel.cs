#if !UNITY_EDITOR

using System;
using UnityEngine;

namespace GamePanelHUDCompass.Models
{
    internal struct StaticModel
    {
        public string Id;

        public Vector3 Where;

        public string ZoneId;

        public string[] Target;

        public string NameKey;

        public string DescriptionKey;

        public string TraderId;

        public bool IsNotNecessary;

        public Type InfoType;

        public Func<bool>[] Requirements;

        public enum Type
        {
            Airdrop,
            Exfiltration,
            Switch,
            ConditionLeaveItemAtLocation,
            ConditionPlaceBeacon,
            ConditionFindItem,
            ConditionVisitPlace,
            ConditionInZone
        }
    }
}

#endif