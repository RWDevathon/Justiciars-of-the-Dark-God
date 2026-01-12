using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace ArtificialBeings
{
    public class ShotReport_Patch
    {
        // This transpiler makes justiciars ignore blind gas (and by extension, their own conjured darkness) and weather when firing.
        [HarmonyPatch(typeof(ShotReport), "HitReportFor")]
        public class ShotReport_HitReportFor_Patch
        {
            [HarmonyTranspiler]
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> insts, ILGenerator generator)
            {
                bool indicatorLocated = false;
                List<CodeInstruction> instructions = new List<CodeInstruction>(insts);
                MethodInfo targetIndicatorProperty = AccessTools.PropertyGetter(typeof(CompUniqueWeapon), nameof(CompUniqueWeapon.IgnoreAccuracyMaluses));

                for (int i = 0; i < instructions.Count; i++)
                {
                    // If we have found our indicator, then the next assignment to this local variable will be consumed and replaced.
                    if (indicatorLocated && instructions[i].opcode == OpCodes.Stloc_2)
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0); // Grab a copy of the caster Thing
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ShotReport_HitReportFor_Patch), nameof(IgnoresAccuracyMaluses)));
                    }
                    yield return instructions[i];
                    // The indicator denotes a location where we want to consume a boolean value prior to it being stored.
                    if (instructions[i].Calls(targetIndicatorProperty))
                    {
                        indicatorLocated = true;
                    }
                }
            }

            // Returns true if the caster was already immune to maluses or if they are a Justiciar.
            private static bool IgnoresAccuracyMaluses(bool alreadyImmune, Thing caster)
            {
                return alreadyImmune || (caster is Pawn pawn && JDG_Utils.IsJusticiar(pawn));
            }
        }
    }
}
