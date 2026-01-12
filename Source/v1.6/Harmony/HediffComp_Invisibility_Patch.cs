using HarmonyLib;
using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class HediffComp_Invisibility_Patch
    {
        // Non-player pawns that are invisible are forcibly revealed while in a Justiciar's conjured darkness.
        [HarmonyPatch(typeof(HediffComp_Invisibility), "CompPostTick")]
        public static class HediffComp_Invisibility_CompPostTick_Patch
        {

            [HarmonyPostfix]
            public static void Listener(HediffComp_Invisibility __instance)
            {
                Pawn pawn = __instance.Pawn;
                if (pawn.Spawned && pawn.Faction != Faction.OfPlayer && JDG_Utils.IsDark(pawn.Position, pawn.Map))
                {
                    __instance.DisruptInvisibility();
                }
            }
        }
    }
}
