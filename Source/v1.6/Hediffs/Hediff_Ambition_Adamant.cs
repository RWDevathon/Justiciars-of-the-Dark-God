using RimWorld;
using System.Linq;
using Verse;

namespace ArtificialBeings
{
    // Adamant means the justiciar is attempting to avoid having any other ambitions or inspirations. It fails if any occur before it expires, and succeeds otherwise.
    public class Hediff_Ambition_Adamant : Hediff_Ambition
    {
        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (!complete && (pawn.health.hediffSet.hediffs.Any(hediff => hediff != this && hediff is Hediff_MentalTracker tracker && !tracker.complete) || pawn.Inspired))
            {
                NotifyFailed();
            }
            else if (TicksRemaining <= 0)
            {
                if (!complete)
                {
                    NotifySucceeded();
                }
                else
                {
                    pawn.health.RemoveHediff(this);
                }
            }
        }

        // On failure, the justiciar suffers crushing despair 10 times longer than its normal duration.
        public override void NotifyFailed()
        {
            Hediff crushingDespair = HediffMaker.MakeHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_CrushingDespair, pawn);
            if (crushingDespair.TryGetComp<HediffComp_Disappears>() is HediffComp_Disappears comp)
            {
                comp.SetDuration(comp.disappearsAfterTicks * 10);
            }
            pawn.health.AddHediff(crushingDespair);
            pawn.health.RemoveHediff(this);
            Find.LetterStack.ReceiveLetter("JDG_AmbitionFailed".Translate(), "JDG_AmbitionFailed_Adamant".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NegativeEvent);
            pawn.mindState.mentalBreaker.Notify_RecoveredFromMentalState();
        }

        // On success, the justiciar should receive a longer-term bonus. It is expected that the bonus is unlocked by setting the Severity to 1.
        // The bonus effect will last a length of time roughly twice as long as the ambition did.
        public override void NotifySucceeded()
        {
            base.NotifySucceeded();
            complete = true;
            Severity = 1f;
            expirationTick = Extension.expirationTicks.RandomInRange * 2;
            Find.LetterStack.ReceiveLetter("JDG_AmbitionSucceeded".Translate(), "JDG_AmbitionSucceeded_Adamant".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.PositiveEvent);

            // Completing this ambition grants favor.
            pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>()?.NotifyFavorGained(40f);
        }
    }
}
