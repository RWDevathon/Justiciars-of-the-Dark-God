using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ArtificialBeings
{
    public class LordToil_PsychicRitual_Patch
    {
        // The Dark God is displeased by completing psychic rituals that pay tribute to a rival "deity". It is pleased by Void Provocation rituals, as this is a challenge to a pretender.
        [HarmonyPatch(typeof(LordToil_PsychicRitual), "RitualCompleted")]
        public static class LordToil_PsychicRitual_RitualCompleted_Patch
        {
            [HarmonyPostfix]
            public static void Listener(LordToil_PsychicRitual __instance, PsychicRitualRoleAssignments ___assignments)
            {
                if (__instance.RitualData.playerRitual)
                {
                    if (__instance.RitualData.psychicRitual.def == JDG_PsychicRitualDefOf.VoidProvocation)
                    {
                        bool anyJusticiarsAffected = false;
                        foreach (Pawn pawn in __instance.Map.mapPawns.FreeColonists)
                        {
                            if (!pawn.Dead && pawn.Faction == Faction.OfPlayer && (JDG_Utils.IsJusticiar(pawn) || JDG_Utils.IsAcolyte(pawn)))
                            {
                                pawn.health.hediffSet.GetFirstHediff<Hediff_Devotee>()?.NotifyFavorGained(10f);
                            }
                        }
                        if (anyJusticiarsAffected)
                        {
                            Messages.Message("JDG_VoidProvocationFavor".Translate(10.ToString()), MessageTypeDefOf.PositiveEvent, false);
                        }
                    }
                    else
                    {
                        JDG_Utils.DispleaseJusticiars("JDG_PerformingPsychicRituals", 10f);
                    }
                }
            }
        }
    }
}
