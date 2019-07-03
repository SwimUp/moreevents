using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MoreEvents.Events.AttackFriendlySettlement
{
    public class IncidentWorker_AttackFriendlySettlement : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["AttackFriendlySettlement"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Faction f = GetFriendlyFaction();
            if (f == null)
                return false;

            if (GetEnemyFaction(f) == null)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            Faction faction = GetFriendlyFaction();
            Faction faction2 = GetEnemyFaction(faction);
            Settlement factionBase = Find.WorldObjects.Settlements.Where(x => x.Faction == faction).RandomElement();

            if (factionBase == null)
                return false;

            FriendlySettlement site = (FriendlySettlement)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.FriendlySettlementHelp);
            site.Tile = factionBase.Tile;
            site.SetFaction(faction);
            var comp = site.GetComponent<FriendlySettlementComp>();
            comp.TicksToAttack = Rand.Range(8, 15) * 60000;
            comp.OffensiveFaction = faction2;
            comp.InitPoints();
            Find.WorldObjects.Add(site);

            string letterText;
            if(Rand.Chance(0.35f))
            {
                comp.ShowThreat = true;
                letterText = "AttackFriendlySettlementLetter2".Translate(faction.Name, comp.Points);
            }
            else
            {
                letterText = "AttackFriendlySettlementLetter1".Translate(faction.Name);
            }

            Find.LetterStack.ReceiveLetter(def.letterLabel, letterText, def.letterDef, site);

            return true;
        }

        private Faction GetEnemyFaction(Faction faction)
        {
            if ((from x in Find.FactionManager.AllFactions
                 where x != faction && !x.IsPlayer && (!x.def.hidden) && (!x.defeated) && (x.def.humanlikeFaction)  && x.RelationKindWith(faction) == FactionRelationKind.Hostile
                 select x).TryRandomElement(out Faction result))
            {
                return result;
            }
            return null;
        }

        private Faction GetFriendlyFaction()
        {
            Faction f = Find.FactionManager.RandomAlliedFaction();

            if (f == null)
                return null;

            return f;
        }
    }
}
