using DiaRim.Actions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace DiaRim
{
    public class DialogOption
    {
        public DialogPage Page;

        public string Label;

        public bool ResolveTree = false;

        public bool Disabled = false;
        public string DisabledReason;
        public Color DisabledOptionColor = new Color(0.5f, 0.5f, 0.5f);

        public SoundDef ClickSound = SoundDefOf.PageChange;

        public List<OptionAction> Actions;

        public Dialog Dialog => Page.Dialog;

        public void Init(DialogPage parent)
        {
            Page = parent;

            foreach(var action in Actions)
            {
                action.Option = this;
            }
        }

        public float OptOnGUI(Rect rect, bool active = true)
        {
            Color textColor = Widgets.NormalOptionColor;
            string text = Label;
            if (Disabled)
            {
                textColor = DisabledOptionColor;
                if (DisabledReason != null)
                {
                    text = text + " (" + DisabledReason + ")";
                }
            }
            rect.height = Text.CalcHeight(text, rect.width);
            if (Widgets.ButtonText(rect, text, drawBackground: false, doMouseoverSound: false, textColor, active && !Disabled))
            {
                Activate();
            }
            return rect.height;
        }

        protected void Activate()
        {
            if (ClickSound != null && !ResolveTree)
            {
                ClickSound.PlayOneShotOnCamera();
            }
            if (ResolveTree)
            {
                Dialog.Close();
            }

            foreach(var action in Actions)
            {
                action.DoAction();
            }
        }
    }
}
