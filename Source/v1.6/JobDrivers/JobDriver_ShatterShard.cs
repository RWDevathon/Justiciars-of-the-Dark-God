namespace ArtificialBeings
{
    public class JobDriver_ShatterShard : JobDriver_JusticiarBase
    {

        protected override void EndInitAction()
        {
            pawn.health.hediffSet.GetFirstHediff<Hediff_Devotee>()?.NotifyFavorGained(TargetThing.stackCount * 50f);
            TargetThing.Destroy();
        }
    }
}
