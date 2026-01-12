using RimWorld;
using System.Text;
using Verse;

namespace ArtificialBeings
{
    // Cleansing fury means killing a certain number of heretics. It succeeds when that number of kills is met. It fails if the ambition expires.
    public class Hediff_Ambition_CleansingFury : Hediff_Ambition
    {
        public const int targetKills = 10;

        public float killTotal = 0;

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

        // On failure, the justiciar goes into a berserking rage.
        public override void NotifyFailed()
        {
            pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, reason: "ABF_AmbitionFailed".Translate(), forced: true);
            pawn.health.RemoveHediff(this);
        }

        // On success, the justiciar should receive a longer-term bonus. It is expected that the bonus is unlocked by setting the Severity to 1.
        // This bonus lasts an additional duration without resetting, meaning completion of this ambition swiftly allows for a longer bonus.
        public override void NotifySucceeded()
        {
            base.NotifySucceeded();
            complete = true;
            Severity = 1f;
            expirationTick += Extension.expirationTicks.RandomInRange;
            Find.LetterStack.ReceiveLetter("ABF_AmbitionSucceeded".Translate(), "ABF_AmbitionSucceeded_CleansingFury".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.PositiveEvent);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref killTotal, "ABF_killTotal", 0);
        }

        // Players should be able to tell easily how many kills have been scored.
        public override string TipStringExtra
        {
            get
            {
                if (!complete)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("ABF_CurrentKillTotal".Translate(killTotal.ToString()));
                    stringBuilder.Append(base.TipStringExtra);
                    return stringBuilder.ToString();
                }
                return base.TipStringExtra;
            }
        }
    }
}
