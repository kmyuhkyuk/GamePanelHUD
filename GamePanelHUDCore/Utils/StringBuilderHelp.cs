using System.Linq;
using System.Text;

namespace GamePanelHUDCore.Utils
{
    public static class StringBuilderHelp
    {
        public class StringChange
        {
            private string[] Original = new string[0];

            private object[] OriginalObject = new object[0];

            public StringBuilder ChangeStringBuilder;

            public StringChange(int capacity)
            {
                ChangeStringBuilder = new StringBuilder(capacity);
            }

            public string Chcek(params string[] strings)
            {
                if (Original.SequenceEqual(strings))
                {
                    return ChangeStringBuilder.ToString();
                }
                else
                {
                    Original = strings;

                    return ChangeStringBuilder.StringConcat(Original);
                }
            }

            public string Chcek(params object[] strings)
            {
                if (OriginalObject.SequenceEqual(strings))
                {
                    return ChangeStringBuilder.ToString();
                }
                else
                {
                    OriginalObject = strings;

                    return ChangeStringBuilder.StringConcat(OriginalObject);
                }
            }
        }

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
