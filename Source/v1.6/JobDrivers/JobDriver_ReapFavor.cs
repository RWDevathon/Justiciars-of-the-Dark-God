using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class JobDriver_ReapFavor : JobDriver_JusticiarBase
    {
        protected override int WaitTicks => GenDate.TicksPerHour;

        protected override void EndInitAction()
        {
            ThingComp_ProducesFavor favorComp = TargetThing.TryGetComp<ThingComp_ProducesFavor>();
            if (JDG_Utils.IsJusticiar(pawn))
            {
                pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>().NotifyFavorGained(favorComp.currentFavor);
            }
            else if (JDG_Utils.IsAcolyte(pawn))
            {
                pawn.health.hediffSet.GetFirstHediff<Hediff_Acolyte>().NotifyFavorGained(favorComp.currentFavor);
            }
            favorComp.currentFavor = 0;
        }
    }
}
