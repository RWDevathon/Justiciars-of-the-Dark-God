using Verse;
using HarmonyLib;
using RimWorld;

namespace ArtificialBeings
{
    public class PawnAttackGizmoUtility_Patches
    {
        // Dark creatures with the appropriate draft controller should be permitted to be given orders.
        [HarmonyPatch(typeof(PawnAttackGizmoUtility), "CanOrderPlayerPawn")]
        public static class PawnAttackGizmoUtility_CanOrderPlayerPawn_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(Pawn pawn, ref bool __result)
            {
                if (JDG_Utils.darkCreatureKinds.Contains(pawn.kindDef))
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }
    }
}