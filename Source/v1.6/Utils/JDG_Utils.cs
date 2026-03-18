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

        // Dictionary matching pawns' thingIds to trackers including information on when perceptors last revealed something about them.
        public static Dictionary<int, PerceptorTracker> perceptorCheckedPawns = new Dictionary<int, PerceptorTracker>();

        // List referencing all currently existing dark creatures for the umbral anchor.
        public static List<PawnKindDef> darkCreatureKinds = new List<PawnKindDef>()
        {
            JDG_PawnKindDefOf.ABF_PawnKind_Justiciar_Player_CreaturePerceptor,
            JDG_PawnKindDefOf.ABF_PawnKind_Justiciar_Player_CreatureWraith,
            JDG_PawnKindDefOf.ABF_PawnKind_Justiciar_Player_CreatureHerald,
        };

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
            Hediff_Devotee acolyteHediff = source.health.hediffSet.GetFirstHediff<Hediff_Acolyte>();
            if (acolyteHediff != null)
            {
                if (acolyteHediff.FavorCurrent > 100f && source.health.hediffSet.GetFirstHediff<Hediff_Justiciar>() is Hediff_Justiciar justiciarHediff)
                {
                    justiciarHediff.NotifyFavorGained(acolyteHediff.FavorCurrent - 100f);
                }
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

        public static bool IsDevotee(Pawn pawn)
        {
            return IsJusticiar(pawn) || IsAcolyte(pawn);
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
            perceptorCheckedPawns = new Dictionary<int, PerceptorTracker>();
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
                    // Dryads may not be cloned, as they're intrinsically tied to Gauranlen tree mechanics. Dark creatures can still be cloned, but are more expensive.
                    if (pawn.RaceProps.Dryad)
                    {
                        if (darkCreatureKinds.Contains(pawn.kindDef))
                        {
                            return 40f * pawn.BodySize;
                        }
                        else
                        {
                            return 0f;
                        }
                    }
                    return 10f * pawn.RaceProps.baseBodySize;
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
            float result;
            if (pawn.RaceProps.Humanlike)
            {
                result = 10f;
            }
            else if (pawn.IsColonyAnimal)
            {
                result = 10f * pawn.BodySize;
            }
            else if (pawn.IsColonyMechPlayerControlled)
            {
                result = 10f * pawn.GetStatValue(StatDefOf.BandwidthCost, cacheStaleAfterTicks: GenTicks.TicksPerRealSecond);
            }
            else
            {
                result = 0f;
            }
            if (result <= 0f)
            {
                return result;
            }

            // Adjust by development stage
            if (pawn.DevelopmentalStage == DevelopmentalStage.Newborn)
            {
                result /= 10f;
            }
            else if (pawn.DevelopmentalStage == DevelopmentalStage.Child)
            {
                result /= 5f;
            }
            return result;
        }

        // Wild animals that are not aggressive can be dominated and instantly tamed. The cost is given by the following curve, depending on the target's wildness.
        public static readonly SimpleCurve dominationFavorCostForWildnessCurve = new SimpleCurve
        {
            new CurvePoint(0.0f, 5f),
            new CurvePoint(0.1f, 5f),
            new CurvePoint(0.2f, 10f),
            new CurvePoint(0.4f, 20f),
            new CurvePoint(0.8f, 40f),
            new CurvePoint(0.95f, 80f)
        };

        public static float FavorCostToDominate(Pawn pawn)
        {
            return dominationFavorCostForWildnessCurve.Evaluate(pawn.GetStatValue(StatDefOf.Wildness, cacheStaleAfterTicks: GenDate.TicksPerHour));
        }

        public static void UpdatePerceptorTracker(Pawn pawn, int tick)
        {
            if (perceptorCheckedPawns.TryGetValue(pawn.thingIDNumber, out var tracker))
            {
                tracker.tickPerceived = tick;
            }
            else
            {
                perceptorCheckedPawns[pawn.thingIDNumber] = new PerceptorTracker(tick);
            }
        }

        public static PerceptorTracker GetPerceptorTracker(this Pawn pawn)
        {
            return perceptorCheckedPawns.TryGetValue(pawn.thingIDNumber, null);
        }
    }
}
