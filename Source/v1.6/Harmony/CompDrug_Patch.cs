using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace ArtificialBeings
{
    public class CompDrug_Patch
    {
        // This transpiler makes anyone currently under the effect of the orb's contemplated effect immune to new addictions (as long as they are normally curable) when using drugs.
        [HarmonyPatch(typeof(CompDrug), "PrePostIngested")]
        public class CompDrug_PrePostIngested_Patch
        {
            // For debugging, enable [HarmonyDebug]
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> insts, ILGenerator generator)
            {
                bool indicatorLocated = false;
                MethodInfo drugPropsProperty = AccessTools.PropertyGetter(typeof(CompDrug), nameof(CompDrug.Props));
                MethodInfo targetIndicatorProperty = AccessTools.PropertyGetter(typeof(Rand), nameof(Rand.Value));

                CodeInstruction loadCompInstruction = new CodeInstruction(OpCodes.Ldarg_0); // The CompDrug instance
                CodeInstruction loadDrugPropsInstruction = new CodeInstruction(OpCodes.Call, drugPropsProperty);
                CodeInstruction loadPawnInstruction = new CodeInstruction(OpCodes.Ldarg_1); // The ingester Pawn
                CodeInstruction callInstruction = new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CompDrug_PrePostIngested_Patch), nameof(ShouldBeImmune)));

                List<CodeInstruction> instructions = new List<CodeInstruction>(insts);


                for (int i = 0; i < instructions.Count; i++)
                {
                    // The indicator denotes a location where we want to insert a condition prior to running. We should only do this once.
                    if (!indicatorLocated && instructions[i].Calls(targetIndicatorProperty))
                    {
                        indicatorLocated = true;
                        // We perform a look-ahead here to find the label we want to branch to in case our condition is false.
                        for (int j = i + 1; j < instructions.Count; j++)
                        {
                            if (instructions[j].Branches(out Label? branchTargetLabel))
                            {
                                yield return loadCompInstruction;
                                instructions[j].MoveLabelsTo(loadCompInstruction); // Don't jump past our code.
                                yield return loadDrugPropsInstruction;
                                yield return loadPawnInstruction;
                                yield return callInstruction;
                                yield return new CodeInstruction(OpCodes.Brtrue, branchTargetLabel);
                                break;
                            }
                        }
                    }
                    yield return instructions[i];
                }
            }

            // Should return true if the drug can ever be cured via item (IE. isn't permanent) and the pawn has contemplated the orb.
            private static bool ShouldBeImmune(CompProperties_Drug drugProps, Pawn ingester)
            {
                return drugProps.chemical.addictionHediff.everCurableByItem && ingester.health.hediffSet.HasHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_ContemplatedOrb);
            }
        }
    }
}
