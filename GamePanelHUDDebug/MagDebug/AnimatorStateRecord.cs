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
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore
        {
            get
            {
                return GamePanelHUDCorePlugin.HUDCore;
            }
        }

        public static Player.FirearmController firearmcontroller;

        public static Weapon weapon;

        public static Animator animator;

        public static int CurrentState;

        public static string CurrentCilp;

        public static HashSet<int> States = new HashSet<int>();

        public static HashSet<string> Cilps = new HashSet<string>();

        public static Dictionary<int, string> StatesandCilps = new Dictionary<int, string>();

        public static ConfigEntry<bool> KeyRecord;

        public static ConfigEntry<bool> KeyLauncher;

        public static ConfigEntry<KeyboardShortcut> KBSRecord;

        public static ConfigEntry<KeyboardShortcut> KBSClear;

        void Start()
        {
            KeyRecord = Config.Bind<bool>("KeyRecord", "", false);

            KeyLauncher = Config.Bind<bool>("KeyLauncher", "", false);

            KBSRecord = Config.Bind<KeyboardShortcut>("Record KeyboardShortcut", "", KeyboardShortcut.Empty);

            KBSClear = Config.Bind<KeyboardShortcut>("All Claer", "", KeyboardShortcut.Empty);
        }

        void Update()
        {
            if (KBSRecord.Value.IsDown())
            {
                KeyRecord.Value = !KeyRecord.Value;
            }

            if (KBSClear.Value.IsDown())
            {
                States.Clear();
                Cilps.Clear();
            }

            if (HUDCore.YourPlayer != null)
            {
                firearmcontroller = HUDCore.YourPlayer.HandsController as Player.FirearmController;

                weapon = firearmcontroller != null ? firearmcontroller.Item : null;

                StatesandCilps = States.Zip(Cilps, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

                if (weapon != null)
                {
                    if (!KeyLauncher.Value)
                    {
                        animator = Traverse.Create(Traverse.Create(HUDCore.YourPlayer).Property("ArmsAnimatorCommon").GetValue<object>()).Property("Animator").GetValue<Animator>();
                    }
                    else
                    {
                        animator = Traverse.Create(Traverse.Create(HUDCore.YourPlayer).Property("UnderbarrelWeaponArmsAnimator").GetValue<object>()).Property("Animator").GetValue<Animator>();
                    }

                    if (animator != null)
                    {
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
}
#endif
