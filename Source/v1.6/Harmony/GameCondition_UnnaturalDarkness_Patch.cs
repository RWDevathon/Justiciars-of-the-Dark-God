using HarmonyLib;
using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class GameCondition_UnnaturalDarkness_Patch
    {
        // Justiciars are immune to Unnatural Darkness. This does not allow the player to see in it, though.
        [HarmonyPatch(typeof(GameCondition_UnnaturalDarkness), "AffectedByDarkness")]
        public static class GameCondition_UnnaturalDarkness_AffectedByDarkness_Patch
        {

            [HarmonyPrefix]
            public static bool Listener(ref bool __result, Pawn pawn)
            {
                if (JDG_Utils.IsJusticiar(pawn))
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}
