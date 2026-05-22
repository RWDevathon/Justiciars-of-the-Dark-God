using RimWorld;
using Verse;

namespace ArtificialBeings
{
    // This inspiration exists purely to kill the pawn if they do not contemplate the orb. Inspirations are destroyed when the orb is contemplated.
    public class Inspiration_Justiciar_ContemplateOrb : Inspiration
    {
        public override void PostEnd()
        {
            if (!pawn.Dead)
            {
                Hediff crushingDespair = HediffMaker.MakeHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_CrushingDespair, pawn);
                if (crushingDespair.TryGetComp<HediffComp_Disappears>() is HediffComp_Disappears comp)
                {
                    comp.SetDuration(comp.disappearsAfterTicks * 10);
                }
                pawn.health.AddHediff(crushingDespair);
                Find.LetterStack.ReceiveLetter("JDG_InspirationFailed".Translate(), "JDG_InspirationFailed_ContemplateOrb".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NegativeEvent);
                pawn.Kill(null, crushingDespair);
            }
        }

        public override void PostStart(bool sendLetter = true)
        {
            base.PostStart(sendLetter);

            // Receiving an inspiration results in favor loss.
            JDG_Utils.GetJusticiarHediff(pawn)?.NotifyFavorLost(25f);

            // If the player has not yet learned about inspirations, they will also receive a learning helper tip about how they work.
            LessonAutoActivator.TeachOpportunity(JDG_ConceptDefOf.ABF_Concept_Justiciar_Inspirations, OpportunityType.Critical);
        }
    }
}
