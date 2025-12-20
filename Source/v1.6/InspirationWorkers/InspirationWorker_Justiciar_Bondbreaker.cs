using RimWorld;
using Verse;

namespace ArtificialBeings
{
    // This inspiration should only occur if the justiciar has a bond or strong friendship.
    public class InspirationWorker_Justiciar_Bondbreaker : InspirationWorker_Justiciar
    {
        public override bool InspirationCanOccur(Pawn pawn)
        {
            if (!base.InspirationCanOccur(pawn))
            {
                return false;
            }

            if (pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Bond) != null)
            {
                return true;
            }

            foreach (Pawn relation in pawn.relations.RelatedPawns)
            {
                if (pawn.relations.OpinionOf(relation) > 75)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
