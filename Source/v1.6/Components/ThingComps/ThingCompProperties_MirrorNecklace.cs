using Verse;

namespace ArtificialBeings
{
    public class ThingCompProperties_MirrorNecklace : CompProperties
    {
        public HediffDef hediff;

        public BodyPartGroupDef bodyPartGroup;

        public ThingCompProperties_MirrorNecklace()
        {
            compClass = typeof(ThingComp_MirrorNecklace);
        }
    }
}
