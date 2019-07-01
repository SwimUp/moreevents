using DiaRim.Actions;
using DiaRim.Conditions;
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

        [MustTranslate]
        public string Label;

        [Unsaved]
        [TranslationHandle]
        public string unstranslatedId;

        public bool ResolveTree = false;
        public string ResolveSignal;

        public bool Disabled = false;
        public string DisabledReason;
        public Color DisabledOptionColor = new Color(0.5f, 0.5f, 0.5f);

        public SoundDef ClickSound = SoundDefOf.PageChange;

        public Action ClickAction;

        public List<OptionAction> Actions;

        public List<OptionCondition> Conditions;

        public Dictionary<int, float> Transitions;

        public Dialog Dialog => Page.Dialog;

        public void Init(DialogPage parent)
        {
            Page = parent;

            Disabled = CheckConditions();
        }

        public void PostLoad()
        {
            unstranslatedId = Label;
        }

        public bool CheckConditions()
        {
            if (Conditions == null || Dialog.Speaker == null)
                return false;

            bool disable = false;
            StringBuilder builder = new StringBuilder();
            foreach (var condition in Conditions)
            {
                if (!condition.CanUse(Dialog.Speaker))
                {
                    disable = true;
                    builder.AppendLine($"- {condition.DisableReason}");
                }
            }

            if (builder.Length > 0)
            {
                DisabledReason = builder.ToString();
            }

            return disable;
        }

        public float OptOnGUI(Rect rect, bool active = true)
        {
            Color textColor = Widgets.NormalOptionColor;
            string text = Label;
            if (Disabled)
            {
                textColor = DisabledOptionColor;
            }
            rect.height = Text.CalcHeight(text, rect.width);
            TooltipHandler.TipRegion(rect, DisabledReason);
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

            if (Actions != null)
            {
                foreach (var action in Actions)
                {
                    action.DoAction(this);
                }
            }

            ClickAction?.Invoke();

            if (ResolveTree)
            {
                Dialog.Close(ResolveSignal);
                return;
            }

            float value = Rand.Value;
            foreach (var pair in Transitions.OrderBy(pair => pair.Value))
            {
                if (value < pair.Value)
                {
                    Dialog.GotoPage(pair.Key);
                    break;
                }
            }
        }
    }
}
