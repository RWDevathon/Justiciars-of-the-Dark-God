using Verse;

namespace ArtificialBeings
{
    public class JobDriver_StealFavor : JobDriver_JusticiarBase
    {
        public Corpse Corpse => TargetThing as Corpse;

        protected override void EndInitAction()
        {
            Pawn target = Corpse.InnerPawn;
            float toTransfer;
            if (target.health.hediffSet.GetFirstHediff<Hediff_Devotee>() is Hediff_Devotee devoteeHediff)
            {
                toTransfer = devoteeHediff.FavorCurrent;
                devoteeHediff.NotifyFavorLost(toTransfer);
            }
            else
            {
                return;
            }

            pawn.health.hediffSet.GetFirstHediff<Hediff_Devotee>().NotifyFavorGained(toTransfer);
        }
    }
}
