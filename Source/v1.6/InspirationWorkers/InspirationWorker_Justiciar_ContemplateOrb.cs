using Verse;

namespace ArtificialBeings
{
    // This inspiration should only occur if the justiciar has an obsidian orb on the same map.
    public class InspirationWorker_Justiciar_ContemplateOrb : InspirationWorker_Justiciar
    {
        public override bool InspirationCanOccur(Pawn pawn)
        {
            return base.InspirationCanOccur(pawn) && pawn.MapHeld != null && !pawn.MapHeld.listerThings.ThingsOfDef(JDG_ThingDefOf.ABF_Thing_Justiciar_Orb).NullOrEmpty();
        }
    }
}
