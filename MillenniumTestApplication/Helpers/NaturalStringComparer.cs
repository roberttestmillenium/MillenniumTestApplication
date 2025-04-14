using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MillenniumTestApplication.Helpers
{
    public class NaturalStringComparer : IComparer<string>
    {
        public int Compare(string a, string b)
        {
            if (a == b)
                return 0;
            if (a == null)
                return -1;
            if (b == null)
                return 1;

            var regex = new Regex(@"\d+|\D+");
            var aParts = regex.Matches(a);
            var bParts = regex.Matches(b);
            int count = Math.Min(aParts.Count, bParts.Count);
            for (int i = 0; i < count; i++)
            {
                string aPart = aParts[i].Value;
                string bPart = bParts[i].Value;
                int result;
                if (int.TryParse(aPart, out int aNum) && int.TryParse(bPart, out int bNum))
                {
                    result = aNum.CompareTo(bNum);
                }
                else
                {
                    result = string.Compare(aPart, bPart, StringComparison.OrdinalIgnoreCase);
                }
                if (result != 0)
                    return result;
            }
            return aParts.Count.CompareTo(bParts.Count);
        }
    }
}
