using RimWorld;
using System.Text;
using Verse;

namespace ArtificialBeings
{
    // Contemplating murder means killing a colonist or several slaves or animals. It succeeds when the threshold is passed. It fails if the ambition expires.
    public class Hediff_Ambition_ContemplatingMurder : Hediff_Ambition
    {
        public float killLevel = 0;

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
            if (!complete)
            {
                if (victim.RaceProps.Humanlike && victim.IsFreeNonSlaveColonist)
                {
                    NotifySucceeded();
                    return;
                }
                else if (victim.RaceProps.Humanlike && victim.IsColonist)
                {
                    killLevel += 0.5f;
                }
                else if (victim.IsColonyAnimal)
                {
                    killLevel += 0.167f;
                }
                if (killLevel >= 1f)
                {
                    NotifySucceeded();
                }
            }
        }

        // On failure, the justiciar goes into a murderous rage.
        public override void NotifyFailed()
        {
            pawn.mindState.mentalStateHandler.TryStartMentalState(JDG_MentalStateDefOf.MurderousRage, reason: "ABF_AmbitionFailed".Translate(), forced: true);
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
            Find.LetterStack.ReceiveLetter("ABF_AmbitionSucceeded".Translate(), "ABF_AmbitionSucceeded_ContemplatingMurder".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.PositiveEvent);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref killLevel, "ABF_killLevel", 0);
        }

        // Players should be able to tell easily how many kills have been scored.
        public override string TipStringExtra
        {
            get
            {
                if (!complete)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("ABF_CurrentKillLevel".Translate(killLevel.ToStringPercent()));
                    stringBuilder.Append(base.TipStringExtra);
                    return stringBuilder.ToString();
                }
                return base.TipStringExtra;
            }
        }
    }
}
