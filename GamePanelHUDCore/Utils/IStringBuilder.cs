using System.Text;
using System.Linq;
using TMPro;

namespace GamePanelHUDCore.Utils
{
    public class IStringBuilder
    {
        private string[] OldStrings = new string[0];

        private readonly StringBuilder StringBuilder;

        public string Cache { get; private set; }

        public IStringBuilder()
        {
            StringBuilder = new StringBuilder();
        }

        public IStringBuilder(int capacity)
        {
            StringBuilder = new StringBuilder(capacity);
        }

        public string Concat(params string[] texts)
        {
            if (!OldStrings.SequenceEqual(texts))
            {
                StringBuilder.Clear();

                foreach (string text in texts)
                {
                    StringBuilder.Append(text);
                }

                OldStrings = texts;

                Cache = StringBuilder.ToString();

                return Cache;
            }
            else
            {
                return Cache;
            }
        }
    }
}
