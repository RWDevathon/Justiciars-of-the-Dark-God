using Verse;

namespace ArtificialBeings
{
    public interface IJusticiarMaintainable : ILoadReferenceable
    {
        Thing Maintainer { get; set; }

        void Maintain(int ticks);

        void Terminate();
    }
}
