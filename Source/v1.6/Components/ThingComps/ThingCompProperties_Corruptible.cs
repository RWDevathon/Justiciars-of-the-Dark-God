using Verse;

namespace ArtificialBeings
{
    public class ThingCompProperties_Corruptible : CompProperties
    {
        public ThingDef defToSpawn;

        // If true, the stack count of the thing spawned will match the stack count of the thing corrupted. Otherwise, only 1 will ever spawn.
        public bool matchStackCount = true;

        public ThingCompProperties_Corruptible()
        {
            compClass = typeof(ThingComp_Corruptible);
        }
    }
}
