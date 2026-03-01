using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class JobDriver_DoManualScrying : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil endToil = ToilMaker.MakeToil("MakeNewToils");
            endToil.initAction = delegate
            {
                if (pawn.GetComp<ThingComp_PerceptorVision>() is ThingComp_PerceptorVision perceptorVision)
                {
                    perceptorVision.DoScrying(TargetThingA);
                    perceptorVision.ticksToNextManualScrying = GenDate.TicksPerDay * 5;
                }
            };
            endToil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return endToil;
        }
    }
}
