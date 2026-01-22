using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class JobDriver_Clone : JobDriver_JusticiarBase
    {
        protected override float VeilRadius => 1.9f;

        protected override void WaitInitAction()
        {
            if (TargetThing is Pawn pawnToClone)
            {
                PawnUtility.ForceWait(pawnToClone, WaitTicks, pawn);
            }
        }

        protected override void EndInitAction()
        {
            if (TargetThing is Pawn pawnToClone)
            {
                Pawn clonedPawn = PawnGenerator.GeneratePawn(pawnToClone.kindDef, pawnToClone.Faction);
                // Cloned animals should copy all learned skills as well as assigned master from the original source, if applicable.
                if (pawnToClone.IsColonyAnimal)
                {
                    Pawn_TrainingTracker originalTracker = pawnToClone.training;
                    Pawn_TrainingTracker newTracker = clonedPawn.training;
                    foreach (TrainableDef trainableDef in DefDatabase<TrainableDef>.AllDefsListForReading)
                    {
                        if (originalTracker.HasLearned(trainableDef))
                        {
                            newTracker.Train(trainableDef, null, true);
                        }
                        if (originalTracker.GetWanted(trainableDef))
                        {
                            newTracker.SetWantedRecursive(trainableDef, true);
                        }
                        clonedPawn.playerSettings.Master = pawnToClone.playerSettings.Master; // Can be null.
                    }
                }
                // Cloned mechanoids should, if possible, automatically link to the same mechanitor as the original.
                else if (pawnToClone.IsColonyMechPlayerControlled)
                {
                    Pawn overseer = MechanitorUtility.GetOverseer(pawnToClone);
                    if (overseer != null && MechanitorUtility.CanControlMech(overseer, clonedPawn))
                    {
                        overseer.relations.AddDirectRelation(PawnRelationDefOf.Overseer, clonedPawn);
                    }
                }
                GenSpawn.Spawn(clonedPawn, pawnToClone.Position, pawnToClone.Map);
            }
            else
            {
                UmbralClonableExtension extension = TargetThing.def.GetModExtension<UmbralClonableExtension>();
                Thing thing = ThingMaker.MakeThing(TargetThing.def, TargetThing.Stuff);
                if (extension.useStackCount)
                {
                    thing.stackCount = TargetThing.stackCount;
                }
                // Copy the quality of the item, if applicable
                if (TargetThing.TryGetQuality(out var quality))
                {
                    thing.TryGetComp<CompQuality>()?.SetQuality(quality, ArtGenerationContext.Colony);
                }
                GenPlace.TryPlaceThing(thing, TargetThing.Position, TargetThing.Map, ThingPlaceMode.Near);
            }
            // This costs the justiciar who carried out the cloning process an amount of favor. The cost is expected to be a non-zero, positive number.
            pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>().NotifyFavorLost(JDG_Utils.FavorCostToClone(TargetThing));
        }
    }
}
