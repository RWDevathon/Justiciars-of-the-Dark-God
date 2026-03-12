using Verse;
using HarmonyLib;
using RimWorld;

namespace ArtificialBeings
{
    public class PregnancyUtility_Patch
    {
        // If a justiciar has a child, then their child should be an acolyte with an arbiter/neophyte relationship.
        [HarmonyPatch(typeof(PregnancyUtility), "ApplyBirthOutcome")]
        public class PregnancyUtility_ApplyBirthOutcome_Patch
        {
            // This patch needs to be run in a SCOS, not in the Mod class.
            [HarmonyPrepare]
            public static bool Prepare() => DefDatabase<PawnRelationDef>.AllDefsListForReading.Count > 0;

            [HarmonyPostfix]
            public static void Listener(ref Thing __result, Pawn geneticMother, Pawn father = null)
            {
                if (__result is Pawn newborn)
                {
                    Pawn justiciar = null;
                    if (geneticMother != null && JDG_Utils.IsJusticiar(geneticMother) && father != null && JDG_Utils.IsJusticiar(father))
                    {
                        justiciar = Rand.Bool ? geneticMother : father;
                    }
                    else if (geneticMother != null && JDG_Utils.IsJusticiar(geneticMother))
                    {
                        justiciar = geneticMother;
                    }
                    else if (father != null && JDG_Utils.IsJusticiar(father))
                    {
                        justiciar = father;
                    }

                    if (justiciar != null)
                    {
                        newborn.health.AddHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_Acolyte);
                        // The newborn views the justiciar as an arbiter. The justiciars views the new acolyte as their neophyte.
                        newborn.relations.AddDirectRelation(JDG_PawnRelationDefOf.ABF_PawnRelation_Justiciar_Arbiter, justiciar);
                        justiciar.relations.AddDirectRelation(JDG_PawnRelationDefOf.ABF_PawnRelation_Justiciar_Neophyte, newborn);
                    }
                }
            }
        }
    }
}