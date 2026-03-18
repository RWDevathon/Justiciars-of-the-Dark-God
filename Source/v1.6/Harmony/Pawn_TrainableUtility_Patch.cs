using HarmonyLib;
using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class Pawn_TrainableUtility_Patch
    {
        // Shadespirits only answer to Justiciar masters, and can have any Justiciar as their master.
        [HarmonyPatch(typeof(TrainableUtility), nameof(TrainableUtility.CanBeMaster))]
        public class TrainableUtility_CanBeMaster_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(Pawn master, Pawn animal, bool checkSpawned, ref bool __result)
            {
                if (JDG_Utils.IsShadeSpirit(animal))
                {
                    if ((checkSpawned && !master.Spawned) || master.IsPrisoner || !JDG_Utils.IsJusticiar(master))
                    {
                        __result = false;
                        return false;
                    }
                    __result = true;
                    return false;
                }
                return true;
            }
        }
    }
}
