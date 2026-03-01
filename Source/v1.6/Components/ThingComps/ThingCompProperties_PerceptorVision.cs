using Verse;

namespace ArtificialBeings
{
    public class ThingCompProperties_PerceptorVision : CompProperties
    {
        public IntRange daysBetweenScrying;

        public int plantRatioForOptimalDetection = 50;

        public ThingCompProperties_PerceptorVision()
        {
            compClass = typeof(ThingComp_PerceptorVision);
        }
    }
}
