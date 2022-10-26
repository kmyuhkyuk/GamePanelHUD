using System.Collections.Generic;
using UnityEngine;

namespace GamePanelHUDCore.Utils
{
    public static class ColorHelp
    {
        private static Dictionary<Color, string> HexColorPool = new Dictionary<Color, string>();

        public static string ColorToHtml(this Color color)
        {
            string hexClor;

            HexColorPool.TryGetValue(color, out hexClor);

            if (hexClor != null)
            {
                return hexClor;
            }
            else
            {
                string hexColor = string.Concat("#", ColorUtility.ToHtmlStringRGBA(color));

                HexColorPool.Add(color, hexColor);

                return hexColor;
            }
        }
    }
}
