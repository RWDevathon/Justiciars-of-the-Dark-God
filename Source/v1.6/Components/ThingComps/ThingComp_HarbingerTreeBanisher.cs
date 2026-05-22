using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    // This provides float menu options for banning harbinger tree spawn incidents for the next year (or extending the current timer by one year if already protected).
    public class ThingComp_HarbingerTreeBanisher : ThingComp
    {
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (JDG_Utils.IsJusticiar(selPawn))
            {
                Hediff_Justiciar justiciarHediff = JDG_Utils.GetJusticiarHediff(selPawn);
                if (justiciarHediff.FavorCurrent < JDG_Utils.favorCostToAbjure)
                {
                    yield return new FloatMenuOption("JDG_InsufficientFavorToAbjureHarbinger".Translate(JDG_Utils.favorCostToAbjure.ToString("F0")), null);
                }
                else
                {
                    yield return new FloatMenuOption("JDG_AbjureHarbinger".Translate(JDG_Utils.favorCostToAbjure.ToString("F0")), delegate
                    {
                        Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_AbjureHarbinger, parent);
                        selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    });
                }
            }
            yield break;
        }
    }
}
