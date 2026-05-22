using HarmonyLib;
using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class CompGoldenCube_Patch
    {
        // Those who have contemplated the orb cannot become interested in the cube until it expires.
        [HarmonyPatch(typeof(CompGoldenCube), "ValidatePawn")]
        public static class CompGoldenCube_ValidatePawn_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(ref bool __result, Pawn pawn)
            {
                if (pawn.health.hediffSet.HasHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_ContemplatedOrb))
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}
