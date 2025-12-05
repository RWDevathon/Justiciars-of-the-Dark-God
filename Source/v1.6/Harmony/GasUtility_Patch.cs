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
            private static Dictionary<Map, MapComponent_DarknessGrid> darkGridDict = new Dictionary<Map, MapComponent_DarknessGrid> ();

            [HarmonyPostfix]
            public static bool Listener(bool __result, IntVec3 cell, Map map, GasType gasType)
            {
                if (__result || gasType != GasType.BlindSmoke)
                {
                    return __result;
                }

                if (!darkGridDict.ContainsKey(map))
                {
                    darkGridDict[map] = map.GetComponent<MapComponent_DarknessGrid>();
                }
                return darkGridDict[map].IsDark(cell);
            }
        }
    }
}
