using HarmonyLib;
using RimWorld;

namespace ArtificialBeings
{
    public class HarbingerTree_Patch
    {
        // Harbinger trees spawning as a result of being fed costs justiciars favor.
        [HarmonyPatch(typeof(HarbingerTree), "SpawnNewTree")]
        public static class HarbingerTree_SpawnNewTree_Patch
        {
            [HarmonyPostfix]
            public static void Listener(HarbingerTree __instance)
            {
                JDG_Utils.DispleaseJusticiars("JDG_HarbingersPermitted", 5f);
            }
        }
    }
}
