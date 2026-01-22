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
            if (target.health.hediffSet.GetFirstHediff<Hediff_Justiciar>() is Hediff_Justiciar justiciarHediff)
            {
                toTransfer = justiciarHediff.FavorCurrent;
                justiciarHediff.NotifyFavorLost(toTransfer);
            }
            else if (target.health.hediffSet.GetFirstHediff<Hediff_Acolyte>() is Hediff_Acolyte acolyteHediff)
            {
                toTransfer = acolyteHediff.favorCurrent;
                acolyteHediff.favorCurrent = 0f;
            }
            else
            {
                return;
            }

            if (JDG_Utils.IsJusticiar(pawn))
            {
                pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>().NotifyFavorGained(toTransfer);
            }
            else if (JDG_Utils.IsAcolyte(pawn))
            {
                pawn.health.hediffSet.GetFirstHediff<Hediff_Acolyte>().favorCurrent += toTransfer;
            }
        }
    }
}
