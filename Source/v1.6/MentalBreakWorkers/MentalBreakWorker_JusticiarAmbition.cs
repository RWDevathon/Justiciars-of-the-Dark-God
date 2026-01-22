using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class MentalBreakWorker_JusticiarAmbition : MentalBreakWorker
    {
        private JusticiarMentalBreakExtension extension;

        public JusticiarMentalBreakExtension Extension
        {
            get
            {
                if (extension == null)
                {
                    extension = def.GetModExtension<JusticiarMentalBreakExtension>();
                }
                return extension;
            }
        }

        // Justiciars may not have multiple copies of the same ambition at once.
        public override bool BreakCanOccur(Pawn pawn)
        {
            if (!pawn.health.hediffSet.HasHediff(Extension.associatedHediffDef))
            {
                return base.BreakCanOccur(pawn);
            }
            return false;
        }

        public override bool TryStart(Pawn pawn, string reason, bool causedByMood)
        {
            pawn.health.AddHediff(Extension.associatedHediffDef);
            // Send a letter to the player explaining the ambition if appropriate. Don't use the parent TrySendLetter, as it doesn't allow for full control.
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                TaggedString label = def.LabelCap + ": " + pawn.LabelShortCap;
                TaggedString taggedString = "JDG_AmbitionAcquired".Translate(pawn.LabelShort, pawn.Named("PAWN")).CapitalizeFirst();
                if (Extension?.associatedHediffDef != null)
                {
                    taggedString += " " + "JDG_AmbitionDetails".Translate(Extension.associatedHediffDef.label, pawn.Named("PAWN"));
                }
                if (reason != null)
                {
                    taggedString += "\n\n" + reason;
                }
                taggedString = taggedString.AdjustedFor(pawn);
                Find.LetterStack.ReceiveLetter(label, taggedString, LetterDefOf.NeutralEvent, pawn);
            }
            return true;
        }
    }
}
