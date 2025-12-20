namespace ArtificialBeings
{
    // Base class to control logic for Justiciar Ambitions specifically.
    public abstract class Hediff_Ambition : Hediff_MentalTracker
    {
        // Ambitions should be treated as if a mental break occurred and completed when they are created to prevent back-to-back ambition assignments.
        public override void PostMake()
        {
            base.PostMake();
            pawn.mindState.mentalBreaker.Notify_RecoveredFromMentalState();
        }

        public override void NotifySucceeded()
        {
            if (pawn.Inspiration is Inspiration_Justiciar_SeekRepentance repentance)
            {
                repentance.NotifyAmbitionComplete();
            }
        }
    }
}
