using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    public class IncidentWorker_CropBlight_Patch
    {
        // Perceptors may identify crop blights early, which effectively reduces the size of the incident. This is done destructively.
        [HarmonyPatch(typeof(IncidentWorker_CropBlight), "TryExecuteWorker")]
        public class IncidentWorker_CropBlight_TryExecuteWorker_Patch
        {
            private const float Radius = 11f;

            private static readonly SimpleCurve BlightChancePerRadius = new SimpleCurve
            {
                new CurvePoint(0f, 1f),
                new CurvePoint(8f, 1f),
                new CurvePoint(11f, 0.3f)
            };

            private static readonly SimpleCurve RadiusFactorPerPointsCurve = new SimpleCurve
            {
                new CurvePoint(100f, 0.6f),
                new CurvePoint(500f, 1f),
                new CurvePoint(2000f, 2f)
            };

            [HarmonyPrefix]
            public static bool Prefix(ref bool __result, IncidentParms parms)
            {
                Map map = (Map)parms.target;
                int optimalDetectionLimit = 0;
                foreach (Pawn pawn in map.mapPawns.ColonyAnimals)
                {
                    if (pawn.GetComp<ThingComp_PerceptorVision>() is ThingComp_PerceptorVision perceptorVision)
                    {
                        optimalDetectionLimit += perceptorVision.Props.plantRatioForOptimalDetection;
                    }
                }
                if (optimalDetectionLimit == 0)
                {
                    return true;
                }

                List<Thing> plants = map.listerThings.ThingsInGroup(ThingRequestGroup.Plant);
                int plantsBlightable = 0;
                for (int i = plants.Count - 1; i >= 0; i--)
                {
                    if (!((Plant)plants[i]).BlightableNow)
                    {
                        plants.RemoveAt(i);
                    }
                    else
                    {
                        plantsBlightable++;
                    }
                }
                if (plants.TryRandomElement(out Thing incidentEpicenter))
                {
                    float radiusFactorFromPerceptors = 1 - Mathf.Min((float)optimalDetectionLimit / plantsBlightable, 0.8f);
                    float radiusFactor = RadiusFactorPerPointsCurve.Evaluate(parms.points) * radiusFactorFromPerceptors;
                    Room room = incidentEpicenter.GetRoom();
                    for (int i = GenRadial.NumCellsInRadius(Radius * radiusFactor); i >= 0; i--)
                    {
                        IntVec3 intVec = incidentEpicenter.Position + GenRadial.RadialPattern[i];
                        if (intVec.InBounds(map) && intVec.GetRoom(map) == room)
                        {
                            Plant blightablePlant = BlightUtility.GetFirstBlightableNowPlant(intVec, map);
                            if (blightablePlant != null && blightablePlant.def == incidentEpicenter.def && Rand.Chance(BlightChancePerRadius.Evaluate(blightablePlant.Position.DistanceTo(incidentEpicenter.Position) / radiusFactor)))
                            {
                                blightablePlant.CropBlighted();
                            }
                        }
                    }
                    Find.LetterStack.ReceiveLetter("JDG_PerceptorDetectedBlight".Translate(new NamedArgument(incidentEpicenter.def, "PLANTDEF")), "JDG_PerceptorDetectedBlightDesc".Translate(new NamedArgument(incidentEpicenter.def, "PLANTDEF"), radiusFactorFromPerceptors.ToStringPercent()), LetterDefOf.NegativeEvent, incidentEpicenter);

                    __result = true;
                    return false;
                }
                __result = false;
                return false;
            }
        }
    }
}
