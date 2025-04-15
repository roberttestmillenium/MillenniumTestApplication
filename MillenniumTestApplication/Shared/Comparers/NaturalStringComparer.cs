using System.Text.RegularExpressions;

namespace MillenniumTestApplication.Shared.Comparers
{
    public partial class NaturalStringComparer : IComparer<string>
    {
        [GeneratedRegex(@"\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
        private static partial Regex NumberRegex();

        public int Compare(string? a, string? b)
        {
            if (a == null && b == null) return 0;
            if (a == null) return -1;
            if (b == null) return 1;

            var regex = NumberRegex();
            var aParts = regex.Split(a);
            var bParts = regex.Split(b);
            var aNums = regex.Matches(a);
            var bNums = regex.Matches(b);

            int i = 0;
            while (i < aParts.Length && i < bParts.Length)
            {
                var result = string.Compare(aParts[i], bParts[i], StringComparison.OrdinalIgnoreCase);
                if (result != 0) return result;

                if (i < aNums.Count && i < bNums.Count)
                {
                    var aNum = int.Parse(aNums[i].Value);
                    var bNum = int.Parse(bNums[i].Value);
                    if (aNum != bNum) return aNum.CompareTo(bNum);
                }

                i++;
            }

            return aParts.Length.CompareTo(bParts.Length);
        }
    }
}
