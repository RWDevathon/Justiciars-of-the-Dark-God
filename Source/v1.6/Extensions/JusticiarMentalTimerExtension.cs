using Verse;

namespace ArtificialBeings
{
    // Mod extension applied to HediffDefs so the class can check/apply the correct information
    public class JusticiarMentalTimerExtension : DefModExtension
    {
        // Range controlling how long an ambition/inspiration will last before it expires.
        public IntRange expirationTicks;
    }
}
