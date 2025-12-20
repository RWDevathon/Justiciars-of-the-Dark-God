using RimWorld;
using Verse;

namespace ArtificialBeings
{
    // Motivation to recruit means the justiciar wants to raise a new justiciar (one who didn't have the trait already). It fails if the ambition expires before they have a chance to do so.
    // The raise justiciar job notifies this hediff directly of success when completed.
    public class Hediff_Ambition_RecruitmentMotivation : Hediff_Ambition
    {

        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (GenTicks.TicksGame > expirationTick)
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

        // On failure, the justiciar has a minor breakdown and falls into despair for a little while.
        public override void NotifyFailed()
        {
            pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Wander_Sad, reason: "ABF_AmbitionFailed".Translate(), forced: true);
            Hediff crushingDespair = HediffMaker.MakeHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_CrushingDespair, pawn);
            pawn.health.AddHediff(crushingDespair);
            pawn.health.RemoveHediff(this);
        }

        // On success, the justiciar should receive a longer-term bonus. It is expected that the bonus is unlocked by setting the Severity to 1.
        public override void NotifySucceeded()
        {
            base.NotifySucceeded();
            complete = true;
            Severity = 1f;
            expirationTick = Extension.expirationTicks.RandomInRange;
            Find.LetterStack.ReceiveLetter("ABF_AmbitionSucceeded".Translate(), "ABF_AmbitionSucceeded_RecruitmentMotivation".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.PositiveEvent);
        }
    }
}
