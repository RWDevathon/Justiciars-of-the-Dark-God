using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class Inspiration_Justiciar_EmbraceScarcity : Inspiration
    {
        public int ticksSinceScarcityBegan = 0;

        public bool complete = false;

        public override void InspirationTick(int delta)
        {
            if (pawn.health.hediffSet.hediffs.Any(hediff => hediff.def == HediffDefOf.Malnutrition && hediff.Severity >= 0.5f) && pawn.needs?.mood?.CurLevelPercentage < 0.5f)
            {
                ticksSinceScarcityBegan += delta;
                if (ticksSinceScarcityBegan > GenDate.TicksPerDay)
                {
                    complete = true;
                    End();
                }
            }
            else
            {
                ticksSinceScarcityBegan = 0;
            }
            // Handles expiration
            base.InspirationTick(delta);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksSinceScarcityBegan, "ABF_ticksSinceScarcityBegan");
        }

        public override void PostEnd()
        {
            if (complete)
            {
                Find.LetterStack.ReceiveLetter("JDG_InspirationSucceeded".Translate(), "JDG_InspirationSucceeded_EmbraceScarcity".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NeutralEvent);
                pawn.mindState.mentalBreaker.Notify_RecoveredFromMentalState();
            }
            else
            {
                // If the Justiciar can reach the map edge, have them give up and leave. Otherwise, they just die of despair.
                if (!pawn.Downed && pawn.CanReachMapEdge())
                {
                    pawn.mindState.mentalStateHandler.TryStartMentalState(JDG_MentalStateDefOf.Binging_DrugExtreme, reason: "JDG_InspirationFailed".Translate(), forced: true);
                }
                else
                {
                    Hediff crushingDespair = HediffMaker.MakeHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_CrushingDespair, pawn);
                    if (crushingDespair.TryGetComp<HediffComp_Disappears>() is HediffComp_Disappears comp)
                    {
                        comp.SetDuration(comp.disappearsAfterTicks * 10);
                    }
                    pawn.health.AddHediff(crushingDespair);
                    Find.LetterStack.ReceiveLetter("JDG_InspirationFailed".Translate(), "JDG_InspirationFailed_EmbraceScarcity".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NegativeEvent);
                    pawn.Kill(null, crushingDespair);
                }
            }
        }

        public override void PostStart(bool sendLetter = true)
        {
            base.PostStart(sendLetter);

            // Receiving an inspiration results in favor loss.
            pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>()?.NotifyFavorLost(25f);

            // If the player has not yet learned about inspirations, they will also receive a learning helper tip about how they work.
            LessonAutoActivator.TeachOpportunity(JDG_ConceptDefOf.ABF_Concept_Justiciar_Inspirations, OpportunityType.Critical);
        }
    }
}
