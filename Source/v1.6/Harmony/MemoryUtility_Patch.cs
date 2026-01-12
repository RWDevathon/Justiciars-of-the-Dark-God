using HarmonyLib;
using Verse.Profile;

namespace ArtificialBeings
{
    public class MemoryUtility_Patch
    {
        // When the game cleans up all maps and the world, also clear static references to some game-specific caches held by the utils class.
        [HarmonyPatch(typeof(MemoryUtility), "ClearAllMapsAndWorld")]
        public static class MemoryUtility_ClearAllMapsAndWorld_Patch
        {
            [HarmonyPostfix]
            public static void Listener()
            {
                JDG_Utils.ClearCaches();
            }
        }
    }
}
