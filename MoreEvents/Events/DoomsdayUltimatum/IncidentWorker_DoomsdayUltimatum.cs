using MoreEvents.Communications;
using QuestRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MoreEvents.Events.DoomsdayUltimatum
{
    public class IncidentWorker_DoomsdayUltimatum : IncidentWorker
    {
        private EventSettings settings => Settings.EventsSettings["DoomsdayUltimatum"];

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            if (GetEnemyFaction() == null)
                return false;

            if (GetPlace() == -1)
                return false;

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!settings.Active)
                return false;

            int spawnPoint = GetPlace();
            DoomsdaySite site = (DoomsdaySite)WorldObjectMaker.MakeWorldObject(WorldObjectsDefOfLocal.DoomsdayUltimatumCamp);
            site.Tile = spawnPoint;
            site.SetFaction(GetEnemyFaction());
            var comp = site.GetComponent<DoomsdayUltimatumComp>();
            comp.SetTimer(Rand.Range(10, 20));
            site.comp = comp;
            List<Faction> factions = Find.FactionManager.AllFactionsListForReading.Where(f => !f.IsPlayer && !f.def.hidden && f != site.Faction && f.RelationKindWith(site.Faction) == FactionRelationKind.Hostile).ToList();
            comp.FactionSilver = Mathf.Clamp(factions.Count * Rand.Range(2000, 4000), 15000, 35000);

            QuestsManager.Communications.AddCommunication("DoomsdayCardLabel".Translate(), "DoomsdayDesc".Translate(site.Faction.Name), site.Faction, def, new List<CommOption> {
                new CommOption()
                {
                    Label = "DiscussWithOtherFactions".Translate(),
                    Actions = new List<CommAction>
                    {
                        new CommAction_DoomsdayDialog(site, site.Faction)
                    }
                }
            });

            Find.WorldObjects.Add(site);

            SendStandardLetter(site);

            DoomsdaySite.ActiveSite = site;

            return true;
        }

        private Faction GetEnemyFaction()
        {
            Faction f = Find.FactionManager.RandomEnemyFaction();

            if (f == null)
                return null;

            return f;
        }

        private int GetPlace()
        {
            int result = -1;
            if (TileFinder.TryFindNewSiteTile(out result, 14, 24))
            {
                return result;
            }
            return -1;
        }
    }
}
