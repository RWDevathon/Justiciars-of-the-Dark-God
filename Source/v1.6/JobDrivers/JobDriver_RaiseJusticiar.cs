using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class JobDriver_RaiseJusticiar : JobDriver
    {
        public Corpse Corpse => job.GetTarget(TargetIndex.A).Thing as Corpse;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            pawn.Reserve(Corpse, job);
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil raisingJusticiar = Toils_General.Wait(600).WithProgressBarToilDelay(TargetIndex.A);
            raisingJusticiar.initAction = delegate
            {
                BlackVeil veil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
                veil.Radius = 0.9f;
                veil.ticksLeft = 900;
                GenSpawn.Spawn(veil, Corpse.Position, Corpse.Map);
            };
            yield return raisingJusticiar;
            Toil raiseJusticiar = ToilMaker.MakeToil("MakeNewToils");
            raiseJusticiar.initAction = delegate
            {
                Find.WindowStack.Add(new Dialog_RaiseJusticiar(Corpse.InnerPawn, Corpse.Position, Corpse.Map));
                Corpse.Destroy();
            };
            raiseJusticiar.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return raiseJusticiar;
        }
    }
}
