using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class Inspiration_Justiciar_InduceWithdrawal : Inspiration
    {
        public int ticksSinceWithdrawalBegan = 0;

        public bool complete = false;

        public override void InspirationTick(int delta)
        {
            if (pawn.health.hediffSet.hediffs.Any(hediff => hediff is Hediff_Addiction addiction && addiction.CurStageIndex > 0))
            {
                ticksSinceWithdrawalBegan += delta;
                if (ticksSinceWithdrawalBegan > GenDate.TicksPerDay)
                {
                    complete = true;
                    End();
                }
            }
            else
            {
                ticksSinceWithdrawalBegan = 0;
            }
            // Handles expiration
            base.InspirationTick(delta);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksSinceWithdrawalBegan, "ABF_ticksSinceWithdrawalBegan");
        }

        public override void PostEnd()
        {
            if (complete)
            {
                Find.LetterStack.ReceiveLetter("JDG_InspirationSucceeded".Translate(), "JDG_InspirationSucceeded_InduceWithdrawal".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NeutralEvent);
                pawn.mindState.mentalBreaker.Notify_RecoveredFromMentalState();
            }
            else
            {
                Hediff crushingDespair = HediffMaker.MakeHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_CrushingDespair, pawn);
                if (crushingDespair.TryGetComp<HediffComp_Disappears>() is HediffComp_Disappears comp)
                {
                    comp.SetDuration(comp.disappearsAfterTicks * 10);
                }
                pawn.health.AddHediff(crushingDespair);
                pawn.mindState.mentalStateHandler.TryStartMentalState(JDG_MentalStateDefOf.Binging_DrugExtreme, reason: "JDG_InspirationFailed".Translate(), forced: true);
            }
        }

        public override void PostStart(bool sendLetter = true)
        {
            base.PostStart(sendLetter);

            // Receiving an inspiration results in favor loss.
            pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>()?.NotifyFavorLost(25f);

            // If the player has not yet learned about inspirations, they will also receive a learning helper tip about how they work.
            LessonAutoActivator.TeachOpportunity(JDG_ConceptDefOf.ABF_Concept_Justiciar_Inspirations, OpportunityType.Critical);
        }
    }
}
