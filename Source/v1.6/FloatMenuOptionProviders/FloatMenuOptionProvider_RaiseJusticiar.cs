using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class FloatMenuOptionProvider_RaiseJusticiar : FloatMenuOptionProvider
    {
        protected override bool Drafted => true;

        protected override bool Undrafted => true;

        protected override bool Multiselect => false;

        protected override bool IgnoreFogged => false;

        // Can only target the corpses of justiciars. Only justiciars who have accrued at least 100 favor over their lifetime can raise others.
        public override bool TargetThingValid(Thing thing, FloatMenuContext context)
        {
            if (!(thing is Corpse corpse) || corpse.InnerPawn.Faction != Faction.OfPlayerSilentFail || !JDG_Utils.IsJusticiar(corpse.InnerPawn) || !JDG_Utils.IsJusticiar(context.FirstSelectedPawn)) {
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

            Hediff_Justiciar corpseHediff = corpse.InnerPawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>();
            Hediff_Justiciar casterHediff = context.FirstSelectedPawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>();
            if (corpseHediff.FavorCurrent + casterHediff.FavorCurrent < 100f)
            {
                return new FloatMenuOption("JDG_InsufficientFavorToRaise".Translate(corpse.InnerPawn.NameShortColored, context.FirstSelectedPawn.NameShortColored), null);
            }

            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("JDG_RaiseJusticiar".Translate(corpse.InnerPawn.NameShortColored), delegate
            {
                Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_Raise, new LocalTargetInfo(corpse));
                context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }, MenuOptionPriority.High), context.FirstSelectedPawn, corpse);
        }
    }
}
