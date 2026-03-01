using Verse;
using HarmonyLib;
using RimWorld;

namespace ArtificialBeings
{
    public class FleeUtility_Patches
    {
        // Dark creatures that are drafted should not flee danger.
        [HarmonyPatch(typeof(FleeUtility), "ShouldAnimalFleeDanger")]
        public class FleeUtility_ShouldAnimalFleeDanger_Patch
        {
            [HarmonyPostfix]
            public static bool Listener(bool __result, Pawn pawn)
            {
                return __result && !(JDG_Utils.darkCreatureKinds.Contains(pawn.kindDef) && pawn.Drafted);
            }
        }
    }
}