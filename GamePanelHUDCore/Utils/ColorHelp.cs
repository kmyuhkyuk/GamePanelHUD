#if !UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace GamePanelHUDCore.Utils
{
    public static class ColorHelp
    {
        private static Dictionary<Color, string> HexColorPool = new Dictionary<Color, string>();

        public static string ColorToHtml(this Color color)
        {
            if (HexColorPool.TryGetValue(color, out string hexColor))
            {
                return hexColor;
            }
            else
            {
                hexColor = string.Concat("#", ColorUtility.ToHtmlStringRGBA(color));

                HexColorPool.Add(color, hexColor);

                return hexColor;
            }
        }
    }
}
#endif
