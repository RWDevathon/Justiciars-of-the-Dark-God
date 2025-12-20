using RimWorld;
using Verse;

namespace ArtificialBeings
{
    // Inspirations using this worker cannot be selected if any ambitions or inspirations are in progress.
    public class InspirationWorker_Justiciar_NoneInProgress : InspirationWorker_Justiciar
    {
        public override bool InspirationCanOccur(Pawn pawn)
        {
            return base.InspirationCanOccur(pawn) && !pawn.Inspired && !pawn.health.hediffSet.hediffs.Any(hediff => hediff is Hediff_MentalTracker tracker && !tracker.complete);
        }
    }
}
