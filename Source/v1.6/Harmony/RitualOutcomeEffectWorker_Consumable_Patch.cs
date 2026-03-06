using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class RitualOutcomeEffectWorker_Consumable_Patch
    {
        // Rituals can apply inspirations directly. So as to not penalize players for doing rituals, justiciars will never get inspirations from them.
        [HarmonyPatch(typeof(RitualOutcomeEffectWorker_Consumable), "ApplyExtraOutcome")]
        public class RitualOutcomeEffectWorker_Consumable_ApplyExtraOutcome_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(ref Dictionary<Pawn, int> totalPresence)
            {
                totalPresence.RemoveAll(pairing => JDG_Utils.IsJusticiar(pairing.Key));
                return true;
            }
        }
    }
}
