#if !UNITY_EDITOR
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using System.Collections.Generic;
using EFT;
using GamePanelHUDCore;

namespace GamePanelHUDDebug.MagDebug
{
    [BepInPlugin("com.kmyuhkyuk.OperationRecord", "kmyuhkyuk-OperationRecord", "1.0.0")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class OperationRecord : BaseUnityPlugin
    {
        public Player IsYourPlayer;

        public object FirearmController;

        public static object CurrentOperation;

        public static HashSet<object> Operations = new HashSet<object>();

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

            if (IsYourPlayer != null)
            {
                FirearmController = IsYourPlayer.HandsController;

                if (KBSRecord.Value.IsDown())
                {
                    KeyRecord.Value = !KeyRecord.Value;
                }

                if (KBSClear.Value.IsDown())
                {
                    Operations.Clear();
                }

                if (FirearmController != null)
                {
                    CurrentOperation = Traverse.Create(FirearmController).Property("CurrentOperation").GetValue<object>();

                    if (KeyRecord.Value)
                    {
                        Operations.Add(CurrentOperation);
                    }
                }
            }
        }
    }
}
#endif
