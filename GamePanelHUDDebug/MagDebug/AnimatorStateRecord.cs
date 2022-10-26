#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using EFT;
using EFT.InventoryLogic;
using GamePanelHUDCore;

namespace GamePanelHUDDebug.MagDebug
{
    [BepInPlugin("com.kmyuhkyuk.AnimatorStateRecord", "kmyuhkyuk-AnimatorStateRecord", "1.0.0")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class AnimatorStateRecord : BaseUnityPlugin
    {
        public Player IsYourPlayer;

        public static Weapon weapon_0;

        public static Animator animator;

        public static int CurrentState;

        public static string CurrentCilp;

        public static HashSet<int> States = new HashSet<int>();

        public static HashSet<string> Cilps = new HashSet<string>();

        public static Dictionary<int, string> StatesandCilps = new Dictionary<int, string>();

        public static ConfigEntry<bool> KeyRecord { get; set; }

        public static ConfigEntry<KeyboardShortcut> KBSRecord { get; set; }

        public static ConfigEntry<KeyboardShortcut> KBSClear { get; set; }

        void Start()
        {
            KeyRecord = Config.Bind<bool>("KeyRecord", "", false);

            KBSRecord = Config.Bind<KeyboardShortcut>("Record KeyboardShortcut", "", KeyboardShortcut.Empty);

            KBSClear = Config.Bind<KeyboardShortcut>("All Claer", "", KeyboardShortcut.Empty);
        }

        void Update()
        {
            IsYourPlayer = GamePanelHUDCorePlugin.HUDCore.IsYourPlayer;

            if (KBSRecord.Value.IsDown())
            {
                KeyRecord.Value = !KeyRecord.Value;
            }

            if (KBSClear.Value.IsDown())
            {
                States.Clear();
                Cilps.Clear();
            }

            if (IsYourPlayer != null)
            {
                var weapon = IsYourPlayer.WeaponRoot.Original.gameObject;

                //Get WeaponPrefab
                var weaponprefab = weapon.GetComponentInParent<WeaponPrefab>();

                weapon_0 = Traverse.Create(weaponprefab).Field("weapon_0").GetValue<Weapon>();

                StatesandCilps = States.Zip(Cilps, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

                if (weapon_0 != null)
                {
                    animator = weapon.GetComponentInParent<Animator>();

                    CurrentState = animator.GetCurrentAnimatorStateInfo(1).fullPathHash;

                    CurrentCilp = animator.GetCurrentAnimatorClipInfo(1)[0].clip.name;

                    if (KeyRecord.Value)
                    {
                        States.Add(CurrentState);
                        Cilps.Add(CurrentCilp);
                    }
                }
            }
        }
    }
}
#endif
