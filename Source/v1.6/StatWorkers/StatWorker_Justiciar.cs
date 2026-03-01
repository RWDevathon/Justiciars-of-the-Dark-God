using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class StatWorker_Justiciar : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            return req.HasThing && req.Thing is Pawn pawn && (JDG_Utils.IsJusticiar(pawn) || JDG_Utils.IsAcolyte(pawn));
        }
    }
}
