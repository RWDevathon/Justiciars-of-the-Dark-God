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
            Find.LetterStack.ReceiveLetter("ABF_InspirationSucceeded".Translate(), "ABF_InspirationSucceeded_VowOfSilence".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NeutralEvent);
        }
    }
}
