using QuestRim;
using RimOverhaul;
using RimOverhaul.Gss;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace DarkNET
{
    public class DarkNet : GameComponent
    {
        public List<DarkNetTrader> Traders;

        public static bool PlayerHasDarkNetConsole;

        private int nextChangeDangerousTicks;

        public static float BaseDangerous => 2;

        public float Dangerous => dangerous;

        private float dangerous;

        public static Faction GssFaction => Find.FactionManager.FirstFactionOfDef(MoreEvents.FactionDefOfLocal.GalacticSecurityService);

        private int lastRaidTicks = 0;

        public DarkNet()
        {

        }

        public DarkNet(Game game)
        {
            PlayerHasDarkNetConsole = false;
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();

            foreach (var map in Find.Maps)
            {
                if (map.IsPlayerHome)
                {
                    if (map.listerBuildings.allBuildingsColonist.Any(b => b is Building_DarkNetConsole))
                    {
                        PlayerHasDarkNetConsole = true;
                        break;
                    }
                }
            }
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();

            if (Traders == null)
            {
                InitDarkNet();
            }
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            if (Traders == null)
                return;

            if(Find.TickManager.TicksGame % 60000 == 0)
            {
                foreach(var trader in Traders)
                {
                    OnDayPassed();

                    trader.OnDayPassed();
                }
            }
        }

        public void OnDayPassed()
        {
            int passedDays = Find.TickManager.TicksGame - nextChangeDangerousTicks;
            if(passedDays >= 0)
            {
                ChangeDangerous();

                nextChangeDangerousTicks = (int)(Find.TickManager.TicksGame + (Rand.Range(0.4f, 4f) * Rand.Range(35000, 60000)));
            }
        }

        public void ChangeDangerous()
        {
            int onlineTraders = 0;
            if(Traders != null)
            {
                Traders.ForEach(x =>
                {
                    if (x.Online)
                    {
                        onlineTraders++;
                    }
                });
            }

            dangerous = Mathf.Round(BaseDangerous + (onlineTraders * 1.2f) + Rand.Range(1, 10));
        }

        public override void LoadedGame()
        {
            base.LoadedGame();

            if (Traders == null)
            {
                InitDarkNet();
            }
        }

        public void SendGssRaid(Map map, bool force = false)
        {
            float passedDays = (lastRaidTicks - Find.TickManager.TicksGame) / 60000f;

            if (force || passedDays <= 0)
            {
                lastRaidTicks = Find.TickManager.TicksGame + 60000;

                if (GssFaction.RelationKindWith(Faction.OfPlayer) != FactionRelationKind.Hostile)
                {
                    GssFaction.TrySetRelationKind(Faction.OfPlayer, FactionRelationKind.Hostile, true, "DarKNet_WhyAffect".Translate());
                }

                GssRaids.SendRaid(map, Mathf.Max(300, StorytellerUtility.DefaultThreatPointsNow(map)));
            }
        }

        public void InitDarkNet()
        {
            Traders = new List<DarkNetTrader>();

            foreach(var trader in DefDatabase<DarkNetTraderDef>.AllDefs)
            {
                DarkNetTrader newTrader = InitTrader(trader);

                Traders.Add(newTrader);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref Traders, "Traders", LookMode.Deep);

            Scribe_Values.Look(ref nextChangeDangerousTicks, "nextChangeDangerousTicks");
            Scribe_Values.Look(ref dangerous, "dangerous");
            Scribe_Values.Look(ref lastRaidTicks, "lastRaidTicks");
        }

        private DarkNetTrader InitTrader(DarkNetTraderDef def)
        {
            DarkNetTrader newTrder = (DarkNetTrader)Activator.CreateInstance(def.workerClass);

            newTrder.def = def;
            newTrder.FirstInit();

            return newTrder;
        }

        public static EmailMessage FormMessageFromDarkNet(string text, string subject, DarkNetTraderDef trader)
        {
            Faction darkNetFaction = Find.FactionManager.FirstFactionOfDef(FactionDefOfLocal.DarkNetTraders);
            EmailBox Owner = QuestsManager.Communications.PlayerBox;

            EmailMessage message = new EmailMessage();
            message.Faction = darkNetFaction;
            message.To = Owner.Owner.Name;
            message.From = $"{darkNetFaction?.Name} ({trader.LabelCap})";
            message.Subject = subject;
            message.Message = text;
            message.SendTick = Find.TickManager.TicksGame;
            message.MessageRead = false;

            return message;
        }
    }
}
