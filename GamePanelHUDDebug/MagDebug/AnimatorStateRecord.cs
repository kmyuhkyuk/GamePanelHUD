#if !UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using EFT;
using EFT.InventoryLogic;
using GamePanelHUDCore;
using HarmonyLib;
using UnityEngine;

namespace GamePanelHUDDebug.MagDebug
{
    [BepInPlugin("com.kmyuhkyuk.AnimatorStateRecord", "kmyuhkyuk-AnimatorStateRecord", "1.0.0")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class AnimatorStateRecord : BaseUnityPlugin
    {
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore
        {
            get { return GamePanelHUDCorePlugin.HUDCore; }
        }

        public static Player.FirearmController Firearmcontroller;

        public static Weapon Weapon;

        public static Animator Animator;

        public static int CurrentState;

        public static string CurrentCilp;

        public static HashSet<int> States = new HashSet<int>();

        public static HashSet<string> Cilps = new HashSet<string>();

        public static Dictionary<int, string> StatesandCilps = new Dictionary<int, string>();

        public static ConfigEntry<bool> KeyRecord;

        public static ConfigEntry<bool> KeyLauncher;

        public static ConfigEntry<KeyboardShortcut> KbsRecord;

        public static ConfigEntry<KeyboardShortcut> KbsClear;

        private void Start()
        {
            KeyRecord = Config.Bind<bool>("KeyRecord", string.Empty, false);

            KeyLauncher = Config.Bind<bool>("KeyLauncher", string.Empty, false);

            KbsRecord = Config.Bind<KeyboardShortcut>("Record KeyboardShortcut", string.Empty, KeyboardShortcut.Empty);

            KbsClear = Config.Bind<KeyboardShortcut>("All Claer", string.Empty, KeyboardShortcut.Empty);
        }

        private void Update()
        {
            if (KbsRecord.Value.IsDown())
            {
                KeyRecord.Value = !KeyRecord.Value;
            }

            if (KbsClear.Value.IsDown())
            {
                States.Clear();
                Cilps.Clear();
            }

            if (HUDCore.YourPlayer != null)
            {
                Firearmcontroller = HUDCore.YourPlayer.HandsController as Player.FirearmController;

                Weapon = Firearmcontroller != null ? Firearmcontroller.Item : null;

                StatesandCilps = States.Zip(Cilps, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

                if (Weapon != null)
                {
                    if (!KeyLauncher.Value)
                    {
                        Animator = Traverse
                            .Create(Traverse.Create(HUDCore.YourPlayer).Property("ArmsAnimatorCommon")
                                .GetValue<object>()).Property("Animator").GetValue<Animator>();
                    }
                    else
                    {
                        Animator = Traverse
                            .Create(Traverse.Create(HUDCore.YourPlayer).Property("UnderbarrelWeaponArmsAnimator")
                                .GetValue<object>()).Property("Animator").GetValue<Animator>();
                    }

                    if (Animator != null)
                    {
                        CurrentState = Animator.GetCurrentAnimatorStateInfo(1).fullPathHash;

                        CurrentCilp = Animator.GetCurrentAnimatorClipInfo(1)[0].clip.name;

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