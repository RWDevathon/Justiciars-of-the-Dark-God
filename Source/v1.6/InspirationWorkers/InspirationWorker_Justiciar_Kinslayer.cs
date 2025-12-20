using System.Linq;
using Verse;

namespace ArtificialBeings
{
    // This inspiration should only occur if the justiciar has kin around.
    public class InspirationWorker_Justiciar_Kinslayer : InspirationWorker_Justiciar
    {
        public override bool InspirationCanOccur(Pawn pawn)
        {
            return base.InspirationCanOccur(pawn) && pawn.relations.FamilyByBlood.Any(kin => kin.MapHeld == pawn.MapHeld && !kin.Dead);
        }
    }
}
