using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace ArtificialBeings
{
    public class LordToil_PsychicRitual_Patch
    {
        // The Dark God is displeased by completing psychic rituals that pay tribute to a rival "deity".
        [HarmonyPatch(typeof(LordToil_PsychicRitual), "RitualCompleted")]
        public static class LordToil_PsychicRitual_RitualCompleted_Patch
        {
            [HarmonyPostfix]
            public static void Listener(LordToil_PsychicRitual __instance)
            {
                // Void provocation is excluded, as this is insulting the rival "deity" and intentionally provoking hostility from it.
                if (__instance.RitualData.playerRitual && !(__instance.RitualData.CurPsychicRitualToil is PsychicRitualToil_VoidProvocation))
                {
                    bool anyJusticiarsAffected = false;
                    foreach (Pawn justiciar in JDG_Utils.Justiciars)
                    {
                        if (justiciar.Faction == Faction.OfPlayer && !justiciar.Dead)
                        {
                            justiciar.needs?.mood?.thoughts?.memories?.TryGainMemory(JDG_ThoughtDefOf.ABF_Thought_Justiciar_Displeased);
                            Hediff crushingDespair = HediffMaker.MakeHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_CrushingDespair, justiciar);
                            if (crushingDespair.TryGetComp<HediffComp_Disappears>() is HediffComp_Disappears comp)
                            {
                                comp.SetDuration(GenDate.TicksPerDay);
                            }
                            justiciar.health.AddHediff(crushingDespair);
                            justiciar.health.hediffSet.GetFirstHediff<Hediff_Justiciar>()?.NotifyFavorLost(10f);
                            anyJusticiarsAffected = true;
                        }
                    }
                    if (anyJusticiarsAffected)
                    {
                        Messages.Message("JDG_JusticiarsDispleased".Translate(10.ToString()), MessageTypeDefOf.NegativeEvent, false);
                    }
                }
            }
        }
    }
}
