using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class FloatMenuOptionProvider_StealFavor : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool IgnoreFogged => false;

        public override bool TargetThingValid(Thing thing, FloatMenuContext context)
        {
            if (!(thing is Corpse corpse) || (!JDG_Utils.IsJusticiar(corpse.InnerPawn) && !JDG_Utils.IsAcolyte(corpse.InnerPawn))
                || (!JDG_Utils.IsJusticiar(context.FirstSelectedPawn)) && !JDG_Utils.IsAcolyte(context.FirstSelectedPawn)) {
                return false;
            }
            return base.TargetThingValid(thing, context);
        }

        protected override FloatMenuOption GetSingleOptionFor(Thing clickedThing, FloatMenuContext context)
        {
            if (!(clickedThing is Corpse corpse))
            {
                return null;
            }

            // Favor can only be stolen if there is any to take.
            float? toTake = (corpse.InnerPawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>()?.FavorCurrent) ?? (corpse.InnerPawn.health.hediffSet.GetFirstHediff<Hediff_Acolyte>()?.favorCurrent);
            if (!toTake.HasValue || toTake <= 0f)
            {
                return null;
            }

            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("JDG_StealFavor".Translate(corpse.InnerPawn.NameShortColored, ((float)toTake).ToString("F2")), delegate
            {
                Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_StealFavor, new LocalTargetInfo(corpse));
                context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }, MenuOptionPriority.High), context.FirstSelectedPawn, corpse);
        }
    }
}
