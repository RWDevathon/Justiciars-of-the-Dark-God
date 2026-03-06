using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class ThingComp_ProducesFavor : ThingComp
    {
        public ThingCompProperties_ProducesFavor Props => (ThingCompProperties_ProducesFavor)props;

        public float currentFavor = 0;

        public override void CompTickLong()
        {
            currentFavor += Props.favorPerDay * GenTicks.TickLongInterval / GenDate.TicksPerDay;
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("JDG_ReapFavor".Translate(currentFavor.ToString("F2")), delegate
            {
                Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_ReapFavor, parent);
                selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }), selPawn, parent);
        }

        public override string CompInspectStringExtra()
        {
            return "JDG_AcolyteFavor".Translate(currentFavor.ToString("F2"));
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref currentFavor, "JDG_currentFavor", 0f);
        }
    }
}
