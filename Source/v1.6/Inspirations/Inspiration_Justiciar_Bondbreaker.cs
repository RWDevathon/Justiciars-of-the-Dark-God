using RimWorld;
using Verse;

namespace ArtificialBeings
{
    // This inspiration just creates a Hediff and then reacts as appropriate to it.
    public class Inspiration_Justiciar_Bondbreaker : Inspiration
    {
        Hediff_Inspiration_Bondbreaker bondbreakerHediff;

        // Use the Hediff's expiration timer, not the InspirationDef's.
        public override string InspectLine
        {
            get
            {
                return def.baseInspectLine + " (" + "ExpiresIn".Translate() + ": " + bondbreakerHediff.TicksRemaining.ToStringTicksToPeriod() + ")";
            }
        }

        // Explicitly do not allow parent method to do anything. We do not want the Inspiration to be responsible for ending itself.
        public override void InspirationTick(int delta)
        {
        }

        public override void PostStart(bool sendLetter = true)
        {
            base.PostStart(sendLetter);
            bondbreakerHediff = (Hediff_Inspiration_Bondbreaker)HediffMaker.MakeHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_Inspiration_Bondbreaker, pawn);
            bondbreakerHediff.BondbreakerInspiration = this;
            pawn.health.AddHediff(bondbreakerHediff);
        }
    }
}
