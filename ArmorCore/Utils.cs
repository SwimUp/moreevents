using RimArmorCore.Mk1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimArmorCore
{
    public static class Utils
    {
        public static string GetLabel(this ArmorModuleCategory category)
        {
            switch(category)
            {
                case ArmorModuleCategory.Body:
                    return "ArmorModuleCategory_Body".Translate();
                case ArmorModuleCategory.General:
                    return "ArmorModuleCategory_General".Translate();
                case ArmorModuleCategory.Head:
                    return "ArmorModuleCategory_Head".Translate();
                case ArmorModuleCategory.Legs:
                    return "ArmorModuleCategory_Legs".Translate();
            }

            return "";
        }

        public static string[] ToArrayValues<T,D>(this Dictionary<T, D> dictionary)
        {
            string[] result = new string[dictionary.Count];

            for(int i = 0; i < dictionary.Count; i++)
            {
                D value = dictionary.ElementAt(i).Value;
                result[i] = value.ToString();
            }

            return result;
        }
    }
}
