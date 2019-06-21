using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DiaRim
{
    public class Dialog : Window
    {
        public DialogDef DialogDef;

        public Dictionary<int, DialogPage> Pages = new Dictionary<int, DialogPage>();

        public DialogPage CurrentPage;

        protected float minOptionsAreaHeight;
        private Vector2 scrollPosition;
        private float optTotalHeight;

        public Dialog(DialogDef dialogDef)
        {
            DialogDef = dialogDef;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect rect = inRect.AtZero();
            DrawNode(rect);
        }

        protected void DrawNode(Rect rect)
        {
            GUI.BeginGroup(rect);
            Text.Font = GameFont.Small;
            Rect outRect = new Rect(0f, 0f, rect.width, rect.height - Mathf.Max(optTotalHeight, minOptionsAreaHeight));
            float width = rect.width - 16f;
            Rect rect2 = new Rect(0f, 0f, width, Text.CalcHeight(CurrentPage.PageText, width));
            Widgets.BeginScrollView(outRect, ref scrollPosition, rect2);
            Widgets.Label(rect2, CurrentPage.PageText);
            Widgets.EndScrollView();
            float num = rect.height - optTotalHeight;
            float num2 = 0f;
            for (int i = 0; i < CurrentPage.Options.Count; i++)
            {
                Rect rect3 = new Rect(15f, num, rect.width - 30f, 999f);
                float num3 = CurrentPage.Options[i].OptOnGUI(rect3);
                num += num3 + 7f;
                num2 += num3 + 7f;
            }
            if (Event.current.type == EventType.Layout)
            {
                optTotalHeight = num2;
            }
            GUI.EndGroup();
        }

        public void Init()
        {
            InitPages();

            CurrentPage = Pages[DialogDef.FirstPageId];
        }

        public void GotoPage(DialogPage page)
        {
            CurrentPage = page;
        }

        public void GotoPage(int pageId)
        {
            CurrentPage = Pages[pageId];
        }

        private void InitPages()
        {
            foreach (var page in DialogDef.Pages)
            {
                page.Init(this);
                Pages.Add(page.UniqueId, page);
            }
        }
    }
}
