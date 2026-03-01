using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class JobDriver_SummonCreature : JobDriver_JusticiarBase
    {
        protected override int WaitTicks => GenDate.TicksPerHour * 4;

        protected override void EndInitAction()
        {
            ThingComp_UmbralAnchor umbralAnchorComp = TargetThing.TryGetComp<ThingComp_UmbralAnchor>();
            Pawn creature = PawnGenerator.GeneratePawn(umbralAnchorComp.nextToSpawn, Faction.OfPlayer);
            GenSpawn.Spawn(creature, TargetThing.Position, pawn.Map);
            if (umbralAnchorComp.currentCreature != null)
            {
                umbralAnchorComp.currentCreature.Destroy();
            }
            umbralAnchorComp.currentCreature = creature;
            umbralAnchorComp.ResetTimer();
        }
    }
}
