using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoreEvents.Things;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace MoreEvents.Events.ClimateBomb
{
    public class ClimateBombSite : VisitableSite
    {
        public CaravanVisitAction_ClimateBomb caravanAction;
        public ClimateBombComp Comp;

        public Building_ClimateBomb Bomb => Comp.Bomb;

        public override void SpawnSetup()
        {
            base.SpawnSetup();

            Comp = GetComponent<ClimateBombComp>();

            RemoveAfterLeave = false;

            caravanAction = new CaravanVisitAction_ClimateBomb(this);
        }
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Caravan caravan, MapParent mapParent)
        {
            return CaravanArrivalActionUtility.GetFloatMenuOptions(() => caravanAction.CanVisit(caravan, mapParent), () => caravanAction, "EnterMap".Translate(mapParent.Label), caravan, mapParent.Tile, mapParent);
        }

        public override bool CanLeave()
        {
            if (GenHostility.AnyHostileActiveThreatToPlayer(this.Map))
            {
                Messages.Message(Translator.Translate("EnemyOnTheMap"), MessageTypeDefOf.NeutralEvent, false);
                return false;
            }

            return true;
        }

        public void DisarmBomb()
        {
            RemoveAfterLeave = true;

            var letter = LetterMaker.MakeLetter(Translator.Translate("SuccessDisarming"), Translator.Translate("SuccessDisarmingDesc"), LetterDefOf.PositiveEvent);
            Find.LetterStack.ReceiveLetter(letter);
        }

        public void DetonateBomb()
        {
            if (HasMap)
            {
                Bomb.Detonate();
            }
            else
            {
                Find.WorldObjects.Remove(this);
            }

            RemoveAfterLeave = true;
            var condition = GameConditionMaker.MakeCondition(GameConditionDefOfLocal.ClimateChaos, Rand.Range(15, 25) * 60000);
            Find.World.gameConditionManager.RegisterCondition(condition);
            Find.LetterStack.ReceiveLetter(GameConditionDefOfLocal.ClimateChaos.label, GameConditionDefOfLocal.ClimateChaos.description, LetterDefOf.NegativeEvent);
        }
    }
}
