using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class JobDriver_InfluenceBiocoding : JobDriver_JusticiarBase
    {
        protected override void EndInitAction()
        {
            if (TargetThing.TryGetComp<CompBiocodable>() is CompBiocodable compBiocodable)
            {
                float favorCostToBiocode = 5f;
                if (compBiocodable.CodedPawn != null)
                {
                    favorCostToBiocode = 10f;
                    compBiocodable.UnCode();
                }
                compBiocodable.CodeFor(pawn);
                pawn.health.hediffSet.GetFirstHediff<Hediff_Devotee>()?.NotifyFavorLost(favorCostToBiocode);
            }
        }
    }
}
