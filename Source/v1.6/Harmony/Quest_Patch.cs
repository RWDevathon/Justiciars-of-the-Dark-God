using HarmonyLib;
using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class Quest_Patch
    {
        // The Dark God may pass judgement on how certains quests are ended.
        [HarmonyPatch(typeof(Quest), "End")]
        public static class Quest_End_Patch
        {
            [HarmonyPostfix]
            public static void Listener(QuestEndOutcome outcome, Quest __instance)
            {
                // Succeeding in charitable quests is frowned upon.
                if (__instance.root.defaultCharity && (outcome == QuestEndOutcome.Success || outcome == QuestEndOutcome.Unknown))
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
