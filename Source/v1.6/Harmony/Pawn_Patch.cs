using HarmonyLib;
using Verse;

namespace ArtificialBeings
{
    public class Pawn_Patches
    {
        // Specific shadespirits can be given direct orders.
        [HarmonyPatch(typeof(Pawn), "get_CanTakeOrder")]
        public static class Pawn_get_CanTakeOrder_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(ref Pawn __instance, ref bool __result)
            {
                if (JDG_Utils.darkCreatureKinds.Contains(__instance.kindDef))
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(Pawn), nameof(Pawn.Discard))]
        public static class Pawn_Discard_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref Pawn __instance)
            {
                JDG_Utils.perceptorCheckedPawns.Remove(__instance.thingIDNumber);
            }
        }

        [HarmonyPatch(typeof(Pawn), nameof(Pawn.ExposeData))]
        public static class Pawn_ExposeData_Patch
        {
            [HarmonyPostfix]
            public static void Listener(ref Pawn __instance)
            {
                if (__instance.RaceProps.Humanlike)
                {
                    PerceptorTracker tracker = JDG_Utils.GetPerceptorTracker(__instance);
                    Scribe_Deep.Look(ref tracker, "perceptionTracker");

                    if (Scribe.mode != LoadSaveMode.Saving && tracker != null)
                    {
                        JDG_Utils.perceptorCheckedPawns.SetOrAdd(__instance.thingIDNumber, tracker);
                    }
                }
            }
        }
    }
}
