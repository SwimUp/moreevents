using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DiaRim
{
    public class DialogOption
    {
        public DialogPage Page;

        public string Label;

        public int NextPage;

        public bool DialogEnd = false;

        [Unsaved]
        public Action Action;

        public Dialog Dialog => Page.Dialog;

        public void Init(DialogPage parent)
        {
            Page = parent;
        }
    }
}
