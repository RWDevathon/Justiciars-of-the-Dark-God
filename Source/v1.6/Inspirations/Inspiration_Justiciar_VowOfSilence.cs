using RimWorld;
using Verse;

namespace ArtificialBeings
{
    // This inspiration just creates a Hediff and then maintains it.
    public class Inspiration_Justiciar_VowOfSilence : Inspiration
    {
        private Hediff silenceHediff;

        public override void InspirationTick(int delta)
        {
            base.InspirationTick(delta);
            // Get or Add will create the hediff for us if it is missing for whatever reason.
            if (silenceHediff == null)
            {
                silenceHediff = pawn.health.GetOrAddHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_Inspiration_VowOfSilence);
            }
        }

        public override void PostEnd()
        {
            base.PostEnd();
            if (silenceHediff != null)
            {
                pawn.health.RemoveHediff(silenceHediff);
            }
            Find.LetterStack.ReceiveLetter("JDG_InspirationSucceeded".Translate(), "JDG_InspirationSucceeded_VowOfSilence".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NeutralEvent);
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
