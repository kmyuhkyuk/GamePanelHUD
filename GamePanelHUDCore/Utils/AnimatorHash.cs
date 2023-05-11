using UnityEngine;

namespace GamePanelHUDCore.Utils
{
    public static class AnimatorHash
    {
        public static readonly int Active = Animator.StringToHash("Active");

        public static readonly int Reverse = Animator.StringToHash("Reverse");

        public static readonly int Speed = Animator.StringToHash("Speed");

        public static readonly int Always = Animator.StringToHash("Always");

        public static readonly int Zero = Animator.StringToHash("Zero");

        public static readonly int MagInWeapon = Animator.StringToHash("MagInWeapon");

        public static readonly int AmmoInChamber = Animator.StringToHash("AmmoInChamber");

        public static readonly int AmmoInMag = Animator.StringToHash("AmmoInMag");

        public static readonly int Complete = Animator.StringToHash("Complete");

        public static readonly int ActiveDead = Animator.StringToHash("ActiveDead");

        public static readonly int ActiveSpeed = Animator.StringToHash("ActiveSpeed");

        public static readonly int EndSpeed = Animator.StringToHash("EndSpeed");

        public static readonly int DeadSpeed = Animator.StringToHash("DeadSpeed");

        public static readonly int ActiveLeft = Animator.StringToHash("ActiveLeft");

        public static readonly int ActiveRight = Animator.StringToHash("ActiveRight");

        public static readonly int Passive = Animator.StringToHash("Passive");

        public static readonly int Clear = Animator.StringToHash("Clear");

        public static readonly int CanDestroy = Animator.StringToHash("CanDestroy");

        public static readonly int Fire = Animator.StringToHash("Fire");

        public static readonly int ToSmallSpeed = Animator.StringToHash("ToSmallSpeed");

        public static readonly int SmallSpeed = Animator.StringToHash("SmallSpeed");
    }
}
