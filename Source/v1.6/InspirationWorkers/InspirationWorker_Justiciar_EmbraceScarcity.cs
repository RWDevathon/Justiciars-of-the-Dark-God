using RimWorld;
using Verse;

namespace ArtificialBeings
{
    // This inspiration should only occur if the justiciar is not currently experiencing malnutrition (and needs food and mood in the first place).
    public class InspirationWorker_Justiciar_EmbraceScarcity : InspirationWorker_Justiciar
    {
        public override bool InspirationCanOccur(Pawn pawn)
        {
            return base.InspirationCanOccur(pawn) && pawn.needs?.mood != null && pawn.needs?.food != null && !pawn.health.hediffSet.hediffs.Any(hediff => hediff.def == HediffDefOf.Malnutrition);
        }
    }
}
