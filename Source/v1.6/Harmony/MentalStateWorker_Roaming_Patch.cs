using HarmonyLib;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class MentalStateWorker_Roaming_Patch
    {
        // Shadespirits do not roam away.
        [HarmonyPatch(typeof(MentalStateWorker_Roaming), nameof(MentalStateWorker_Roaming.CanRoamNow))]
        public class MentalStateWorker_Roaming_CanRoamNow_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(Pawn pawn)
            {
                return !JDG_Utils.IsShadeSpirit(pawn);
            }
        }
    }
}
