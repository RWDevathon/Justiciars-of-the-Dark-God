using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    // This turns the parent Thing into a silent trap waiting for a justiciar to activate it.
    public class ThingComp_MaliciousSpirit : ThingComp
    {
        public int readyTick = 0;

        public override void PostPostMake()
        {
            base.PostPostMake();
            readyTick = GenTicks.TicksGame + GenDate.TicksPerTwelfth;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref readyTick, "JDG_readyTick", 0);
        }

        public override string CompInspectStringExtra()
        {
            if (readyTick <= GenTicks.TicksGame)
            {
                return "JDG_MaliciousSpirit_Ready".Translate();
            }
            return "JDG_MaliciousSpirit_Preparing".Translate((readyTick - GenTicks.TicksGame).ToStringTicksToPeriod());
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (JDG_Utils.IsJusticiar(selPawn))
            {
                if (readyTick <= GenTicks.TicksGame)
                {
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("JDG_AwakenMaliciousSpirit".Translate(), delegate
                    {
                        Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_AwakenMaliciousSpirit, parent);
                        selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    }), selPawn, parent);
                }
                else
                {
                    yield return new FloatMenuOption("JDG_MaliciousSpirit_Preparing".Translate((readyTick - GenTicks.TicksGame).ToStringTicksToPeriod()), null);
                }
            }
            yield break;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action ready = new Command_Action
                {
                    defaultLabel = "DEV: Ready",
                    action = delegate
                    {
                        readyTick = GenTicks.TicksGame;
                    }
                };
                yield return ready;
            }
        }
    }
}
