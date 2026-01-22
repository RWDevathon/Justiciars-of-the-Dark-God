using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class ChoiceLetter_AcceptAcolyteWhispers : ChoiceLetter
    {
        public Pawn candidate;

        public override bool CanDismissWithRightClick => false;

        public override IEnumerable<DiaOption> Choices
        {
            get
            {
                if (ArchivedOnly)
                {
                    yield return Option_Close;
                    yield break;
                }
                DiaOption acceptOption = new DiaOption("AcceptButton".Translate())
                {
                    action = delegate
                    {
                        candidate.health.AddHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_Acolyte);
                        Find.LetterStack.RemoveLetter(this);
                    },
                    resolveTree = true
                };
                yield return acceptOption;
                yield return Option_Reject;
                if (lookTargets.IsValid())
                {
                    yield return Option_JumpToLocationAndPostpone;
                }
                yield return Option_Postpone;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref candidate, "JDG_candidateAcolyte");
        }
    }
}