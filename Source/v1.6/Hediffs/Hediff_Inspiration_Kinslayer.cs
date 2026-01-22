using RimWorld;
using System;
using System.Linq;
using Verse;

namespace ArtificialBeings
{
    // Kinslayer means to kill a pawn related by blood. It succeeds when the victim is killed. It fails if the inspiration expires.
    public class Hediff_Inspiration_Kinslayer : Hediff_MentalTracker
    {
        // Inspiration that creates this hediff should also set it. If it's missing for whatever reason, go looking for it.
        private Inspiration_Justiciar_Kinslayer kinslayerInspiration;

        public Inspiration_Justiciar_Kinslayer KinslayerInspiration
        {
            get
            {
                if (kinslayerInspiration == null && pawn.Inspiration is Inspiration_Justiciar_Kinslayer inspiration)
                {
                    kinslayerInspiration = inspiration;
                }
                return kinslayerInspiration;
            }
            set
            {
                kinslayerInspiration = value;
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
            if (pawn.relations.FamilyByBlood.Contains(victim))
            {
                NotifySucceeded();
            }
        }

        // On failure, the justiciar goes into a murderous rage and suffers crushing despair 10 times longer than its normal duration.
        public override void NotifyFailed()
        {
            pawn.mindState.mentalStateHandler.TryStartMentalState(JDG_MentalStateDefOf.MurderousRage, reason: "JDG_AmbitionFailed".Translate(), forced: true);
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
            pawn.mindState.inspirationHandler.EndInspiration(KinslayerInspiration);
            pawn.health.RemoveHediff(this);
            Find.LetterStack.ReceiveLetter("JDG_InspirationSucceeded".Translate(), "JDG_InspirationSucceeded_Kinslayer".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NeutralEvent);
        }
    }
}
