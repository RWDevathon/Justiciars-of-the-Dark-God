using Verse;
using HarmonyLib;
using RimWorld;

namespace ArtificialBeings
{
    public class IncidentWorker_Revenant_Patch
    {
        // Perceptors can warn of revenants arriving unexpectedly.
        [HarmonyPatch(typeof(IncidentWorker_Revenant), "TryExecuteWorker")]
        public class IncidentWorker_Revenant_TryExecuteWorker_Patch
        {
            [HarmonyPostfix]
            public static void Listener(bool __result, IncidentParms parms)
            {
                if (__result && ((Map)parms.target).mapPawns.ColonyAnimals.Any(pawn => pawn.kindDef == JDG_PawnKindDefOf.ABF_PawnKind_Justiciar_Player_CreaturePerceptor))
                {
                    Find.LetterStack.ReceiveLetter("JDG_PerceptorDetectedInvisibleThreat".Translate(), "JDG_PerceptorDetectedInvisibleThreatDesc".Translate(), LetterDefOf.ThreatBig);
                }
            }
        }
    }
}