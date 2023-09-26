#if !UNITY_EDITOR

using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using EFT;
using GamePanelHUDCore;
using HarmonyLib;

namespace GamePanelHUDDebug.MagDebug
{
    [BepInPlugin("com.kmyuhkyuk.OperationRecord", "kmyuhkyuk-OperationRecord", "1.0.0")]
    [BepInDependency("com.kmyuhkyuk.GamePanelHUDCore")]
    public class OperationRecord : BaseUnityPlugin
    {
        private GamePanelHUDCorePlugin.HUDCoreClass HUDCore
        {
            get { return GamePanelHUDCorePlugin.HUDCore; }
        }

        public Player.FirearmController firearmController;

        public static object CurrentOperation;

        public static HashSet<object> Operations = new HashSet<object>();

        public static ConfigEntry<bool> KeyRecord;

        public static ConfigEntry<KeyboardShortcut> KbsRecord;

        public static ConfigEntry<KeyboardShortcut> KbsClear;

        private void Start()
        {
            KeyRecord = Config.Bind<bool>("KeyRecord", string.Empty, false);

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
                Operations.Clear();
            }

            if (HUDCore.YourPlayer != null)
            {
                firearmController = HUDCore.YourPlayer.HandsController as Player.FirearmController;

                if (firearmController != null)
                {
                    CurrentOperation = Traverse.Create(firearmController).Property("CurrentOperation")
                        .GetValue<object>();

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