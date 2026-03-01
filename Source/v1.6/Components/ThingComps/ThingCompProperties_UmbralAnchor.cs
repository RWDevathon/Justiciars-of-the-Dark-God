using Verse;

namespace ArtificialBeings
{
    public class ThingCompProperties_UmbralAnchor : CompProperties
    {
        public float daysBetweenSpawns = 15f;

        public ThingCompProperties_UmbralAnchor()
        {
            compClass = typeof(ThingComp_UmbralAnchor);
        }
    }
}
