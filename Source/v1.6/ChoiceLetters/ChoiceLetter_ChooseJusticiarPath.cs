using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class ChoiceLetter_ChooseJusticiarPath : ChoiceLetter
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
                DiaOption diaOption = new DiaOption("AcceptButton".Translate())
                {
                    action = delegate
                    {
                        Find.WindowStack.Add(new Dialog_InductJusticiar(candidate));
                        Find.LetterStack.RemoveLetter(this);
                    },
                    resolveTree = true
                };
                yield return diaOption;
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
            Scribe_References.Look(ref candidate, "JDG_newJusticiar");
        }
    }
}