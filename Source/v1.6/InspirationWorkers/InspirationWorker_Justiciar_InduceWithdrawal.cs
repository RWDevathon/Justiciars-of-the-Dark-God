using RimWorld;
using Verse;

namespace ArtificialBeings
{
    // This inspiration should only occur if the justiciar is not currently experiencing withdrawal symptoms (or else it'd be too easy).
    public class InspirationWorker_Justiciar_InduceWithdrawal : InspirationWorker_Justiciar
    {
        public override bool InspirationCanOccur(Pawn pawn)
        {
            return base.InspirationCanOccur(pawn) && !pawn.health.hediffSet.hediffs.Any(hediff => hediff is Hediff_Addiction addiction && addiction.CurStageIndex > 0);
        }
    }
}
