using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public static class JDG_Utils
    {
        // Takes a Pawn and applies certain details from the provided PawnKindDef to them, such as traits or hediffs.
        // This is a way of taking an existing pawn and using an XML def to add some additional information to them.
        public static void ConvertIntoJusticiar(Pawn source, PawnKindDef justiciarKind)
        {
            if (justiciarKind.forcedTraits is List<TraitRequirement> requirements)
            {
                foreach (TraitRequirement requirement in requirements)
                {
                    if (!source.story.traits.HasTrait(requirement.def))
                    {
                        source.story.traits.GainTrait(new Trait(requirement.def, requirement.degree.GetValueOrDefault(), forced: true));
                    }
                }
            }
            if (justiciarKind.startingHediffs is List<StartingHediff> hediffs)
            {
                // AddStartingHediffs handles them already having the hediff itself.
                HealthUtility.AddStartingHediffs(source, hediffs);
            }
            source.story.Childhood = justiciarKind.fixedChildBackstories.RandomElement();
            source.story.Adulthood = justiciarKind.fixedAdultBackstories.RandomElement();
            source.skills.Notify_SkillDisablesChanged();
        }
    }
}
