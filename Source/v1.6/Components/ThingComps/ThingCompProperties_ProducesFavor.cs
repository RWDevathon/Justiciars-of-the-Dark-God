using Verse;

namespace ArtificialBeings
{
    public class ThingCompProperties_ProducesFavor : CompProperties
    {
        public float favorPerDay;

        public ThingCompProperties_ProducesFavor()
        {
            compClass = typeof(ThingComp_ProducesFavor);
        }
    }
}
