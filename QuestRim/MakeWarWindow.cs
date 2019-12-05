using DiaRim;
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

        private DialogDef dialog => DialogDefOfLocal.PeaceTalk;

        private FactionInteraction interactFaction;

        private Pawn speaker;

        private Pawn defendant;

        private bool defendMode;

        private WarGoalDef warGoalDef;

        private string editBufferName;

        private War war;

        private string stat;

        public int BlockTime => 5 * 60000;

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

            if(defendMode)
            {
                war = faction.InWars.First(x => x.DeclaredWarFaction.Faction == speaker.Faction);
                stat = war.StatWorker.GetStat();
            }

            warGoalDef = DefDatabase<WarGoalDef>.AllDefs.First();
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (defendMode)
            {
                DrawDefendMode(inRect);
            }
            else
            {
                DrawWarMode(inRect);
            }
        }

        private void DrawDefendMode(Rect inRect)
        {
            Text.Anchor = TextAnchor.MiddleCenter;

            float y = inRect.y;

            Rect titleRect = new Rect(inRect.x, y, inRect.width, 25);
            Widgets.Label(titleRect, "MakeWarWindow_DrawDefendModeTitle".Translate(war.WarName, interactFaction.Faction.Name));

            y += 30;

            Text.Anchor = TextAnchor.UpperLeft;

            float height = inRect.height - y - 35;
            Rect fullInfoRect = new Rect(inRect.x, y, inRect.width, height);
            Widgets.Label(fullInfoRect, "MakeWarWindow_DrawDefendModeDesc".Translate(war.WarGoalDef.LabelCap, stat));

            y += height;
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect buttonRect = new Rect(inRect.x, y, inRect.width, 25);
            bool canTruce = war.CanTruceRightNow();
            if(GUIUtils.DrawCustomButton(buttonRect, "MakeWarWindow_DrawDefendModeTruce".Translate(), canTruce ? Color.white : Color.gray))
            {
                if(canTruce)
                {
                    Dialog dia = new Dialog(dialog, speaker, defendant);
                    dia.Init();
                    dia.CloseAction = CheckAnswer;
                    Find.WindowStack.Add(dia);
                }
                else
                {
                    Messages.Message("MakeWarWindow_DrawDefendModeCantTruce".Translate(), MessageTypeDefOf.NeutralEvent);
                }
            }

            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void CheckAnswer(string answer)
        {
            if (answer == "удача")
            {
                Close();

                war.EndWar(Winner.Draw);
            }

            war.LastTruceTicks = Find.TickManager.TicksGame + BlockTime;
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
