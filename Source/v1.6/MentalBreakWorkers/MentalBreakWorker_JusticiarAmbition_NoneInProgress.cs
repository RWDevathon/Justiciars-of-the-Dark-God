using System.Linq;
using Verse;

namespace ArtificialBeings
{
    public class MentalBreakWorker_JusticiarAmbition_NoneInProgress : MentalBreakWorker_JusticiarAmbition
    {
        // An Ambition of this class is only available if there are no other pre-existing ambitions or inspirations which are not completed.
        public override bool BreakCanOccur(Pawn pawn)
        {
            if (pawn.Inspired || pawn.health.hediffSet.hediffs.Any(hediff => hediff is Hediff_MentalTracker tracker && !tracker.complete))
            {
                return false;
            }
            return base.BreakCanOccur(pawn);
        }
    }
}
