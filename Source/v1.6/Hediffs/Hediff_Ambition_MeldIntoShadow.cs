using RimWorld;
using System.Text;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    // Melding into the shadows means trying to spend more time in extreme dark than otherwise. It succeeds if the justiciar spent more than half of their time in <30% light conditions before the ambition expires, and fails otherwise.
    public class Hediff_Ambition_MeldIntoShadow : Hediff_Ambition
    {
        public int ticksInDarkness = 0;
        public int ticksTotal = 0;

        public float TimeInDarknessAsPercentage => ticksInDarkness / Mathf.Max(ticksTotal, 1f);

        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (!complete)
            {
                if (GenTicks.TicksGame > expirationTick)
                {
                    if (TimeInDarknessAsPercentage >= 0.5f)
                    {
                        NotifySucceeded();
                    }
                    else
                    {
                        NotifyFailed();
                    }
                }
                else
                {
                    // Assume that pawns that are unspawned or otherise have no position are in darkness. We can't know if it's true or not, but we will give players the benefit of the doubt.
                    if (pawn.PositionHeld == IntVec3.Invalid || pawn.Map.glowGrid.GroundGlowAt(pawn.PositionHeld) < 0.3)
                    {
                        ticksInDarkness += delta;
                    }
                    ticksTotal += delta;
                }
            }
            else if (GenTicks.TicksGame > expirationTick)
            {
                pawn.health.RemoveHediff(this);
            }
        }

        // On failure, the justiciar has a tantrum and falls into despair for a little while.
        public override void NotifyFailed()
        {
            pawn.mindState.mentalStateHandler.TryStartMentalState(JDG_MentalStateDefOf.Tantrum, reason: "JDG_AmbitionFailed".Translate(), forced: true);
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
            Find.LetterStack.ReceiveLetter("JDG_AmbitionSucceeded".Translate(), "JDG_AmbitionSucceeded_MeldIntoShadow".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.PositiveEvent);

            // Completing this ambition grants favor.
            pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>()?.NotifyFavorGained(10f);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksInDarkness, "ABF_ticksInDarkness", 0);
            Scribe_Values.Look(ref ticksTotal, "ABF_totalTicks", 0);
        }

        // Players should be able to tell easily roughly how much of the pawn's time has been spent in sufficient darkness.
        public override string TipStringExtra
        {
            get
            {
                if (!complete)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("JDG_InShadowsPercentage".Translate(TimeInDarknessAsPercentage.ToStringPercent()));
                    stringBuilder.Append(base.TipStringExtra);
                    return stringBuilder.ToString();
                }
                return base.TipStringExtra;
            }
        }
    }
}
