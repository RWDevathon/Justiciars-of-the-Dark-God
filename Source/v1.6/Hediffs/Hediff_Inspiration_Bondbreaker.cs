using RimWorld;
using Verse;

namespace ArtificialBeings
{
    // Bond breaker means to kill a bonded animal or a friend. It succeeds when the victim is killed. It fails if the inspiration expires.
    public class Hediff_Inspiration_Bondbreaker : Hediff_MentalTracker
    {
        // Inspiration that creates this hediff should also set it. If it's missing for whatever reason, go looking for it.
        private Inspiration_Justiciar_Bondbreaker bondbreakerInspiration;

        public Inspiration_Justiciar_Bondbreaker BondbreakerInspiration
        {
            get
            {
                if (bondbreakerInspiration == null && pawn.Inspiration is Inspiration_Justiciar_Bondbreaker inspiration)
                {
                    bondbreakerInspiration = inspiration;
                }
                return bondbreakerInspiration;
            }
            set
            {
                bondbreakerInspiration = value;
            }
        }

        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (TicksRemaining <= 0)
            {
                NotifyFailed();
            }
        }

        public override void Notify_KilledPawn(Pawn victim, DamageInfo? dinfo)
        {
            base.Notify_KilledPawn(victim, dinfo);
            if (pawn.relations.GetDirectRelation(PawnRelationDefOf.Bond, victim) != null)
            {
                NotifySucceeded();
            }
            if (pawn.relations.OpinionOf(victim) > 75)
            {
                NotifySucceeded();
            }
        }

        // On failure, the justiciar goes into a murderous rage and suffers crushing despair 10 times longer than its normal duration.
        public override void NotifyFailed()
        {
            pawn.mindState.mentalStateHandler.TryStartMentalState(JDG_MentalStateDefOf.MurderousRage, reason: "ABF_AmbitionFailed".Translate(), forced: true);
            Hediff crushingDespair = HediffMaker.MakeHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_CrushingDespair, pawn);
            if (crushingDespair.TryGetComp<HediffComp_Disappears>() is HediffComp_Disappears comp)
            {
                comp.SetDuration(comp.disappearsAfterTicks * 10);
            }
            pawn.health.AddHediff(crushingDespair);
            pawn.health.RemoveHediff(this);
        }

        // On success, the justiciar simply no longer suffers this hediff.
        public override void NotifySucceeded()
        {
            pawn.mindState.inspirationHandler.EndInspiration(BondbreakerInspiration);
            pawn.health.RemoveHediff(this);
            Find.LetterStack.ReceiveLetter("ABF_InspirationSucceeded".Translate(), "ABF_InspirationSucceeded_Bondbreaker".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NeutralEvent);
        }
    }
}
