using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class JobDriver_ContemplateOrb : JobDriver
    {
        public Thing TargetThing => job.GetTarget(TargetIndex.A).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            pawn.Reserve(TargetThing, job);
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_General.Wait(5000).WithProgressBarToilDelay(TargetIndex.A); ;
            Toil endToil = ToilMaker.MakeToil("MakeNewToils");
            endToil.initAction = delegate
            {
                TargetThing.TryGetComp<ThingComp_ObsidianOrb>()?.Notify_Used(pawn);
            };
            endToil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return endToil;
        }
    }
}
