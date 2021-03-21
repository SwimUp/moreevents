using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Sound;

namespace RimOverhaul
{
    class Sounds
    {
        static void Prefix(ref SoundDef soundDef, ref SoundInfo soundInfo)
        {
            try
            {
                Gender gender = info.Maker.Cell.GetFirstPawn(info.Maker.Map).gender;


            }
            catch { }
        }
    }
}
