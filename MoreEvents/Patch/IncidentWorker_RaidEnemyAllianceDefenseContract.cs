using Harmony;
using QuestRim;
using RimOverhaul.Alliances;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimOverhaul.Patch
{
    [HarmonyPatch(typeof(IncidentWorker_RaidEnemy))]
    [HarmonyPatch("TryExecuteWorker")]
    public class IncidentWorker_RaidEnemyAllianceDefenseContract
    {
        static void Postfix(ref bool __result, IncidentParms parms)
        {
            if (__result)
            {
                Map map = parms.target as Map;
                if (map != null)
                {
                    if (map.ParentFaction != null)
                    {
                        FactionInteraction inter = QuestsManager.Communications.FactionManager.GetInteraction(map.ParentFaction);
                        if (inter != null)
                        {
                            Alliance alliance = inter.Alliance;
                            if (alliance != null)
                            {
                                DefenseContractComp defenseContractComp = alliance.AllianceAgreements.FirstOrDefault(x => x is DefenseContractComp) as DefenseContractComp;
                                if (defenseContractComp != null)
                                {
                                    if (defenseContractComp.OwnerFaction == inter)
                                    {
                                        defenseContractComp.SendHelp(parms);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
