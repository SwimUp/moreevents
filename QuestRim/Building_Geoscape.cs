﻿using DiaRim;
using QuestRim.Actions;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace QuestRim
{
    public class Building_Geoscape : Building
    {
        public CompPowerTrader power;

        public static bool PlayerHasGeoscape = false;

        public bool HasPower
        {
            get
            {
                if (power != null && power.PowerOn)
                {
                    return !this.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare);
                }
                return false;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            power = GetComp<CompPowerTrader>();
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            yield return new FloatMenuOption(HasPower ? "OpenCommsGeo".Translate().ToString() : "OpenCommsGeoNoPower".Translate().ToString(), delegate
            {
                if (HasPower)
                {
                    if (selPawn != null)
                    {
                        Job job = new Job(JobDefOfLocal.UseGeoscape, this);
                        selPawn.jobs.TryTakeOrderedJob(job);
                    }
                }
            });
        }

        public void OpenConsole(Pawn pawn)
        {
            QuestsManager.Communications.OpenCommunications(pawn);
        }
    }
}
