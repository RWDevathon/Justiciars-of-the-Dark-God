using HarmonyLib;
using RimWorld;
using Verse;

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
                bool anyJusticiarsAffected = false;
                foreach (Pawn justiciar in JDG_Utils.Justiciars)
                {
                    if (justiciar.Faction == Faction.OfPlayer && !justiciar.Dead && justiciar.Map == __instance.Map)
                    {
                        justiciar.needs?.mood?.thoughts?.memories?.TryGainMemory(JDG_ThoughtDefOf.ABF_Thought_Justiciar_Displeased);
                        justiciar.health.hediffSet.GetFirstHediff<Hediff_Justiciar>()?.NotifyFavorLost(5f);
                        anyJusticiarsAffected = true;
                    }
                }
                if (anyJusticiarsAffected)
                {
                    Messages.Message("JDG_JusticiarsDispleased".Translate(5.ToString()), MessageTypeDefOf.NegativeEvent, false);
                }
            }
        }
    }
}
