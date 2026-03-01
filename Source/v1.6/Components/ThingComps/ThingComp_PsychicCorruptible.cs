using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    // This provides float menu options for having a justiciar corrupt the parent Thing, which is expected to be a non-moving object with health.
    public class ThingComp_PsychicCorruptible : ThingComp
    {
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (JDG_Utils.IsJusticiar(selPawn))
            {
                yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("JDG_PillagePsychicEnergy".Translate(), delegate
                {
                    Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_PillagePsychicEnergy, parent);
                    selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                }), selPawn, parent);

                if (selPawn.HasPsylink)
                {
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("JDG_SiphonPsychicEnergy".Translate(), delegate
                    {
                        Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_SiphonPsychicEnergy, parent);
                        selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    }), selPawn, parent);
                }
            }
            yield break;
        }
    }
}
