using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class IncidentWorker_AcolyteWhispers : IncidentWorker
    {
        // Only fire if the player has no justiciars and acolytes.
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            foreach (Pawn justiciar in JDG_Utils.Justiciars)
            {
                if (!justiciar.Dead && justiciar.IsPlayerControlled)
                {
                    return false;
                }
            }

            foreach (Pawn acolyte in JDG_Utils.Acolytes)
            {
                if (!acolyte.Dead && acolyte.IsPlayerControlled)
                {
                    return false;
                }
            }
            return base.CanFireNowSub(parms);
        }

        // Identify a non-acolyte, non-justiciar player pawn and offer the chance for them to become an acolyte.
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            // Prefer picking pawns with lower mood at the moment. The Dark God's whispers resonate more with them.
            Pawn possibleAcolyte = PawnsFinder.AllMapsCaravansAndTravellingTransporters_AliveSpawned_FreeColonists.RandomElementByWeightWithFallback(pawn => 1 - pawn.needs?.mood?.CurLevelPercentage ?? 0.25f);
            if (possibleAcolyte == null)
            {
                return false;
            }
            ChoiceLetter_AcceptAcolyteWhispers choiceLetter = (ChoiceLetter_AcceptAcolyteWhispers)LetterMaker.MakeLetter(JDG_LetterDefOf.ABF_Letter_Justiciar_AcolyteWhispers);
            choiceLetter.candidate = possibleAcolyte;
            choiceLetter.Label = "JDG_AcolyteWhispers".Translate(possibleAcolyte.LabelShort);
            choiceLetter.Text = "JDG_AcolyteWhispersDesc".Translate(possibleAcolyte.LabelShort, possibleAcolyte.Named("PAWN"));
            choiceLetter.lookTargets = possibleAcolyte;
            Find.LetterStack.ReceiveLetter(choiceLetter);
            return true;
        }
    }
}