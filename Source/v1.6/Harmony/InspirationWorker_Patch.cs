using HarmonyLib;
using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class InspirationWorker_Patch
    {
        // Justiciars only get their own unique inspirations. If none of their unique inspirations are available, one will simply not happen.
        [HarmonyPatch(typeof(InspirationWorker), "InspirationCanOccur")]
        public static class InspirationWorker_InspirationCanOccur_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn pawn, ref bool __result, InspirationWorker __instance)
            {
                if (pawn.story?.traits?.HasTrait(JDG_TraitDefOf.ABF_Trait_Justiciar_Adherent) == true && !(__instance is InspirationWorker_Justiciar))
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}
