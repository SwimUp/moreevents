using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace QuestRim
{
    public class MakeWarWindow : Window
    {
        public override Vector2 InitialSize => new Vector2(800, 600);

        private FactionInteraction interactFaction;

        private Pawn speaker;

        private Pawn defendant;

        private bool defendMode;

        private WarGoalDef warGoalDef;

        private string editBufferName;

        public MakeWarWindow()
        {

        }

        public MakeWarWindow(FactionInteraction faction, Pawn speaker, Pawn defendant)
        {
            doCloseX = true;
            forcePause = true;

            interactFaction = faction;

            this.speaker = speaker;
            this.defendant = defendant;

            defendMode = faction.InWars.Any(x => x.DefendingFactions.Contains(faction) && x.DeclaredWarFaction.Faction == speaker.Faction);

            warGoalDef = DefDatabase<WarGoalDef>.AllDefs.First();
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (defendMode)
            {

            }
            else
            {
                DrawWarMode(inRect);
            }
        }

        private void DrawWarMode(Rect inRect)
        {
            Text.Anchor = TextAnchor.MiddleCenter;

            float y = inRect.y;
            Rect titleRect = new Rect(inRect.x, y, inRect.width, 25);
            Widgets.Label(titleRect, "MakeWarWindow_DrawWarModeTitle".Translate(interactFaction.Faction.Name));

            y += 30;
            Rect selectWarGoalRect = new Rect(inRect.x, y, inRect.width, 25);
            if (GUIUtils.DrawCustomButton(selectWarGoalRect, "MakeWarWindow_DrawWarMode_SelectWarGoal".Translate(warGoalDef.LabelCap), Color.white))
            {
                List<FloatMenuOption> options = new List<FloatMenuOption>();
                foreach(var goal in DefDatabase<WarGoalDef>.AllDefs)
                {
                    options.Add(new FloatMenuOption(goal.LabelCap, delegate
                    {
                        warGoalDef = goal;
                    }));
                }

                Find.WindowStack.Add(new FloatMenu(options));
            }

            y += 28;

            Text.Anchor = TextAnchor.UpperLeft;

            float height = inRect.height - y - 80;
            Rect fullInfoRect = new Rect(inRect.x, y, inRect.width, height);
            Widgets.Label(fullInfoRect, warGoalDef.description);

            y += height;

            Text.Anchor = TextAnchor.MiddleCenter;
            Rect warNameRect = new Rect(inRect.x, y, inRect.width, 25);
            Widgets.Label(warNameRect, "MakeWarWindow_WarNameLabel".Translate());
            y += 22;
            warNameRect.y += 22;
            editBufferName = Widgets.TextField(warNameRect, editBufferName);

            y += 30;

            Rect createWarButtonRect = new Rect(inRect.x, y, inRect.width, 25);
            if (GUIUtils.DrawCustomButton(createWarButtonRect, "MakeWarWindow_DrawWarMode_MakeWar".Translate(interactFaction.Faction.Name), Color.white))
            {
                if (string.IsNullOrEmpty(editBufferName))
                {
                    Messages.Message("MakeWarWindow_EmptyName".Translate(), MessageTypeDefOf.NeutralEvent);
                }
                else
                {
                    War war = WarMaker.MakeWar(editBufferName, warGoalDef, QuestsManager.Communications.FactionManager.GetInteraction(speaker.Faction), interactFaction);

                    QuestsManager.Communications.FactionManager.AddWar(war);

                    war.StartWar();

                    Close();
                }
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }
    }
}
