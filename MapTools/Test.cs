using EmailMessages;
using MoreEvents.Communications;
using MoreEvents.Quests;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DiaRim
{
    public class TestDialogs : GameComponent
    {
        private static Dictionary<IIncidentTarget, StoryState> tmpOldStoryStates = new Dictionary<IIncidentTarget, StoryState>();

        public TestDialogs()
        {

        }

        public TestDialogs(Game game)
        {

        }

        public class TestOption : QuestOption
        {
            public override string Label => "TEST" + Rand.Range(3, 225);

            public override void DoAction(Quest quest, Pawn speaker, Pawn defendant)
            {
                Log.Message($"TRIGGER --> {quest.CardLabel}: {speaker.Name} : {defendant?.Name}");
            }
        }

        public class TestOption2 : CommOption
        {
            public static int I;

            public override string Label => "TEST" + I;

            public override string Description => "dddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "ddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "dddddddddddddddddddddddddddddddddddddddddddddddddddddddd" +
                "12341231231331313123";

            public override void DoAction(CommunicationDialog dialog, Pawn speaker, Pawn defendant)
            {
                Log.Message($"TRIGGER --> {dialog.CardLabel}: {speaker.Name} : {defendant?.Name}");
            }
        }

        public override void GameComponentTick()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                Find.WindowStack.Add(new TestDialogsWindow());
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                Find.WindowStack.Add(new TestTools());
            }
        }

        public class TestTools : Window
        {
            public override void DoWindowContents(Rect inRect)
            {
                Listing_Standard listing = new Listing_Standard();
                listing.Begin(inRect);
                if(listing.ButtonText("Try add GetHelp option"))
                {
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    foreach(var faction in QuestsManager.Communications.FactionManager.Factions)
                    {
                        list.Add(new FloatMenuOption(faction.Faction.Name, delegate
                        {
                            CommOption_GetHelp.AddComponentWithStack(faction.Faction, 1);
                        }));
                    }

                    Find.WindowStack.Add(new FloatMenu(list));
                }

                if (listing.ButtonText("Create test message"))
                {
                    EmailMessage message = new EmailMessage();

                    Find.WindowStack.Add(new CreateMessageWindow(message));
                }
                if (listing.ButtonText("End quest..."))
                {
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    foreach(EndCondition condition in Enum.GetValues(typeof(EndCondition)))
                    {
                        list.Add(new FloatMenuOption(condition.ToString(), delegate
                        {
                            List<FloatMenuOption> list2 = new List<FloatMenuOption>();
                            foreach (var quest in QuestsManager.Communications.Quests)
                            {
                                list2.Add(new FloatMenuOption(quest.CardLabel, delegate
                                {
                                    if (quest.Site != null)
                                        quest.Site.EndQuest(null, condition);
                                    else
                                        quest.EndQuest(null, condition);
                                }));
                            }

                            Find.WindowStack.Add(new FloatMenu(list2));
                        }));
                    }

                    Find.WindowStack.Add(new FloatMenu(list));
                }
                if (listing.ButtonText("Add scout component"))
                {
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    foreach (var faction in QuestsManager.Communications.FactionManager.Factions)
                    {
                        list.Add(new FloatMenuOption(faction.Faction.Name, delegate
                        {
                            if (!ScoutingComp.ScoutAlready(faction.Faction, out ScoutingComp outComp))
                            {
                                ScoutingComp comp = new ScoutingComp(faction.Faction, 4000, 200000, 5);
                                comp.id = QuestsManager.Communications.UniqueIdManager.GetNextComponentID();

                                QuestsManager.Communications.RegisterComponent(comp);
                            }
                            else
                            {
                                Messages.Message("Already has", MessageTypeDefOf.PositiveEvent, false);
                            }
                        }));
                    }

                    Find.WindowStack.Add(new FloatMenu(list));
                }

                if (listing.ButtonText("Send email message"))
                {
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    foreach (var def in DefDatabase<EmailMessageDef>.AllDefs)
                    {
                        list.Add(new FloatMenuOption(def.defName, delegate
                        {
                            Find.World.GetComponent<MessageSender>().TrySendMessage(def);
                        }));
                    }

                    Find.WindowStack.Add(new FloatMenu(list));
                }

                if (listing.ButtonText("Add quest pawn"))
                {
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    foreach (var pawn in Find.CurrentMap.mapPawns.AllPawnsSpawned)
                    {
                        if (pawn.Name != null)
                        {
                            list.Add(new FloatMenuOption(pawn.Name.ToStringFull, delegate
                            {
                                List<FloatMenuOption> list2 = new List<FloatMenuOption>();
                                list2.Add(new FloatMenuOption("Missing people", delegate
                                {
                                    Quest_MissingPeople quest = new Quest_MissingPeople(Rand.Range(3, 9), 5, 5);
                                    quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
                                    quest.GenerateRewards(quest.GetQuestThingFilter(), new FloatRange(1000, 6000), new IntRange(4, 25), null, null);
                                    quest.Options = new List<QuestOption>();
                                    quest.Options.Add(new TestOption());
                                    quest.Options.Add(new TestOption());
                                    quest.Options.Add(new TestOption());
                                    quest.Options.Add(new TestOption());
                                    quest.Options.Add(new TestOption());
                                    quest.Options.Add(new TestOption());
                                    quest.Faction = Find.FactionManager.RandomNonHostileFaction();
                                    quest.ShowInConsole = false;
                                    QuestsManager.Communications.AddQuestPawn(pawn, quest);
                                    QuestsManager.Communications.AddQuest(quest);
                                }));
                                list2.Add(new FloatMenuOption("Building new base", delegate
                                {
                                    Quest_BuildNewBase quest = new Quest_BuildNewBase();
                                    quest.id = QuestsManager.Communications.UniqueIdManager.GetNextQuestID();
                                    quest.Faction = Find.FactionManager.RandomNonHostileFaction();
                                    quest.Options = new List<QuestOption>();
                                    quest.Options.Add(new TestOption());
                                    quest.Options.Add(new TestOption());
                                    quest.Options.Add(new TestOption());
                                    quest.GenerateRewards(quest.GetQuestThingFilter(), new FloatRange(1000, 6000), new IntRange(4, 25), null, null);
                                    quest.ShowInConsole = false;
                                    QuestsManager.Communications.AddQuestPawn(pawn, quest);
                                    QuestsManager.Communications.AddQuest(quest);
                                }));

                                Find.WindowStack.Add(new FloatMenu(list2));
                            }));
                        }
                    }

                    Find.WindowStack.Add(new FloatMenu(list));
                }

                if (listing.ButtonText("Add pawn dialog"))
                {
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    foreach (var pawn in Find.CurrentMap.mapPawns.AllPawnsSpawned)
                    {
                        if (pawn.Name != null)
                        {
                            list.Add(new FloatMenuOption(pawn.Name.ToStringFull, delegate
                            {
                                List<FloatMenuOption> list2 = new List<FloatMenuOption>();
                                list2.Add(new FloatMenuOption("Диалог для вида 1", delegate
                                {
                                    CommunicationDialog dialog = new CommunicationDialog();
                                    dialog.CardLabel = "Диалог для вида 1";
                                    dialog.Description = "errrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr" +
                                    "errrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr" +
                                    "errrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr" +
                                    "errrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr" +
                                    "errrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr" +
                                    "errrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr" +
                                    "errrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrerrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrerrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr" +
                                    "errrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr" +
                                    "errrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrerrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr" +
                                    "errrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrerrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr" +
                                    "errrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrerrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr" +
                                    "errrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr" +
                                    "12314rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr" +
                                    "rt343y6yh35y55555555555555555555555555555";
                                    dialog.Options = new List<CommOption>();
                                    for (int i = 0; i < 8; i++)
                                    {
                                        dialog.Options.Add(new TestOption2());
                                        TestOption2.I++;
                                    }
                                    dialog.id = QuestsManager.Communications.UniqueIdManager.GetNextDialogID();
                                    QuestsManager.Communications.AddCommunication(dialog);
                                    QuestsManager.Communications.AddQuestPawn(pawn, dialog);
                                }));
                                list2.Add(new FloatMenuOption("Диалог для вида 2", delegate
                                {
                                    CommunicationDialog dialog = new CommunicationDialog();
                                    dialog.Options = new List<CommOption>();
                                    dialog.CardLabel = "Диалог для вида 2";
                                    for (int i = 0; i < 4; i++)
                                    {
                                        dialog.Options.Add(new TestOption2());
                                        TestOption2.I++;
                                    }
                                    dialog.id = QuestsManager.Communications.UniqueIdManager.GetNextDialogID();
                                    QuestsManager.Communications.AddCommunication(dialog);
                                    QuestsManager.Communications.AddQuestPawn(pawn, dialog);
                                }));

                                Find.WindowStack.Add(new FloatMenu(list2));
                            }));
                        }
                    }

                    Find.WindowStack.Add(new FloatMenu(list));
                }

                if (listing.ButtonText("Add dialog timer 30k"))
                {
                    CommunicationDialog dialog = CommunicationDialogMaker.MakeCommunicationDialog("test", "sasasasasasasasas", Find.FactionManager.RandomNonHostileFaction(),
                        DefDatabase<IncidentDef>.GetRandom(), null);

                    CommunicationComponent_CommunicationDialogTimer timer = new CommunicationComponent_CommunicationDialogTimer(30000,
                        dialog);

                    QuestsManager.Communications.RegisterComponent(timer);

                    Messages.Message("added", MessageTypeDefOf.NeutralEvent, false);
                }

                if (listing.ButtonText("Spawn 100k silver"))
                {
                    DebugTools.curTool = new DebugTool("SUPA SPAWN ZULUL", delegate
                    {
                        Thing t = ThingMaker.MakeThing(ThingDefOf.Silver);
                        t.stackCount = 1000000;

                        GenDrop.TryDropSpawn(t, UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near, out Thing resultThing);
                    });
                }

                listing.End();
            }

        }

        private string GetIncidentTargetLabel(IIncidentTarget target)
        {
            if (target == null)
            {
                return "null target";
            }
            if (target is Map)
            {
                return "Map";
            }
            if (target is World)
            {
                return "World";
            }
            if (target is Caravan)
            {
                return ((Caravan)target).LabelCap;
            }
            return target.ToString();
        }

        public class CreateMessageWindow : Window
        {
            private EmailMessage message;

            public CreateMessageWindow(EmailMessage message)
            {
                this.message = message;
            }

            public override void DoWindowContents(Rect inRect)
            {
                Listing_Standard listing = new Listing_Standard();
                listing.Begin(inRect);
                listing.Label("from");
                message.From = listing.TextEntry(message.From);

                listing.Label("to");
                message.To = listing.TextEntry(message.To);

                listing.Label("subject");
                message.Subject = listing.TextEntry(message.Subject);

                listing.Label("Message");
                message.Message = listing.TextEntry(message.Message);

                if(listing.ButtonText("send"))
                {
                    for(int i = 0; i < Rand.Range(1, 4); i++)
                        message.Answers.Add(new MessageAnwer_OpenDialog("Тест ответ 123", null));

                    DebugGetFutureIncidents(15, true, out Dictionary<IIncidentTarget, int> incCountsForTarget, out int[] incCountsForComp, out List<Pair<IncidentDef, IncidentParms>> allIncidents, out int threatBigCount, out List<float> daysToEvents);
                    message.Message += $"Событий на 15 дней: {allIncidents.Count}";
                    for(int i = 0; i < allIncidents.Count; i++)
                    {
                        Pair<IncidentDef, IncidentParms> pair = allIncidents[i];
                        message.Message += $"\n{pair.First.LabelCap} --> Сложность: {pair.Second.points} --> Через: {daysToEvents[i].ToString("f2")}";
                    }

                    QuestsManager.Communications.PlayerBox.SendMessage(message);
                }

                listing.End();
            }
        }

        public static void DebugGetFutureIncidents(int numTestDays, bool currentMapOnly, out Dictionary<IIncidentTarget, int> incCountsForTarget, out int[] incCountsForComp, out List<Pair<IncidentDef, IncidentParms>> allIncidents, out int threatBigCount, out List<float> daysToEvents, StringBuilder outputSb = null, StorytellerComp onlyThisComp = null)
        {
            int ticksGame = Find.TickManager.TicksGame;
            daysToEvents = new List<float>();
            IncidentQueue incidentQueue = Find.Storyteller.incidentQueue;
            List<IIncidentTarget> allIncidentTargets = Find.Storyteller.AllIncidentTargets;
            tmpOldStoryStates.Clear();
            for (int i = 0; i < allIncidentTargets.Count; i++)
            {
                IIncidentTarget incidentTarget = allIncidentTargets[i];
                tmpOldStoryStates.Add(incidentTarget, incidentTarget.StoryState);
                new StoryState(incidentTarget).CopyTo(incidentTarget.StoryState);
            }
            Find.Storyteller.incidentQueue = new IncidentQueue();
            int num = numTestDays * 60;
            incCountsForComp = new int[Find.Storyteller.storytellerComps.Count];
            incCountsForTarget = new Dictionary<IIncidentTarget, int>();
            allIncidents = new List<Pair<IncidentDef, IncidentParms>>();
            threatBigCount = 0;
            for (int j = 0; j < num; j++)
            {
                IEnumerable<FiringIncident> enumerable = (onlyThisComp == null) ? Find.Storyteller.MakeIncidentsForInterval() : Find.Storyteller.MakeIncidentsForInterval(onlyThisComp, Find.Storyteller.AllIncidentTargets);
                foreach (FiringIncident item in enumerable)
                {
                    if (item == null)
                    {
                        Log.Error("Null incident generated.");
                    }
                    if (!currentMapOnly || item.parms.target == Find.CurrentMap)
                    {
                        item.parms.target.StoryState.Notify_IncidentFired(item);
                        allIncidents.Add(new Pair<IncidentDef, IncidentParms>(item.def, item.parms));
                        item.parms.target.StoryState.Notify_IncidentFired(item);
                        if (!incCountsForTarget.ContainsKey(item.parms.target))
                        {
                            incCountsForTarget[item.parms.target] = 0;
                        }
                        Dictionary<IIncidentTarget, int> dictionary;
                        IIncidentTarget target;
                        (dictionary = incCountsForTarget)[target = item.parms.target] = dictionary[target] + 1;
                        string text = "  ";
                        if (item.def.category == IncidentCategoryDefOf.ThreatBig || item.def.category == IncidentCategoryDefOf.RaidBeacon)
                        {
                            threatBigCount++;
                            text = "T";
                        }
                        else if (item.def.category == IncidentCategoryDefOf.ThreatSmall)
                        {
                            text = "S";
                        }
                        int num2 = Find.Storyteller.storytellerComps.IndexOf(item.source);
                        incCountsForComp[num2]++;
                        daysToEvents.Add(Find.TickManager.TicksGame.TicksToDays());
                        //outputSb?.AppendLine(Find.TickManager.TicksGame.TicksToDays().ToString("F1") + "days");
                    }
                }
                Find.TickManager.DebugSetTicksGame(Find.TickManager.TicksGame + 1000);
            }
            Find.TickManager.DebugSetTicksGame(ticksGame);
            Find.Storyteller.incidentQueue = incidentQueue;
            for (int k = 0; k < allIncidentTargets.Count; k++)
            {
                tmpOldStoryStates[allIncidentTargets[k]].CopyTo(allIncidentTargets[k].StoryState);
            }
            tmpOldStoryStates.Clear();
        }

        public class TestDialogsWindow : Window
        {
            private Vector2 scrollPosition = Vector2.zero;
            public override Vector2 InitialSize => new Vector2(200, 530);

            private int defSize = 0;

            public string X = "500";
            public string Y = "500";

            public TestDialogsWindow()
            {
                resizeable = false;

                defSize = DefDatabase<DialogDef>.AllDefsListForReading.Count;
            }

            public override void DoWindowContents(Rect inRect)
            {
                int size = defSize * 25;

                Text.Font = GameFont.Small;

                int y = 0;
                Rect scrollRectFact = new Rect(0, 0, 190, 390);
                Rect scrollVertRectFact = new Rect(0, 0, scrollRectFact.x, size);
                Widgets.BeginScrollView(scrollRectFact, ref scrollPosition, scrollVertRectFact);

                foreach (var def in DefDatabase<DialogDef>.AllDefs)
                {
                    if (Widgets.ButtonText(new Rect(0, y, 170, 20), def.defName))
                    {
                        Dialog dia = new Dialog(def, Current.Game.CurrentMap.mapPawns.FreeColonists.RandomElement());
                        dia.Init();
                        Find.WindowStack.Add(dia);
                    }
                    y += 22;
                }

                Widgets.EndScrollView();

                X = Widgets.TextField(new Rect(0, 420, 170, 20), X);
                Y = Widgets.TextField(new Rect(0, 450, 170, 20), Y);
            }
        }
    }
}
