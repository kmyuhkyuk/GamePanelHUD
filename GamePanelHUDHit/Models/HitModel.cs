#if !UNITY_EDITOR

using UnityEngine;

namespace GamePanelHUDHit.Models
{
    internal struct HitModel
    {
        public float Damage;

        public EBodyPart DamagePart;

        public Vector3 HitPoint;

        public Vector3 HitDirection;

        public Hit HitType;

        public bool HasArmorHit;

        public float ArmorDamage;

        public bool IsTest;

        public enum Hit
        {
            OnlyHp,
            HasArmorHit,
            Dead,
            Head
        }

        public enum Direction
        {
            Center,
            Left,
            Right
        }
    }
}

#endif