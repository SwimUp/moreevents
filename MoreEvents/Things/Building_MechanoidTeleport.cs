using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse.AI.Group;
using System.IO;

namespace MoreEvents.Things
{
    public class Building_MechanoidTeleport : Building
    {
        private int spawnTime = 30000;

        private Lord lord;

        private bool destroyed = false;
        private int lightTime = 20;
        private int offset = 0;
        private int count;
        private int maxRadius = 12;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);

            count = Rand.Range(20, 40);
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (!destroyed)
            {
                destroyed = true;
                return;
            }

            base.Destroy(mode);
        }

        public override void Tick()
        {
            base.Tick();

            if (!destroyed)
            {
                if (Find.TickManager.TicksGame % spawnTime == 0)
                {
                    int lightCount = Rand.Range(4, 7);
                    for (int i = 0; i < lightCount; i++)
                    {
                        if (CellFinder.TryFindRandomCellNear(Position, Map, 10, null, out IntVec3 lightPos))
                        {
                            Find.CurrentMap.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(Find.CurrentMap, lightPos));
                        }
                    }

                    PawnKindDef kind;
                    if (!(from def in DefDatabase<PawnKindDef>.AllDefs where def.RaceProps.IsMechanoid && def.isFighter select def).TryRandomElement(out kind))
                    {
                        return;
                    }
                    if (!CellFinder.TryFindRandomCellNear(Position, Map, 4, null, out IntVec3 center))
                    {
                        return;
                    }

                    LordJob_AssaultColony lordJob = new LordJob_AssaultColony(Faction.OfMechanoids, false, true, false, false, false);
                    lord = LordMaker.MakeNewLord(Faction.OfMechanoids, lordJob, Map, null);
                    int count = Rand.Range(1, 4);
                    for (int i = 0; i < count; i++)
                    {
                        PawnGenerationRequest request = new PawnGenerationRequest(kind, Faction.OfMechanoids, PawnGenerationContext.NonPlayer, -1, true, false, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null, null);
                        Pawn pawn = PawnGenerator.GeneratePawn(request);
                        GenSpawn.Spawn(pawn, center, Map);
                        lord.AddPawn(pawn);
                    }
                }
            }
            else
            {
                if (Find.TickManager.TicksGame % lightTime == 0)
                {
                    DestroyAnim();
                }
            }
        }

        private void DestroyAnim()
        {
            if(CellFinder.TryFindRandomCellNear(Position, Map, offset, null, out IntVec3 pos))
            {
                Find.CurrentMap.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(Find.CurrentMap, pos));
                offset += Rand.Range(1, 3);
                if (offset >= maxRadius)
                    offset = maxRadius;
                count--;
            }

            if(count == 0)
            {
                Destroy(0);
            }
        }
    }
}
