using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    // Simple workgiver for doing bills that skips non-Justiciars.
    public class WorkGiver_DoBill_Justiciar : WorkGiver_DoBill
    {
        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            return !JDG_Utils.IsJusticiar(pawn) && base.ShouldSkip(pawn, forced);
        }

        public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
        {
            if (!JDG_Utils.IsJusticiar(pawn))
            {
                return null;
            }
            return base.JobOnThing(pawn, thing, forced);
        }
    }
}
