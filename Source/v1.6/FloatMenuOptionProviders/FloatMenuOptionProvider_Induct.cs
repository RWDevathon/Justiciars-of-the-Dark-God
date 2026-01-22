using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class FloatMenuOptionProvider_Induct : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool IgnoreFogged => false;

        public override bool TargetPawnValid(Pawn pawn, FloatMenuContext context)
        {
            if (JDG_Utils.IsJusticiar(pawn) || JDG_Utils.IsAcolyte(pawn) || JDG_Utils.IsShadeSpirit(pawn) || !JDG_Utils.IsJusticiar(context.FirstSelectedPawn)
                || pawn.Faction != context.FirstSelectedPawn.Faction)
            {
                return false;
            }
            return base.TargetPawnValid(pawn, context);
        }

        protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
        {
            Pawn justiciar = context.FirstSelectedPawn;
            float favorCostToInduct = JDG_Utils.FavorCostToInduct(clickedPawn);

            if (favorCostToInduct <= 0f)
            {
                return null;
            }

            if (justiciar.health.hediffSet.GetFirstHediff<Hediff_Justiciar>()?.FavorCurrent < favorCostToInduct)
            {
                return new FloatMenuOption("JDG_InsufficientFavorToInduct".Translate(favorCostToInduct.ToString("F2")), null);
            }

            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("JDG_Induct".Translate(clickedPawn.NameShortColored, favorCostToInduct.ToString("F2")), delegate
            {
                Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_Induct, new LocalTargetInfo(clickedPawn));
                context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }, MenuOptionPriority.High), context.FirstSelectedPawn, clickedPawn);
        }
    }
}
