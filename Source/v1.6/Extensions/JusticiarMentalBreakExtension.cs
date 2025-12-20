using Verse;

namespace ArtificialBeings
{
    // Mod extension applied to MentalBreakDefs and InspirationDefs so the worker can check/apply the correct information
    public class JusticiarMentalBreakExtension : DefModExtension
    {
        // This hediff def is the tracker for the ambition/inspiration of the pawn while it is active, and controls most of the behavior.
        public HediffDef associatedHediffDef;
    }
}
