using RimWorld;
using Verse;

namespace ArtificialBeings
{
    // Craving inspiration means the justiciar wants to acquire an inspiration. It fails if the ambition expires before they have a chance to do so.
    public class Hediff_Ambition_CravingInspiration : Hediff_Ambition
    {
        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (!complete && pawn.Inspired)
            {
                NotifySucceeded();
            }
            else if (TicksRemaining <= 0)
            {
                if (!complete)
                {
                    NotifyFailed();
                }
                else
                {
                    pawn.health.RemoveHediff(this);
                }
            }
        }

        // On failure, the justiciar falls into despair for a long while.
        public override void NotifyFailed()
        {
            Hediff crushingDespair = HediffMaker.MakeHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_CrushingDespair, pawn);
            if (crushingDespair.TryGetComp<HediffComp_Disappears>() is HediffComp_Disappears comp)
            {
                comp.SetDuration(Extension.expirationTicks.RandomInRange);
            }
            pawn.health.AddHediff(crushingDespair);
            pawn.health.RemoveHediff(this);
            Find.LetterStack.ReceiveLetter("ABF_AmbitionFailed".Translate(), "ABF_AmbitionFailed_CravingInspiration".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NegativeEvent);
            pawn.mindState.mentalBreaker.Notify_RecoveredFromMentalState();
        }

        // On success, the justiciar should receive a longer-term bonus. It is expected that the bonus is unlocked by setting the Severity to 1.
        public override void NotifySucceeded()
        {
            // This does not notify the parent class on completion to avoid notifying ambitions/inspirations about something which is mutually exclusive with them.
            complete = true;
            Severity = 1f;
            expirationTick = Extension.expirationTicks.RandomInRange;
            Find.LetterStack.ReceiveLetter("ABF_AmbitionSucceeded".Translate(), "ABF_AmbitionSucceeded_CravingInspiration".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.PositiveEvent);
        }
    }
}
