using RimWorld;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    public class JobDriver_DominateAnimal : JobDriver_JusticiarBase
    {
        protected override int WaitTicks => Mathf.RoundToInt(GenDate.TicksPerHour * Subject.BodySize);

        public Pawn Subject => TargetThing as Pawn;

        protected override void WaitInitAction()
        {
            PawnUtility.ForceWait(Subject, WaitTicks, pawn);
        }

        protected override void EndInitAction()
        {
            float costToDominate = JDG_Utils.FavorCostToDominate(Subject);
            InteractionWorker_RecruitAttempt.DoRecruit(pawn, Subject);
            pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>().NotifyFavorLost(costToDominate);
        }
    }
}
