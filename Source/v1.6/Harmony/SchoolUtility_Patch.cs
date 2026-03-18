using HarmonyLib;
using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class SchoolUtility_Patch
    {
        // It is assumed that it is called for humanlike pawns, which may not be true when dark creatures can be given direct orders. They cannot teach.
        [HarmonyPatch(typeof(SchoolUtility), nameof(SchoolUtility.CanTeachNow))]
        public class SchoolUtility_CanTeachNow_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(Pawn teacher, ref bool __result)
            {
                if (teacher.NonHumanlikeOrWildMan())
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}
