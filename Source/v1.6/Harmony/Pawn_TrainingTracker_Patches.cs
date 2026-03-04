using HarmonyLib;
using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class Pawn_TrainingTracker_Patches
    {
        // Shadespirits do not lose training, so do not need to tick their training tracker.
        [HarmonyPatch(typeof(Pawn_TrainingTracker), "TrainingTrackerTickRare")]
        public class Pawn_TrainingTracker_TrainingTrackerTickRare_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(Pawn ___pawn)
            {
                return !JDG_Utils.IsShadeSpirit(___pawn);
            }
        }
    }
}
