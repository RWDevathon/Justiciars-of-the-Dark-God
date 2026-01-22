using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    // This provides float menu options for opening communications with other justiciars/acolytes
    public class ThingComp_JusticiarComms : ThingComp
    {
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (JDG_Utils.IsJusticiar(selPawn))
            {
                Hediff_Justiciar justiciarHediff = selPawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>();
                if (justiciarHediff.FavorTotalAccrued >= 100f)
                {
                    if (justiciarHediff.FavorCurrent < 60f)
                    {
                        yield return new FloatMenuOption("JDG_InsufficientFavorToRecruit".Translate(), null);
                    }
                    else
                    {
                        yield return new FloatMenuOption("JDG_RecruitAcolyte".Translate(), delegate
                        {
                            Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_SummonAcolyte, parent);
                            selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                        });
                    }
                }
            }
            yield break;
        }
    }
}
