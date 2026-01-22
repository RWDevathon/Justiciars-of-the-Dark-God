using RimWorld;
using System.Text;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    // To endure suffering means trying to have 50% or more pain for 50% or more of the duration.
    public class Hediff_Ambition_EndureSuffering : Hediff_Ambition
    {
        public int ticksWithPain = 0;
        public int ticksTotal = 0;

        public float TimeWithPainAsPercentage => ticksWithPain / Mathf.Max(ticksTotal, 1f);

        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (!complete)
            {
                if (GenTicks.TicksGame > expirationTick)
                {
                    if (TimeWithPainAsPercentage >= 0.5f)
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
                    if (pawn.health.hediffSet.PainTotal >= 0.5)
                    {
                        ticksWithPain += delta;
                    }
                    ticksTotal += delta;
                }
            }
            else if (GenTicks.TicksGame > expirationTick)
            {
                pawn.health.RemoveHediff(this);
            }
        }

        // On failure, the justiciar falls into a berserk rage and despairs.
        public override void NotifyFailed()
        {
            pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, reason: "JDG_AmbitionFailed".Translate(), forced: true);
            Hediff crushingDespair = HediffMaker.MakeHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_CrushingDespair, pawn);
            if (crushingDespair.TryGetComp<HediffComp_Disappears>() is HediffComp_Disappears comp)
            {
                comp.SetDuration(Extension.expirationTicks.RandomInRange);
            }
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
            Find.LetterStack.ReceiveLetter("JDG_AmbitionSucceeded".Translate(), "JDG_AmbitionSucceeded_EndureSuffering".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.PositiveEvent);

            // Completing this ambition grants favor.
            pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>()?.NotifyFavorGained(20f);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksWithPain, "ABF_ticksWithPain", 0);
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
                    stringBuilder.AppendLine("JDG_PainPercentage".Translate(TimeWithPainAsPercentage.ToStringPercent()));
                    stringBuilder.Append(base.TipStringExtra);
                    return stringBuilder.ToString();
                }
                return base.TipStringExtra;
            }
        }
    }
}
