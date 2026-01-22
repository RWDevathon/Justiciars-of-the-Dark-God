using RimWorld;
using System.Text;
using Verse;

namespace ArtificialBeings
{
    // Bloodletter means dealing or taking a certain amount of damage. It succeeds when that damage threshold is met. It fails if the ambition expires.
    public class Hediff_Ambition_Bloodletter : Hediff_Ambition
    {
        public const float targetDamage = 500f;

        public float damageTotal = 0;

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

        // Taking damage
        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
            if (!complete)
            {
                damageTotal += totalDamageDealt;
                if (damageTotal > targetDamage)
                {
                    NotifySucceeded();
                }
            }
        }

        // Dealing damage
        public override void Notify_PawnDamagedThing(Thing thing, DamageInfo dinfo, DamageWorker.DamageResult result)
        {
            base.Notify_PawnDamagedThing(thing, dinfo, result);
            if (!complete && thing is Pawn)
            {
                damageTotal += result.totalDamageDealt;
                if (damageTotal > targetDamage)
                {
                    NotifySucceeded();
                }
            }
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
            expirationTick += Extension.expirationTicks.RandomInRange;
            Find.LetterStack.ReceiveLetter("JDG_AmbitionSucceeded".Translate(), "JDG_AmbitionSucceeded_Bloodletter".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.PositiveEvent);

            // Completing this ambition grants favor.
            pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>()?.NotifyFavorGained(40f);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref damageTotal, "ABF_damageTotal", 0);
        }

        // Players should be able to tell easily how much damage has been dealt or taken.
        public override string TipStringExtra
        {
            get
            {
                if (!complete)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("JDG_CurrentDamageTotal".Translate(damageTotal.ToString()));
                    stringBuilder.Append(base.TipStringExtra);
                    return stringBuilder.ToString();
                }
                return base.TipStringExtra;
            }
        }
    }
}
