using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class JobDriver_RaiseJusticiar : JobDriver_JusticiarBase
    {
        public Corpse Corpse => TargetThing as Corpse;

        protected override void EndInitAction()
        {
            Pawn justiciar = Corpse.InnerPawn;
            ResurrectionUtility.TryResurrect(justiciar);
            justiciar.drafter.Drafted = true; // Try to keep the justiciar still on spawn so they don't immediately run off.
            // This costs the newly resurrected justiciar 100 favor. If they have less than 100, the remainder is taken from the resurrector.
            Hediff_Justiciar justiciarHediff = JDG_Utils.GetJusticiarHediff(justiciar);
            if (justiciarHediff.FavorCurrent < 100f)
            {
                Hediff_Justiciar casterHediff = JDG_Utils.GetJusticiarHediff(pawn);
                float remainder = 100f - justiciarHediff.FavorCurrent;
                casterHediff.NotifyFavorLost(remainder);
                justiciarHediff.NotifyFavorLost(justiciarHediff.FavorCurrent);
            }
            else
            {
                justiciarHediff.NotifyFavorLost(100f);
            }
        }
    }
}
