using HarmonyLib;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class GasUtility_Patch
    {
        // Cells which are in the darkness grid are treated as having blindsmoke gas.
        [HarmonyPatch(typeof(GasUtility), "AnyGas")]
        public static class GasUtility_AnyGas_Patch
        {
            [HarmonyPostfix]
            public static bool Listener(bool __result, IntVec3 cell, Map map, GasType gasType)
            {
                if (__result || gasType != GasType.BlindSmoke)
                {
                    return __result;
                }

                return JDG_Utils.IsDark(cell, map);
            }
        }
    }
}
