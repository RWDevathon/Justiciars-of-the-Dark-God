using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class InspirationWorker_Justiciar : InspirationWorker
    {
        public override bool InspirationCanOccur(Pawn pawn)
        {
            if (pawn.story?.traits?.HasTrait(JDG_TraitDefOf.ABF_Trait_Justiciar_Adherent) != true)
            {
                return false;
            }
            return base.InspirationCanOccur(pawn);
        }
    }
}
