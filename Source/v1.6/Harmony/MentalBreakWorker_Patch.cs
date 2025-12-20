using HarmonyLib;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class MentalBreakWorker_Patch
    {
        // Justiciars only get their own unique mental breaks. If none of their unique breaks are available, one will simply not happen.
        [HarmonyPatch(typeof(MentalBreakWorker), "BreakCanOccur")]
        public static class MentalBreakWorker_BreakCanOccur_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn pawn, ref bool __result, MentalBreakWorker __instance)
            {
                if (pawn.story?.traits?.HasTrait(JDG_TraitDefOf.ABF_Trait_Justiciar_Adherent) == true && !(__instance is MentalBreakWorker_JusticiarAmbition))
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}
