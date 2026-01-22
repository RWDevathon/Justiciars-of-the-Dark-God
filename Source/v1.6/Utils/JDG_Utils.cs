using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public static class JDG_Utils
    {
        // Static reference to the GameComponent that keeps track of all existing justiciars. Should be accessed by the getters.
        private static GameComponent_Justiciars justiciarsCache;

        // Dictionary matching maps to their darkness grids to be checked as needed.
        public static Dictionary<Map, MapComponent_DarknessGrid> darkGridDict = new Dictionary<Map, MapComponent_DarknessGrid>();

        public static GameComponent_Justiciars GameComponent_Justiciars
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

        public static HashSet<Pawn> Justiciars
        {
            get
            {
                return GameComponent_Justiciars.allJusticiars;
            }
        }

        public static HashSet<Pawn> Acolytes
        {
            get
            {
                return GameComponent_Justiciars.allAcolytes;
            }
        }

        public static HashSet<Pawn> ShadeSpirits
        {
            get
            {
                return GameComponent_Justiciars.allShadeSpirits;
            }
        }

        // Takes a Pawn and applies certain details from the provided PawnKindDef to them, such as traits or hediffs.
        // This is a way of taking an existing pawn and using an XML def to add some additional information to them.
        public static void ConvertIntoJusticiar(Pawn source, PawnKindDef justiciarKind)
        {
            if (justiciarKind.abilities is List<AbilityDef> abilities)
            {
                foreach (AbilityDef abilityDef in abilities)
                {
                    source.abilities?.GainAbility(abilityDef);
                }
            }
            if (justiciarKind.startingHediffs is List<StartingHediff> hediffs)
            {
                // AddStartingHediffs handles them already having the hediff itself.
                HealthUtility.AddStartingHediffs(source, hediffs);
            }
            if (source.story != null)
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
                source.story.Childhood = justiciarKind.fixedChildBackstories.RandomElement();
                source.story.Adulthood = justiciarKind.fixedAdultBackstories.RandomElement();
            }
            source.skills?.Notify_SkillDisablesChanged();
            Hediff acolyteHediff = source.health.hediffSet.GetFirstHediff<Hediff_Acolyte>();
            if (acolyteHediff != null)
            {
                source.health.RemoveHediff(acolyteHediff);
            }
        }

        public static bool IsJusticiar(Pawn pawn)
        {
            return Justiciars.Contains(pawn);
        }

        public static bool IsAcolyte(Pawn pawn)
        {
            return Acolytes.Contains(pawn);
        }

        public static bool IsShadeSpirit(Pawn pawn)
        {
            return ShadeSpirits.Contains(pawn);
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

        // For a given target Thing, returns the cost to clone it. Returns 0 if the cost is unknown.
        public static float FavorCostToClone(Thing target)
        {
            if (target == null)
            {
                return 0f;
            }
            // Anything which has an extension specifying its cost should use that (including animals and mechanoids).
            else if (target.def.GetModExtension<UmbralClonableExtension>() is UmbralClonableExtension extension)
            {
                return extension.favorCost;
            }
            else if (target is Pawn pawn)
            {
                // Colony animals may be cloned for 10 favor per body size.
                if (pawn.IsColonyAnimal)
                {
                    return 10f * pawn.BodySize;
                }
                // Colony mechanoids (that are currently controlled) may be cloned for 25 favor per bandwith cost. If it has no cost, it cannot be cloned.
                else if (pawn.IsColonyMechPlayerControlled)
                {
                    return 25f * pawn.GetStatValue(StatDefOf.BandwidthCost, cacheStaleAfterTicks: GenTicks.TicksPerRealSecond);
                }
            }
            return 0f;
        }
        
        // Humanlike colonists can be inducted as acolytes. Player animals/mechanoids can be inducted as shadespirits. Returns 0 if the cost is unknown.
        public static float FavorCostToInduct(Pawn pawn)
        {
            if (pawn.RaceProps.Humanlike)
            {
                return 10f;
            }
            else if (pawn.IsColonyAnimal)
            {
                return 25f * pawn.BodySize;
            }
            else if (pawn.IsColonyMechPlayerControlled)
            {
                return 25f * pawn.GetStatValue(StatDefOf.BandwidthCost, cacheStaleAfterTicks:GenTicks.TicksPerRealSecond);
            }
            return 0f;
        }
    }
}
