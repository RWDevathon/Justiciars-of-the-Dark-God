using RimWorld;
using System.Text;
using Verse;

namespace ArtificialBeings
{
    // Acolytes are cultists yet to achieve the rank of Justiciar. This hediff tracks progress toward the favor necessary to become one.
    public class Hediff_Acolyte : Hediff_Devotee
    {
        // Tracker for if this pawn has ever crossed the threshold to become a justiciar. Used to make sure duplicate letters aren't sent and this hediff doesn't behave weirdly.
        public bool chosenLetterSent = false;

        public override void TickInterval(int delta)
        {
            if (pawn.Spawned)
            {
                Severity = pawn.Map.glowGrid.GroundGlowAt(pawn.Position);
            }
            base.TickInterval(delta);
            if (!chosenLetterSent)
            {
                // Reaching 100 favor makes this acolyte a justiciar. Reaching -50 favor makes them lose the status.
                if (favorCurrent >= 100f)
                {
                    // The newly inducted target must choose their path. When their path is chosen, this hediff will be removed.
                    ChoiceLetter_ChooseJusticiarPath choiceLetter = (ChoiceLetter_ChooseJusticiarPath)LetterMaker.MakeLetter(JDG_LetterDefOf.ABF_Letter_Justiciar_ChoosePath);
                    choiceLetter.candidate = pawn;
                    choiceLetter.Label = "JDG_AcolyteChosen".Translate(pawn.LabelShort);
                    choiceLetter.Text = "JDG_AcolyteChosenDesc".Translate(pawn.LabelShort, pawn.Named("PAWN"));
                    choiceLetter.lookTargets = pawn;
                    Find.LetterStack.ReceiveLetter(choiceLetter);
                    chosenLetterSent = true;
                }
                else if (favorCurrent <= -50f)
                {
                    Find.LetterStack.ReceiveLetter("JDG_AcolyteExpelled".Translate(pawn.LabelShort, pawn.Named("PAWN")), "JDG_AcolyteExpelledDesc".Translate(pawn.LabelShort, pawn.Named("PAWN")), LetterDefOf.NegativeEvent, lookTargets: pawn);
                    pawn.health.RemoveHediff(this);
                }
            }
        }

        // This hediff identifies acolytes, and they should be added to the cache of all known acolytes on being made one as well as receiving a pain grace period.
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            Find.LetterStack.ReceiveLetter("JDG_AcolyteRecruited".Translate(pawn.LabelShort, pawn.Named("PAWN")), "JDG_AcolyteRecruitedDesc".Translate(pawn.LabelShort, pawn.Named("PAWN")), LetterDefOf.PositiveEvent, lookTargets: pawn);
            JDG_Utils.Acolytes.Add(pawn);

            // If the player has not yet learned about acolytes, they will also receive a learning helper tip about how favor and acolytes work.
            LessonAutoActivator.TeachOpportunity(JDG_ConceptDefOf.ABF_Concept_Justiciar_Attainment, OpportunityType.Critical);
        }

        // Acolytes that become justiciars or lose their status are no longer retained as acolytes.
        public override void PostRemoved()
        {
            base.PostRemoved();
            JDG_Utils.Acolytes.Remove(pawn);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref chosenLetterSent, "JDG_chosenLetterSent", false);
            // This hediff identifies acolytes, and they should be re-added to the cache of all known acolytes on loading saves.
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                JDG_Utils.Acolytes.Add(pawn);
            }
        }

        public override string GetTooltip(Pawn pawn, bool showHediffsDebugInfo)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetTooltip(pawn, showHediffsDebugInfo));
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("JDG_AcolyteFavor".Translate(favorCurrent.ToString("F0")));
            return stringBuilder.ToString();
        }
    }
}
