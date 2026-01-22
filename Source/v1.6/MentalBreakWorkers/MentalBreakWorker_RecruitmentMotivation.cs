using Verse;

namespace ArtificialBeings
{
    public class MentalBreakWorker_RecruitmentMotivation : MentalBreakWorker_JusticiarAmbition
    {
        // Justiciars cannot induct new members unless they have acquired at least 100 favor over their lifetime.
        public override bool BreakCanOccur(Pawn pawn)
        {
            if (pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>()?.FavorTotalAccrued >= 100f)
            {
                return base.BreakCanOccur(pawn);
            }
            return false;
        }
    }
}
