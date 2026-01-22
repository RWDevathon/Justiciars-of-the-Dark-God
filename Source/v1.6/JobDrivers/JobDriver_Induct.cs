using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class JobDriver_Induct : JobDriver_JusticiarBase
    {
        public Pawn Inductee => TargetThing as Pawn;

        protected override void WaitInitAction()
        {
            PawnUtility.ForceWait(Inductee, WaitTicks, pawn);
        }

        protected override void EndInitAction()
        {
            float costToInduct = JDG_Utils.FavorCostToInduct(Inductee);
            if (Inductee.RaceProps.Humanlike)
            {
                Inductee.health.AddHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_Acolyte);
                // Inducting a new acolyte can fulfill an ambition for the person who inducted them.
                if (pawn.health.hediffSet.TryGetHediff<Hediff_Ambition_RecruitmentMotivation>(out var hediff) && !hediff.complete)
                {
                    hediff.NotifySucceeded();
                }
                // The inductee views the justiciar as an arbiter. The justiciars views the new acolyte as their neophyte.
                Inductee.relations.AddDirectRelation(JDG_PawnRelationDefOf.ABF_PawnRelation_Justiciar_Arbiter, pawn);
                pawn.relations.AddDirectRelation(JDG_PawnRelationDefOf.ABF_PawnRelation_Justiciar_Neophyte, Inductee);
            }
            else
            {
                Inductee.health.AddHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_ShadeSpirit);
            }
            pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>().NotifyFavorLost(costToInduct);
        }
    }
}
