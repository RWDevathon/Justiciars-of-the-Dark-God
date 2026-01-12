using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class InspirationWorker_Justiciar : InspirationWorker
    {
        public override bool InspirationCanOccur(Pawn pawn)
        {
            if (!JDG_Utils.IsJusticiar(pawn))
            {
                return false;
            }
            return base.InspirationCanOccur(pawn);
        }
    }
}
