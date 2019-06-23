using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DiaRim
{
    public class DialogDef : Def
    {
        public int FirstPageId;

        public List<DialogPage> Pages;

        public List<string> CustomParams;

        public Vector2 WindowSize = new Vector2(500, 500);
    }
}
