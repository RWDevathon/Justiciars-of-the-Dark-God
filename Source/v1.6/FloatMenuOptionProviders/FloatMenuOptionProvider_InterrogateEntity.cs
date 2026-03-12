using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class FloatMenuOptionProvider_InterrogateEntity : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool IgnoreFogged => false;

        protected override bool RequiresManipulation => true;

        protected override bool AppliesInt(FloatMenuContext context)
        {
            return ModsConfig.AnomalyActive;
        }

        public override bool TargetPawnValid(Pawn pawn, FloatMenuContext context)
        {
            if (!JDG_Utils.IsJusticiar(context.FirstSelectedPawn) || !pawn.IsEntity)
            {
                return false;
            }
            return base.TargetPawnValid(pawn, context);
        }

        protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
        {
            if (!context.FirstSelectedPawn.CanReserveAndReach(clickedPawn, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, ignoreOtherReservations: true))
            {
                return new FloatMenuOption("JDG_CannotPathToTarget".Translate(), null);
            }

            if (!clickedPawn.TryGetComp(out CompHoldingPlatformTarget holdComp) || !holdComp.CanBeCaptured)
            {
                return null;
            }

            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("JDG_InterrogateEntity".Translate(clickedPawn.NameShortColored), delegate
            {
                Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_InterrogateEntity, new LocalTargetInfo(clickedPawn));
                context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }, MenuOptionPriority.High), context.FirstSelectedPawn, clickedPawn);
        }
    }
}
