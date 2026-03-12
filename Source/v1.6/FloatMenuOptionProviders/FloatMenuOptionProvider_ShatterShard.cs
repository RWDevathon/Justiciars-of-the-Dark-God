using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class FloatMenuOptionProvider_ShatterShard : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool IgnoreFogged => false;

        public override bool TargetThingValid(Thing thing, FloatMenuContext context)
        {
            if (context.FirstSelectedPawn is Pawn pawn && JDG_Utils.IsDevotee(pawn) && thing.def == ThingDefOf.Shard)
            {
                return base.TargetThingValid(thing, context);
            }
            return false;
        }

        protected override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
        {
            if (!context.FirstSelectedPawn.CanReserveAndReach(clickedThing, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, ignoreOtherReservations: true))
            {
                return new FloatMenuOption("JDG_CannotPathToTarget".Translate(), null);
            }

            return new FloatMenuOption("JDG_ShatterShard".Translate(clickedThing.Label, clickedThing.stackCount * 50f), delegate
            {
                Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_ShatterShard, new LocalTargetInfo(clickedThing));
                context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            });
        }
    }
}
