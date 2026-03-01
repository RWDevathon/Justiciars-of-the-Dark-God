using Verse;

namespace ArtificialBeings
{
    public class JobDriver_CorruptThing : JobDriver_JusticiarBase
    {

        protected override void EndInitAction()
        {
            ThingComp_Corruptible corruptibleComp = TargetThing.TryGetComp<ThingComp_Corruptible>();
            Thing thing = ThingMaker.MakeThing(corruptibleComp.Props.defToSpawn);
            if (corruptibleComp.Props.matchStackCount)
            {
                thing.stackCount = TargetThing.stackCount;
            }
            IntVec3 location = TargetThing.Position;
            Map map = TargetThing.Map;
            TargetThing.Destroy();
            GenSpawn.Spawn(thing, location, map);
        }
    }
}
