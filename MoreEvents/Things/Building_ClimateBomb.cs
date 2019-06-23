using MoreEvents.Events.ClimateBomb;
using RimWorld;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace MoreEvents.Things
{
    public class Building_ClimateBomb : Building
    {
        public ClimateBombSite Site;

        private int cycle = 9000;
        private int cycleTimer = 0;

        public float DisarmingProgress = 0;
        public float DisarmingSpeed = 0.00007f;
        public bool Disarmed = false;
        public bool BlownUp = false;

        private List<WeatherDef> weathers => DefDatabase<WeatherDef>.AllDefsListForReading;
        private List<GameConditionDef> incidents = new List<GameConditionDef>()
        {
            GameConditionDefOfLocal.HeatWave,
            GameConditionDefOfLocal.Flashstorm,
            GameConditionDefOfLocal.SandStorm,
            GameConditionDefOfLocal.ColdSnap
        };

        public bool HintShown = false;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref HintShown, "HintShown");
            Scribe_Values.Look(ref BlownUp, "BlownUp");
            Scribe_Values.Look(ref Disarmed, "Disarmed");
            Scribe_Values.Look(ref DisarmingProgress, "DisarmingProgress");
            Scribe_References.Look(ref Site, "Site");
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            cycleTimer = cycle;
        }

        public override string GetInspectString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"{Translator.Translate("DisarmingProgress")}{DisarmingProgress.ToString("f2")}");

            return builder.ToString();
        }

        public void Disarm()
        {
            Disarmed = true;

            if (CellFinder.TryFindRandomCellNear(Position, Map, 6, null, out IntVec3 result))
            {
                GenSpawn.Spawn(ThingDefOfLocal.ColdFusionReactorHeath, result, Map);
            }

            if(Site != null)
                Site.DisarmBomb();
        }

        public void Detonate()
        {
            BlownUp = true;

            LightingDestroyEffect_ClimateBombComp comp = new LightingDestroyEffect_ClimateBombComp
            {
                parent = this
            };
            AllComps.Add(comp);
            comp.Initialize(null);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            if (!BlownUp && !Disarmed)
            {
                yield return new FloatMenuOption(Translator.Translate("DisarmAction"), delegate
                {
                    Job job = new Job(JobDefOfLocal.DisableClimateBomb, this);
                    selPawn.jobs.TryTakeOrderedJob(job);
                });
            }
        }

        public void ShowHint()
        {
            if (HintShown)
                return;

            DiaNode node = new DiaNode(Translator.Translate("DefusingInfo"));
            DiaOption option = new DiaOption("OK");
            node.options.Add(option);
            option.resolveTree = true;

            var dialog = new Dialog_NodeTree(node);
            Find.WindowStack.Add(dialog);
            HintShown = true;
        }

        public override void Tick()
        {
            if (AllComps != null)
            {
                int i = 0;
                for (int count = AllComps.Count; i < count; i++)
                {
                    AllComps[i].CompTick();
                }
            }

            cycleTimer--;

            if(cycleTimer <= 0f)
            {
                cycleTimer = cycle;
                DoChange();
            }
        }

        private void DoChange()
        {
            if(Rand.Chance(0.3f))
            {
                ChangeWeather();
            }
            else
            {
                ChangeIncident();
            }
        }

        private void ChangeWeather()
        {
            WeatherDef def = weathers.RandomElement();

            Map.weatherManager.TransitionTo(def);
        }

        private void ChangeIncident()
        {
            GameConditionDef def = incidents.RandomElement();
            Map.gameConditionManager.RegisterCondition(GameConditionMaker.MakeCondition(def, Rand.Range(4000, 18000)));
        }
    }
}
