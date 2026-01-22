using RimWorld;
using Verse;

namespace ArtificialBeings
{
    // This inspiration simply waits to be notified by ambitions that they are completed. It fails if it expires.
    public class Inspiration_Justiciar_SeekRepentance : Inspiration
    {
        private int ambitionsCompleted = 0;

        public void NotifyAmbitionComplete()
        {
            if (++ambitionsCompleted > 1)
            {
                End();
            }
        }

        public override void PostEnd()
        {
            if (ambitionsCompleted > 1)
            {
                Find.LetterStack.ReceiveLetter("JDG_InspirationSucceeded".Translate(), "JDG_InspirationSucceeded_SeekRepentance".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NeutralEvent);
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
                    Find.LetterStack.ReceiveLetter("JDG_InspirationFailed".Translate(), "JDG_InspirationFailed_SeekRepentance".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NegativeEvent);
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

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ambitionsCompleted, "ABF_ambitionsCompleted");
        }
    }
}
