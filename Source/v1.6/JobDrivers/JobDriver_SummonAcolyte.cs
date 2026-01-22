using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class JobDriver_SummonAcolyte : JobDriver_JusticiarBase
    {
        protected override int WaitTicks => GenDate.TicksPerHour * 4;

        protected override float VeilRadius => 2.9f;

        protected override void EndInitAction()
        {
            Pawn inductee = PawnGenerator.GeneratePawn(Faction.OfPlayer.def.basicMemberKind, Faction.OfPlayer);
            // Locate a place for the pawn to arrive from. If there is no walkable place, give them a drop pod.
            if (CellFinder.TryFindRandomEdgeCellWith(p => (pawn.Map.TileInfo.AllowRoofedEdgeWalkIn || !pawn.Map.roofGrid.Roofed(p)) && p.Walkable(pawn.Map), pawn.Map, CellFinder.EdgeRoadChance_Friendly, out IntVec3 spawnCenter))
            {
                GenSpawn.Spawn(inductee, spawnCenter, pawn.Map);
            }
            else
            {
                DropCellFinder.TryFindDropSpotNear(pawn.Position, pawn.Map, out IntVec3 dropPoint, false, false);
                TradeUtility.SpawnDropPod(dropPoint, pawn.Map, inductee);
            }
            // Inducting a new acolyte can fulfill an ambition for the person who inducted them.
            if (pawn.health.hediffSet.TryGetHediff<Hediff_Ambition_RecruitmentMotivation>(out var hediff) && !hediff.complete)
            {
                hediff.NotifySucceeded();
            }
            inductee.health.AddHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_Acolyte);
            // The inductee views the justiciar as an arbiter. The justiciars views the new acolyte as their neophyte.
            inductee.relations.AddDirectRelation(JDG_PawnRelationDefOf.ABF_PawnRelation_Justiciar_Arbiter, pawn);
            pawn.relations.AddDirectRelation(JDG_PawnRelationDefOf.ABF_PawnRelation_Justiciar_Neophyte, inductee);
            // This costs the justiciar an amount of favor. A colonist is worth 50 favor, and making them an acolyte is 10, so it breaks even.
            pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>().NotifyFavorLost(60f);
        }
    }
}
