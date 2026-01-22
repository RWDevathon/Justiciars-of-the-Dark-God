using System;
using Verse;

namespace ArtificialBeings
{
    public interface IJusticiarMaintainable : ILoadReferenceable
    {
        Hediff_Justiciar Maintainer { get; set; }

        void Maintain(int ticks);

        void Terminate();
    }
}
