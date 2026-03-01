using Verse;

namespace ArtificialBeings
{
    public class ThingCompProperties_MaintainsDarkness : CompProperties
    {
        public float radius = 1.9f;

        public ThingCompProperties_MaintainsDarkness()
        {
            compClass = typeof(ThingComp_MaintainsDarkness);
        }
    }
}
