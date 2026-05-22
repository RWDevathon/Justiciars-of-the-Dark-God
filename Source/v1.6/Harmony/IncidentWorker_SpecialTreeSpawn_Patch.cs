using HarmonyLib;
using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class IncidentWorker_SpecialTreeSpawn_Patch
    {
        // If players have protected themselves against Harbinger Tree incidents and that protection is active, then it cannot fire.
        // Note that this is a patch on the special tree spawn incident worker rather than harbinger tree spawn specifically, as the latter lacks an override for this method.
        [HarmonyPatch(typeof(IncidentWorker_SpecialTreeSpawn), "CanFireNowSub")]
        public static class IncidentWorker_SpecialTreeSpawn_CanFireNowSub_Patch
        {
            [HarmonyPrefix]
            public static bool Listener(IncidentDef ___def, ref bool __result)
            {
                if (___def.treeDef == ThingDefOf.Plant_TreeHarbinger && Current.Game.GetComponent<GameComponent_Justiciars>().tickProtectedAgainstHarbingerTreeSpawnsUntil > GenTicks.TicksGame)
                {
                    __result = false;
                    return false;
                }
                return true;
            }
        }
    }
}
