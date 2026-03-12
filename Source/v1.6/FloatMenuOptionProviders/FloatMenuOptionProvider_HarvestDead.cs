using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class FloatMenuOptionProvider_HarvestDead : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool IgnoreFogged => false;

        public override bool TargetThingValid(Thing thing, FloatMenuContext context)
        {
            if (context.FirstSelectedPawn?.kindDef == JDG_PawnKindDefOf.ABF_PawnKind_Justiciar_Player_CreatureHerald && ((thing is Corpse corpse && corpse.InnerPawn.RaceProps.Humanlike) || thing.def == ThingDefOf.Skull)) {
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

            Corpse corpse = clickedThing as Corpse;

            return new FloatMenuOption("JDG_HarvestDead".Translate(corpse?.InnerPawn.NameShortColored ?? clickedThing.Label), delegate
            {
                Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_HarvestDead, new LocalTargetInfo(clickedThing));
                context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            });
        }
    }
}
