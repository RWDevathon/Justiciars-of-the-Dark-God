using RimWorld;
using Verse;

namespace ArtificialBeings
{
    // This inspiration exists purely to kill the pawn if they fail to die on their own terms. Inspirations are destroyed and the handler recreated on resurrection.
    public class Inspiration_Justiciar_CommuneWithDarkness : Inspiration
    {
        public override void PostEnd()
        {
            if (!pawn.Dead)
            {
                Hediff crushingDespair = HediffMaker.MakeHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_CrushingDespair, pawn);
                if (crushingDespair.TryGetComp<HediffComp_Disappears>() is HediffComp_Disappears comp)
                {
                    comp.SetDuration(comp.disappearsAfterTicks * 10);
                }
                pawn.health.AddHediff(crushingDespair);
                Find.LetterStack.ReceiveLetter("ABF_InspirationFailed".Translate(), "ABF_InspirationFailed_CommuneWithDarkness".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst(), LetterDefOf.NegativeEvent);
                pawn.Kill(null, crushingDespair);
            }
        }
    }
}
