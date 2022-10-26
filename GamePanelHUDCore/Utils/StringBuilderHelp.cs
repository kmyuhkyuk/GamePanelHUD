using System.Text;

namespace GamePanelHUDCore.Utils
{
    public static class StringBuilderHelp
    {
        public static string StringConcat(this StringBuilder sb, params object[] texts)
        {
            sb.Clear();

            foreach (object text in texts)
            {
                sb.Append(text);
            }

            return sb.ToString();
        }

        public static string StringConcat(this StringBuilder sb, params string[] texts)
        {
            sb.Clear();

            foreach (string text in texts)
            {
                sb.Append(text);
            }

            return sb.ToString();
        }
    }
}
