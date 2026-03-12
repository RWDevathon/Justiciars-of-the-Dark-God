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
                    string messageString = null;
                    MessageTypeDef messageTypeDef = null;
                    if (__instance.RitualData.psychicRitual.def == JDG_PsychicRitualDefOf.VoidProvocation)
                    {
                        foreach (Pawn pawn in __instance.Map.mapPawns.FreeColonists)
                        {
                            if (!pawn.Dead && pawn.Faction == Faction.OfPlayer && (JDG_Utils.IsJusticiar(pawn) || JDG_Utils.IsAcolyte(pawn)))
                            {
                                pawn.health.hediffSet.GetFirstHediff<Hediff_Devotee>()?.NotifyFavorGained(10f);
                                messageString = "JDG_VoidProvocationFavor";
                                messageTypeDef = MessageTypeDefOf.PositiveEvent;
                            }
                        }
                    }
                    else
                    {
                        foreach (Pawn pawn in JDG_Utils.Justiciars)
                        {
                            if (!pawn.Dead && pawn.Faction == Faction.OfPlayer)
                            {
                                pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(JDG_ThoughtDefOf.ABF_Thought_Justiciar_Displeased);
                                Hediff crushingDespair = HediffMaker.MakeHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_CrushingDespair, pawn);
                                if (crushingDespair.TryGetComp<HediffComp_Disappears>() is HediffComp_Disappears comp)
                                {
                                    comp.SetDuration(GenDate.TicksPerDay);
                                }
                                pawn.health.AddHediff(crushingDespair);
                                pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>()?.NotifyFavorLost(10f);
                                messageString = "JDG_JusticiarsDispleased";
                                messageTypeDef = MessageTypeDefOf.NegativeEvent;
                            }
                        }
                    }
                    if (messageString != null)
                    {
                        Messages.Message(messageString.Translate(10.ToString()), messageTypeDef, false);
                    }
                }
            }
        }
    }
}
