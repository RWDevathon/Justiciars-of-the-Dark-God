using RimWorld;
using System;
using System.Text;
using Verse;

namespace ArtificialBeings
{
    // Cleansing fury means killing a certain number of heretics. It succeeds when that number of kills is met. It fails if the ambition expires.
    public class Hediff_Ambition_CleansingFury : Hediff_Ambition
    {
        public int targetKills = 10;

        public float killTotal = 0;

        public const float favorOnSuccess = 20f;

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

        public override void Notify_KilledPawn(Pawn victim, DamageInfo? dinfo)
        {
            base.Notify_KilledPawn(victim, dinfo);
            if (!complete && victim.RaceProps.Humanlike && !JDG_Utils.IsJusticiar(victim))
            {
                killTotal++;
                if (killTotal >= targetKills)
                {
                    NotifySucceeded();
                }
            }
        }

        // When this hediff is generated, the number of heretics to kill will be chosen. The target number depends on the player's own number of pawns, up to 10.
        public override void PostMake()
        {
            base.PostMake();
            targetKills = Math.Min(10, 1 + PawnsFinder.AllMapsCaravansAndTravellingTransporters_AliveSpawned_FreeColonists.Count);
        }

        // On failure, the justiciar goes into a berserking rage.
        public override void NotifyFailed()
        {
            pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, reason: "JDG_AmbitionFailed".Translate(), forced: true);
            pawn.health.RemoveHediff(this);
        }

        // On success, the justiciar should receive a longer-term bonus. It is expected that the bonus is unlocked by setting the Severity to 1.
        // This bonus lasts an additional duration without resetting, meaning completion of this ambition swiftly allows for a longer bonus.
        public override void NotifySucceeded()
        {
            base.NotifySucceeded();
            complete = true;
            Severity = 1f;
            expirationTick = GenTicks.TicksGame + Extension.expirationTicks.RandomInRange;
            Find.LetterStack.ReceiveLetter("JDG_AmbitionSucceeded".Translate(), "JDG_AmbitionSucceeded_CleansingFury".Translate(pawn.LabelShort, pawn.Named("PAWN"), favorOnSuccess.ToString("F0")).CapitalizeFirst(), LetterDefOf.PositiveEvent);

            // Completing this ambition grants favor.
            pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>()?.NotifyFavorGained(favorOnSuccess);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref killTotal, "ABF_killTotal", 0);
            Scribe_Values.Look(ref targetKills, "JDG_targetKills", 4);
        }

        // Players should be able to tell easily how many kills have been scored.
        public override string TipStringExtra
        {
            get
            {
                if (!complete)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("JDG_CurrentKillTotal".Translate(killTotal.ToString(), targetKills.ToString()));
                    stringBuilder.AppendLine("JDG_FavorOnSuccess".Translate(favorOnSuccess.ToString("F0")));
                    stringBuilder.Append(base.TipStringExtra);
                    return stringBuilder.ToString();
                }
                return base.TipStringExtra;
            }
        }
    }
}
