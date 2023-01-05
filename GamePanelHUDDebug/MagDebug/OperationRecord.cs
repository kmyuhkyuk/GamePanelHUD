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
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore
        {
            get
            {
                return GamePanelHUDCorePlugin.HUDCore;
            }
        }

        public Player.FirearmController FirearmController;

        public static object CurrentOperation;

        public static HashSet<object> Operations = new HashSet<object>();

        public static ConfigEntry<bool> KeyRecord;

        public static ConfigEntry<KeyboardShortcut> KBSRecord;

        public static ConfigEntry<KeyboardShortcut> KBSClear;

        void Start()
        {
            KeyRecord = Config.Bind<bool>("KeyRecord", "", false);

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
                Operations.Clear();
            }

            if (HUDCore.IsYourPlayer != null)
            {
                FirearmController = HUDCore.IsYourPlayer.HandsController as Player.FirearmController;

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
