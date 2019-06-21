using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiaRim
{
    public class DialogPage
    {
        public Dialog Dialog;

        public int UniqueId;

        public string Title;

        public string Text;

        public string PageText;

        public List<DialogOption> Options;

        public void Init(Dialog dialog)
        {
            Dialog = dialog;

            foreach(var option in Options)
            {
                option.Init(this);
            }

            PageText = $"{Title}\n\n{Text}";
        }
    }
}
