using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public abstract class JobDriver_JusticiarBase : JobDriver
    {
        protected virtual bool SpawnsAndMaintainsVeil => true;

        protected virtual int WaitTicks => 600;

        protected virtual float VeilRadius => 0.9f;

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
            if (WaitTicks > 0)
            {
                Toil wait = Toils_General.Wait(WaitTicks).WithProgressBarToilDelay(TargetIndex.A);
                if (SpawnsAndMaintainsVeil)
                {
                    BlackVeil veil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
                    veil.Radius = VeilRadius;
                    wait.initAction = delegate
                    {
                        veil.ticksLeft = 30;
                        GenSpawn.Spawn(veil, TargetThing.Position, TargetThing.Map);
                        WaitInitAction();
                    };
                    wait.AddPreTickIntervalAction(delegate (int delta)
                    {
                        veil.ticksLeft = delta + 30;
                        WaitTickIntervalAction();
                    });
                }
                yield return wait;
            }
            Toil endToil = ToilMaker.MakeToil("MakeNewToils");
            endToil.initAction = delegate
            {
                EndInitAction();
            };
            endToil.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return endToil;
        }

        protected virtual void WaitInitAction() { }

        protected virtual void WaitTickIntervalAction() { }

        protected virtual void EndInitAction() { }
    }
}
