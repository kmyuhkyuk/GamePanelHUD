/*
#if !UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using GamePanelHUDMag;

namespace GamePanelHUDDebug.MagDebug
{
    public class JsonGenerate
    {
        public static string filepath = AppDomain.CurrentDomain.BaseDirectory + "/BepInEx/plugins/kmyuhkyuk-GamePanelHUD/data/WeaponAnimatorStateData.json";

        public static GamePanelHUDMagPlugin.AnimatorStateData animatorstatedata = new GamePanelHUDMagPlugin.AnimatorStateData()
        {
            ExternalMagazine = new List<int>()
            {
                1051831248,
                2082422867,
                -987374909,
                -1191338705,
                1282108526,
                925388172,
                -1821557201,
                -1847790525,
                -507049588,
                1998482584,
                1310349991,
                -1759550432,
                32228103,
                -414370645,
                1012780759,
                1225680280,
                1602605308,
                -1162171678,
                -672397693,
                1709803875,
                50750893,
                1898839857,
                -1698903808,
                627807580,
                279177720,
                -686628182,
                1058993437,
                -1200126267
            },

            OnlyBarrel = new List<int>()
            {
                -2070245439,
                1051831248,
                1200289219,
                -1465457249,
                -295977112,
                -1821557201,
                -1266835649,
                32228103,
                -1987535338,
                1898839857,
                -388762365,
                814597461,
                -1191338705
            },

            InternalMagazine = new List<int>()
            {
                1051831248,
                -1756868607,
                -572946515,
                794592048,
                1887866334,
                -1821557201,
                -1767259010,
                32228103,
                1225680280,
                474878765,
                722366425,
                867766280,
                -1048008908,
                279177720,
                1058993437
            },

            ExternalMagazineWithInternalReloadSupport = new List<int>()
            {
                -987374909,
                10403175,
                925388172,
                -1191338705,
                -1847790525,
                -1821557201,
                -1759550432,
                -414370645,
                1012780759,
                1602605308,
                1709803875,
                50750893,
                -1698903808,
                1898839857,
                1051831248,
                -1756868607,
                1887866334,
                1225680280,
                -1767259010,
                1058993437
            }
        };

        public static void Generate()
        {
            StreamWriter sw = new StreamWriter(filepath);

            sw.WriteLine(JsonConvert.SerializeObject(animatorstatedata, Formatting.Indented));

            sw.Close();
        }
    }
}
#endif
*/
