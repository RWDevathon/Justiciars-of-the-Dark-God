using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class FloatMenuOptionProvider_DoManualScrying : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool IgnoreFogged => false;

        public override bool TargetPawnValid(Pawn pawn, FloatMenuContext context)
        {
            if (context.FirstSelectedPawn?.kindDef == JDG_PawnKindDefOf.ABF_PawnKind_Justiciar_Player_CreaturePerceptor && !pawn.Dead && pawn.RaceProps.Humanlike && !pawn.HostileTo(context.FirstSelectedPawn))
            {
                return true;
            }
            return false;
        }

        protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
        {
            if (context.FirstSelectedPawn?.GetComp<ThingComp_PerceptorVision>() is ThingComp_PerceptorVision perceptorVision)
            {
                if (perceptorVision.ticksToNextManualScrying <= 0)
                {
                    if (!context.FirstSelectedPawn.CanReserveAndReach(clickedPawn, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, ignoreOtherReservations: true))
                    {
                        return new FloatMenuOption("JDG_CannotPathToTarget".Translate(), null);
                    }

                    return new FloatMenuOption("JDG_DoManualScrying".Translate(clickedPawn.NameShortColored), delegate
                    {
                        Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_DoManualScrying, new LocalTargetInfo(clickedPawn));
                        context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    });
                }
                return new FloatMenuOption("JDG_CannotScryOnCooldown".Translate(perceptorVision.ticksToNextManualScrying.ToStringTicksToPeriod()), null);
            }
            return null;
        }
    }
}
