using Harmony;
using QuestRim;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Patch
{
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy))]
    [HarmonyPatch("TryResolveRaidFaction")]
    public class IncidentWorker_RaidEnemyNonAgressivePact
    {
        static void Postfix(ref bool __result, IncidentParms parms)
        {
            if(parms.faction != null)
            {
                FactionInteraction interaction = QuestsManager.Communications.FactionManager.GetInteraction(parms.faction);
                if (interaction != null)
                {
                    var war = interaction.FirstWarWithPlayer();
                    if (war != null)
                    {
                        __result = true;
                    }
                    else
                    {
                        var option = interaction.GetOption<CommOption_NonAgressionPact>();
                        if (option != null)
                        {
                            __result = !option.Signed;
                        }
                    }
                }
            }
        }
    }
}
