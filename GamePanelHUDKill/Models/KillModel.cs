#if !UNITY_EDITOR

using EFT;

namespace GamePanelHUDKill.Models
{
    internal struct KillModel
    {
        public string PlayerName;

        public string WeaponName;

        public float Distance;

        public int Level;

        public int Exp;

        public int KillCount;

        public EPlayerSide Side;

        public EBodyPart Part;

        public WildSpawnType Role;

        public float ScavKillExpPenalty;

        public bool HasMarkOfUnknown;

        public bool IsAI;

        public bool IsTest;
    }
}

#endif