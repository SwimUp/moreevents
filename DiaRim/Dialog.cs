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

        public Pawn Speaker;
        public Pawn Defendant;

        public Color screenFillColor = Color.clear;

        public Dictionary<int, DialogPage> Pages = new Dictionary<int, DialogPage>();

        public DialogPage CurrentPage;

        protected float minOptionsAreaHeight;
        private Vector2 scrollPosition;
        private float optTotalHeight;

        public Action<string> CloseAction;

        public Dictionary<string, int> CustomParams = new Dictionary<string, int>();

        public Vector2 Size = new Vector2(500, 500);
        public override Vector2 InitialSize => Size;

        public Dialog(DialogDef dialogDef, Pawn speaker)
        {
            DialogDef = dialogDef;
            Speaker = speaker;
            Size = DialogDef.WindowSize;
        }

        public Dialog(DialogDef dialogDef, Pawn speaker, Vector2 size)
        {
            DialogDef = dialogDef;
            Speaker = speaker;
            Size = size;
        }

        public Dialog(DialogDef dialogDef, Pawn speaker, Pawn defendant, Vector2 size)
        {
            DialogDef = dialogDef;
            Speaker = speaker;
            Size = size;
            Defendant = defendant;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Rect rect = inRect.AtZero();
            DrawNode(rect);
        }

        public void Close(string signal, bool doCloseSound = true)
        {
            CloseAction?.Invoke(signal);

            Close(doCloseSound);
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

        public override void WindowOnGUI()
        {
            if (screenFillColor != Color.clear)
            {
                GUI.color = screenFillColor;
                GUI.DrawTexture(new Rect(0f, 0f, UI.screenWidth, UI.screenHeight), BaseContent.WhiteTex);
                GUI.color = Color.white;
            }
            base.WindowOnGUI();
        }

        public void Init()
        {
            InitPages();

            CurrentPage = Pages[DialogDef.FirstPageId];

            if(DialogDef.CustomParams != null)
                InitParams();
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

        private void InitParams()
        {
            foreach(var param in DialogDef.CustomParams)
            {
                CustomParams.Add(param, 0);
            }
        }
    }
}
