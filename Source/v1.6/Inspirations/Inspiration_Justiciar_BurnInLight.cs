using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class Inspiration_Justiciar_BurnInLight : Inspiration
    {
        public int ticksInDarkness = 0;

        public override void InspirationTick(int delta)
        {
            if (pawn.PositionHeld != IntVec3.Invalid && pawn.Map.glowGrid.GroundGlowAt(pawn.PositionHeld) < 0.3)
            {
                ticksInDarkness += delta;
            }
            // Handles expiration
            base.InspirationTick(delta);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksInDarkness, "ABF_ticksInDarkness");
        }

        public override void PostEnd()
        {
            if (ticksInDarkness < GenDate.TicksPerDay)
            {
                Find.LetterStack.ReceiveLetter("ABF_InspirationSucceeded".Translate(), "ABF_InspirationSucceeded_BurnInlight".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NeutralEvent);
                pawn.mindState.mentalBreaker.Notify_RecoveredFromMentalState();
            }
            else
            {
                Find.LetterStack.ReceiveLetter("ABF_InspirationFailed".Translate(), "ABF_InspirationFailed_BurnInlight".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NegativeEvent);
                foreach (SkillRecord skillRecord in pawn.skills.skills)
                {
                    skillRecord.Level -= 2;
                }
                Hediff crushingDespair = HediffMaker.MakeHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_CrushingDespair, pawn);
                if (crushingDespair.TryGetComp<HediffComp_Disappears>() is HediffComp_Disappears comp)
                {
                    comp.SetDuration(comp.disappearsAfterTicks * 10);
                }
                pawn.health.AddHediff(crushingDespair);
            }
        }
    }
}
