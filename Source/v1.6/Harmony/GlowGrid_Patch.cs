using HarmonyLib;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class GlowGrid_Patch
    {
        // Cells which are in the darkness grid have no glow.
        [HarmonyPatch(typeof(GlowGrid), "GroundGlowAt")]
        public static class GlowGrid_GroundGlowAt_Patch
        {
            private static Dictionary<Map, MapComponent_DarknessGrid> darkGridDict = new Dictionary<Map, MapComponent_DarknessGrid> ();

            [HarmonyPrefix]
            public static bool Listener(IntVec3 c, ref float __result, Map ___map, bool ignoreCavePlants, bool ignoreSky)
            {
                if (!darkGridDict.ContainsKey(___map))
                {
                    darkGridDict[___map] = ___map.GetComponent<MapComponent_DarknessGrid>();
                }
                if (darkGridDict[___map].IsDark(c))
                {
                    __result = 0f;
                    return false;
                }
                return true;
            }
        }
    }
}
