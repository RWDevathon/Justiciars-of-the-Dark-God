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

        public override bool TargetThingValid(Thing thing, FloatMenuContext context)
        {
            if (!(thing is Corpse corpse) || corpse.InnerPawn.Faction != Faction.OfPlayerSilentFail) {
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

            if (ABF_Utils.IsArtificial(corpse.InnerPawn))
            {
                return new FloatMenuOption("ABF_CannotRaiseArtificial".Translate(corpse.InnerPawn), null);
            }

            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("ABF_RaiseJusticiar".Translate(corpse.InnerPawn.NameShortColored), delegate
            {
                Job job = new Job(JDG_JobDefOf.ABF_Job_Justiciar_Raise, new LocalTargetInfo(corpse));
                context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }, MenuOptionPriority.High), context.FirstSelectedPawn, corpse);
        }
    }
}
