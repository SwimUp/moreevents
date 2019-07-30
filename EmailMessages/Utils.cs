using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace EmailMessages
{
    public static class Utils
    {
        public static bool InRange(this IntRange range, int value)
        {
            if(range.max >= value && range.min <= value)
                return true;

            return false;
        }
    }
}
