using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace ArtificialBeings
{
    public class ThingComp_Corruptible : ThingComp
    {
        private static TargetingParameters TargetingParams => new TargetingParameters
        {
            canTargetPawns = true,
            canTargetLocations = false
        };

        public ThingCompProperties_Corruptible Props => (ThingCompProperties_Corruptible)props;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action
            {
                defaultLabel = "JDG_CorruptThing".Translate(parent.def),
                defaultDesc = "JDG_CorruptThingDesc".Translate(parent.def, Props.defToSpawn),
                icon = Props.defToSpawn.uiIcon,
                action = BeginTargeting
            };
        }

        private void BeginTargeting()
        {
            Find.Targeter.BeginTargeting(TargetingParams, delegate (LocalTargetInfo t)
            {
                if (ValidateTarget(t))
                {
                    Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_CorruptThing, parent);
                    ((Pawn)t.Thing).jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                }
                else
                {
                    BeginTargeting();
                }
            }, delegate (LocalTargetInfo t)
            {
                if (ValidateTarget(t))
                {
                    GenDraw.DrawTargetHighlight(t);
                }
            }, (LocalTargetInfo t) => true, null, null, Props.defToSpawn.uiIcon, playSoundOnAction: false);
        }

        public bool ValidateTarget(LocalTargetInfo target)
        {
            return target.Thing is Pawn pawn && JDG_Utils.IsJusticiar(pawn);
        }
    }
}
