using RimWorld;
using System.Text;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    // Fixating on depression means trying to test the limits of one's sanity. It succeeds if the justiciar spent more than half of their time with <25% mood before the ambition expires, and fails otherwise.
    public class Hediff_Ambition_DepressiveFixation : Hediff_Ambition
    {
        public int ticksWithPoorMood = 0;
        public int ticksTotal = 0;

        public float TimeWithPoorMoodAsPercentage => ticksWithPoorMood / Mathf.Max(ticksTotal, 1f);

        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (!complete)
            {
                if (GenTicks.TicksGame > expirationTick)
                {
                    if (TimeWithPoorMoodAsPercentage >= 0.5f)
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
                    if (pawn.needs?.mood?.CurLevelPercentage < 0.25)
                    {
                        ticksWithPoorMood += delta;
                    }
                    ticksTotal += delta;
                }
            }
            else if (GenTicks.TicksGame > expirationTick)
            {
                pawn.health.RemoveHediff(this);
            }
        }

        // On failure, the justiciar has a psychotic breakdown and falls into despair for a little while.
        public override void NotifyFailed()
        {
            pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Wander_Psychotic, reason: "ABF_AmbitionFailed".Translate(), forced: true);
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
            Find.LetterStack.ReceiveLetter("ABF_AmbitionSucceeded".Translate(), "ABF_AmbitionSucceeded_DepressiveFixation".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.PositiveEvent);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksWithPoorMood, "ABF_ticksInDarkness", 0);
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
                    stringBuilder.AppendLine("ABF_PoorMoodPercentage".Translate(TimeWithPoorMoodAsPercentage.ToStringPercent()));
                    stringBuilder.Append(base.TipStringExtra);
                    return stringBuilder.ToString();
                }
                return base.TipStringExtra;
            }
        }
    }
}
