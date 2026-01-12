using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public static class JDG_Utils
    {
        // Static reference to the GameComponent that keeps track of all existing justiciars.
        public static GameComponent_Justiciars justiciarsCache;

        // Dictionary matching maps to their darkness grids to be checked as needed.
        public static Dictionary<Map, MapComponent_DarknessGrid> darkGridDict = new Dictionary<Map, MapComponent_DarknessGrid>();

        public static GameComponent_Justiciars Justiciars
        {
            get
            {
                if (justiciarsCache == null)
                {
                    justiciarsCache = Current.Game.GetComponent<GameComponent_Justiciars>();
                }
                return justiciarsCache;
            }
        }

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

        public static bool IsJusticiar(Pawn pawn)
        {
            return Justiciars.allJusticiars.Contains(pawn);
        }

        public static bool IsDark(IntVec3 cell, Map map)
        {
            if (!darkGridDict.ContainsKey(map))
            {
                darkGridDict[map] = map.GetComponent<MapComponent_DarknessGrid>();
            }
            return darkGridDict[map].IsDark(cell);
        }

        public static void ClearCaches()
        {
            justiciarsCache = null;
        }
    }
}
