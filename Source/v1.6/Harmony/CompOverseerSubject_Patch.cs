using HarmonyLib;
using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class CompOverseerSubject_Patch
    {
        // Shadespirits can not go feral (as mechanoids).
        [HarmonyPatch(typeof(CompOverseerSubject), "CanGoFeral")]
        public class CompOverseerSubject_CanGoFeral_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(Pawn pawn)
            {
                return !JDG_Utils.IsShadeSpirit(pawn);
            }
        }
    }
}
