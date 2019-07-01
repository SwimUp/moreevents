using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace DiaRim
{
    public class DialogPage
    {
        public Dialog Dialog;

        public int UniqueId;

        [Unsaved]
        [TranslationHandle]
        public string unstranslatedId;

        [MustTranslate]
        public string Title;

        [MustTranslate]
        public string Text;

        [MustTranslate]
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

        public void PostLoad()
        {
            unstranslatedId = $"page{UniqueId}";
        }
    }
}
