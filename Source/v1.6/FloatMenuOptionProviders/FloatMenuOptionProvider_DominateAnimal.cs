using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class FloatMenuOptionProvider_DominateAnimal : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool IgnoreFogged => false;

        public override bool TargetPawnValid(Pawn pawn, FloatMenuContext context)
        {
            if (JDG_Utils.IsJusticiar(context.FirstSelectedPawn) && pawn.IsAnimal && pawn.Faction == null && !pawn.InMentalState && !pawn.HostileTo(context.FirstSelectedPawn))
            {
                return true;
            }
            return false;
        }

        protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
        {
            if (clickedPawn.GetStatValue(StatDefOf.Wildness) > 0.95f)
            {
                return new FloatMenuOption("JDG_CannotDominateAnimalTooMuchWildness".Translate(), null);
            }

            if (context.FirstSelectedPawn?.health.hediffSet.GetFirstHediff<Hediff_Justiciar>() is Hediff_Justiciar justiciarHediff)
            {
                if (!context.FirstSelectedPawn.CanReserveAndReach(clickedPawn, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, ignoreOtherReservations: true))
                {
                    return new FloatMenuOption("JDG_CannotPathToTarget".Translate(), null);
                }

                float favorToDominate = JDG_Utils.FavorCostToDominate(clickedPawn);
                if (justiciarHediff.FavorCurrent >= favorToDominate)
                {
                    return new FloatMenuOption("JDG_DominateAnimal".Translate(clickedPawn.NameShortColored, favorToDominate), delegate
                    {
                        Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_DominateAnimal, new LocalTargetInfo(clickedPawn));
                        context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    });
                }
                return new FloatMenuOption("JDG_CannotDominateAnimalInsufficientFavor".Translate(favorToDominate), null);
            }
            return null;
        }
    }
}
