using Verse;

namespace ArtificialBeings
{
    // This tracker is retained in a global static cache, keyed by a pawn's thingId.
    public class PerceptorTracker : IExposable
    {
        public int tickPerceived;

        public PerceptorTracker(int tickCreated)
        {
            tickPerceived = tickCreated;
        }

        public PerceptorTracker()
        {
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref tickPerceived, "tickPerceived");
        }
    }
}
