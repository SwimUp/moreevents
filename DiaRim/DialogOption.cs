﻿using DiaRim.Actions;
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

        public string Label;

        public bool ResolveTree = false;

        public bool Disabled = false;
        public string DisabledReason;
        public Color DisabledOptionColor = new Color(0.5f, 0.5f, 0.5f);

        public SoundDef ClickSound = SoundDefOf.PageChange;

        public Action ClickAction;

        public List<OptionAction> Actions;

        public List<OptionCondition> Conditions;

        public int NextPageId = -1;

        public Dialog Dialog => Page.Dialog;

        public void Init(DialogPage parent)
        {
            Page = parent;

            if (Actions != null)
            {
                foreach (var action in Actions)
                {
                    action.Option = this;
                }
            }

            Disabled = CheckConditions();
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
                /*
                if (DisabledReason != null)
                {
                    text = text + DisabledReason;
                }
                */
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
                    action.DoAction();
                }
            }

            ClickAction?.Invoke();

            if (ResolveTree)
            {
                Dialog.Close();
                return;
            }

            Dialog.GotoPage(NextPageId);
        }
    }
}
