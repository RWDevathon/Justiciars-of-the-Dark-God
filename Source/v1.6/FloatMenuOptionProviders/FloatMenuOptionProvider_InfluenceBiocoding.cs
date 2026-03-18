using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class FloatMenuOptionProvider_InfluenceBiocoding : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool IgnoreFogged => false;

        public override bool TargetThingValid(Thing thing, FloatMenuContext context)
        {
            if (!JDG_Utils.IsJusticiar(context.FirstSelectedPawn) || thing.TryGetComp<CompBiocodable>() == null) {
                return false;
            }
            return base.TargetThingValid(thing, context);
        }

        protected override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
        {
            if (!(clickedThing.TryGetComp<CompBiocodable>() is CompBiocodable compBiocodable) || compBiocodable.CodedPawn == context.FirstSelectedPawn)
            {
                return null;
            }

            if (!context.FirstSelectedPawn.CanReserveAndReach(clickedThing, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, ignoreOtherReservations: true))
            {
                return new FloatMenuOption("JDG_CannotPathToTarget".Translate(), null);
            }

            // Costs twice as much if stripping the coding away from another pawn.
            int favorCost = (compBiocodable.CodedPawn != null) ? 10 : 5;
            if (context.FirstSelectedPawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>().FavorCurrent < favorCost)
            {
                return new FloatMenuOption("JDG_InsufficientFavorToInfluenceBiocode".Translate(favorCost.ToString()), null);
            }

            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("JDG_InfluenceBiocoding".Translate(clickedThing.Label, favorCost.ToString()), delegate
            {
                Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_InfluenceBiocoding, new LocalTargetInfo(clickedThing));
                context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }, MenuOptionPriority.High), context.FirstSelectedPawn, clickedThing);
        }
    }
}
