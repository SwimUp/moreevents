using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class CommunicationDialogWindow : Window
    {
        public Pawn Speaker;
        public Pawn Defendant;
        public QuestPawn QuestPawn;
        public CommunicationDialog Dialog;

        public Vector2 scroll2 = Vector2.zero;
        public Vector2 scroll = Vector2.zero;

        public override Vector2 InitialSize => new Vector2(700, 700);
        protected override float Margin => 0f;

        private static readonly Color CommCardBGColor = new ColorInt(150, 150, 150).ToColor;
        private static readonly Color CommBorderColor = new ColorInt(120, 120, 120).ToColor;

        public CommunicationDialogWindow(CommunicationDialog dialog, Pawn speaker, Pawn defendant)
        {
            forcePause = true;

            Speaker = speaker;
            Defendant = defendant;
            Dialog = dialog;

            defendant.GetQuestPawn(out QuestPawn);
        }
        public override void DoWindowContents(Rect inRect)
        {
            if(QuestPawn != null && !QuestPawn.Dialogs.Contains(Dialog))
            {
                Close(); 
            }

            GUI.color = Color.white;
            Text.Font = GameFont.Medium;
            Rect titleRect = inRect;
            titleRect.y = 10;
            titleRect.x = 10;
            titleRect.width = 690;
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(titleRect, Dialog.CardLabel);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect mainRect = new Rect(10, 40, 680, 280);
            Widgets.LabelScrollable(mainRect, Dialog.Description, ref scroll);

            Text.Anchor = TextAnchor.MiddleCenter;
            int sliderLength = Dialog.Options.Count * 30;
            Rect optionsRect = new Rect(0, 0, 680, 25);
            Rect scrollRewVertRectFact = new Rect(0, 0, 700, sliderLength);
            Widgets.BeginScrollView(new Rect(10, 340, 690, 180), ref scroll2, scrollRewVertRectFact, false);
            for(int i = 0; i < Dialog.Options.Count; i++)
            {
                CommOption option = Dialog.Options[i];
                if(DrawCustomButton(optionsRect, $"{option.Label}", Color.white, option))
                {
                    option.DoAction(Dialog, Speaker, Defendant);
                }
                optionsRect.y += 30;
            }
            Widgets.EndScrollView();
            Text.Anchor = TextAnchor.UpperLeft;

            GUI.color = CommCardBGColor;
            Widgets.DrawLineHorizontal(0, 327, inRect.width);
            Widgets.DrawLineHorizontal(20, 37, 660);
            Widgets.DrawLineHorizontal(0, 530, inRect.width);
            Widgets.DrawLineVertical(340, 530, 170);
            GUI.color = Color.white;
            Rect pawnRect = new Rect(10, 540, 330, 150);
            DrawPawnCard(Speaker, pawnRect);
            pawnRect.x = 350;
            DrawPawnCard(Defendant, pawnRect);
        }

        private bool DrawCustomButton(Rect rect, string label, Color textColor, CommOption option)
        {
            GUI.color = textColor;
            Widgets.Label(rect, label);
            GUI.color = CommCardBGColor;
            Widgets.DrawHighlight(rect);
            GUI.color = CommBorderColor;
            Widgets.DrawBox(rect);
            GUI.color = Color.white;
            Widgets.DrawHighlightIfMouseover(rect);
            TooltipHandler.TipRegion(rect, option.Description);
            return Widgets.ButtonInvisible(rect);
        }

        public static void DrawPawnCard(Pawn pawn, Rect rect)
        {
            Text.Font = GameFont.Small;
            GUI.DrawTexture(new Rect(rect.x, rect.y, 100, 140), PortraitsCache.Get(pawn, new Vector2(100, 140)));
            Rect rect2 = rect;
            rect2.x += 100;
            rect2.width = 210;
            rect2.height = 30;
            Widgets.Label(rect2, pawn.Name.ToStringFull);
            rect2.y += 20;
            Widgets.DrawLineHorizontal(rect2.x, rect2.y, 210);

            Text.Font = GameFont.Tiny;
            Rect skillsRect = rect2;
            skillsRect.y += 10;
            skillsRect.width = 210;
            skillsRect.height = 24;
            GeoscapeWindow.DrawSkill(pawn.skills.GetSkill(SkillDefOf.Social), skillsRect);
            skillsRect.y += 30;
            GeoscapeWindow.DrawSkill(pawn.skills.GetSkill(SkillDefOf.Intellectual), skillsRect);
            skillsRect.y += 35;
            Widgets.Label(skillsRect, pawn.Faction.Name);
            if(pawn.Faction != Faction.OfPlayer)
            {
                skillsRect.y += 20;
                Widgets.Label(skillsRect, $"{pawn.Faction.PlayerRelationKind.GetLabel()}: {pawn.Faction.PlayerGoodwill.ToStringWithSign()}");
            }
        }
    }
}
