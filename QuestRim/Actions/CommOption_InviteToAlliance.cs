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
    public class CommOption_InviteToAlliance : InteractionOption
    {
        public override string Label => "CommOption_InviteToAlliance_Label".Translate();

        public DialogDef DialogDef => DialogDefOfLocal.InvitationAlliance;

        public override Color TextColor
        {
            get
            {
                if (Alliance == null || blocked)
                    return Color.gray;

                return Color.white;
            }
        }

        public Alliance Alliance
        {
            get
            {
                if (playerAlliance == null)
                {
                    playerAlliance = QuestsManager.Communications.FactionManager.PlayerAlliance;
                }

                return playerAlliance;
            }
        }

        private int lastUseTicks;
        private bool blocked => lastUseTicks > Find.TickManager.TicksGame;

        public static int TrustMinNeed => 50;

        public int BlockTime => 180000;

        private FactionInteraction defendantFaction;

        private Alliance playerAlliance;
        public override void DoAction(FactionInteraction interaction, Pawn speaker, Pawn defendant)
        {
            if(interaction.InWars.Any(x => x.DefendingFaction.Faction == speaker.Faction || x.DeclaredWarFaction.Faction == speaker.Faction))
            {
                Messages.Message("CommOption_InviteToAlliance_InWar".Translate(), MessageTypeDefOf.NeutralEvent);
                return;
            }

            if(interaction.Faction.RelationKindWith(Faction.OfPlayer) == FactionRelationKind.Hostile)
            {
                Messages.Message("CommOption_InviteToAlliance_NeedAAlly".Translate(), MessageTypeDefOf.NeutralEvent);
                return;
            }

            if(interaction.Trust < TrustMinNeed)
            {
                Messages.Message("CommOption_InviteToAlliance_NeedTrust".Translate(TrustMinNeed), MessageTypeDefOf.NeutralEvent);
                return;
            }

            if(blocked)
            {
                int waitTicks = (lastUseTicks - Find.TickManager.TicksGame) / 60000;
                Messages.Message("CommOption_InviteToAlliance_Wait".Translate(waitTicks.ToString("f2")), MessageTypeDefOf.NeutralEvent);
                return;
            }

            if (Alliance == null)
            {
                Messages.Message("CommOption_InviteToAlliance_NoAlliance".Translate(), MessageTypeDefOf.NeutralEvent);
                return;
            }

            if(interaction.InAnyAlliance())
            {
                Messages.Message("CommOption_InviteToAlliance_AlreadyInAlliance".Translate(), MessageTypeDefOf.NeutralEvent);
                return;
            }

            if(Alliance.Factions.Any(x => x.Faction.HostileTo(interaction.Faction)))
            {
                Messages.Message("CommOption_InviteToAlliance_AnyHostileInAlliance".Translate(), MessageTypeDefOf.NeutralEvent);
                return;
            }

            defendantFaction = interaction;
            Dialog dia = new Dialog(DialogDef, speaker, defendantFaction.Faction.leader);
            dia.Init();
            dia.CloseAction = CheckAnswer;
            Find.WindowStack.Add(dia);
        }

        private void CheckAnswer(string answer)
        {
            if (answer == "удача")
            {
                Alliance.AddFaction(defendantFaction);
            }

            lastUseTicks = Find.TickManager.TicksGame + BlockTime;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref lastUseTicks, "lastUseTicks");
        }

    }
}
