using RimWorld;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    // Shadespirits are animals/mechanoids of the player that have been granted a justiciar-like ability. They are treated like justiciars in some ways.
    public class Hediff_ShadeSpirit : HediffWithComps
    {
        // This should not remove itself from a pawn under any circumstances.
        public override bool ShouldRemove => false;

        // Shadespirits will spawn a single tile of darkness where they died.
        public override void Notify_PawnKilled()
        {
            base.Notify_PawnKilled();
            BlackVeil veil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
            veil.Radius = 0.9f;
            veil.ticksLeft = 600;
            GenSpawn.Spawn(veil, pawn.PositionHeld, pawn.MapHeld);
        }

        public override void TickInterval(int delta)
        {
            if (pawn.Spawned)
            {
                Severity = pawn.Map.glowGrid.GroundGlowAt(pawn.Position);
            }
            base.TickInterval(delta);
        }

        public override void Tick()
        {
            if (pawn.IsHashIntervalTick(6) && (pawn.Spawned || pawn.ParentHolder is Pawn_CarryTracker))
            {
                Map map = pawn.MapHeld;
                if (map != null)
                {
                    Vector3 location = pawn.DrawPos;
                    if (location.ShouldSpawnMotesAt(map))
                    {
                        float facingAngle = pawn.Rotation.AsAngle;
                        FleckCreationData dataStatic = FleckMaker.GetDataStatic(location, map, JDG_FleckDefOf.ABF_Fleck_Justiciar_Smoke, Rand.Range(1.5f, 3f) * pawn.Graphic.drawSize.magnitude);
                        dataStatic.rotationRate = Rand.Range(-90f, 90f);
                        dataStatic.velocityAngle = Rand.Range(facingAngle - 30, facingAngle + 30);
                        dataStatic.velocitySpeed = Rand.Range(0.05f, 0.2f);
                        map.flecks.CreateFleck(dataStatic);
                    }
                }
            }
            base.Tick();
        }

        // This hediff identifies shadespirits, and they should be added to the cache of all known shadespirits on being made one.
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            JDG_Utils.ShadeSpirits.Add(pawn);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            // This hediff identifies shadespirits, and they should be re-added to the cache of all known shadespirits on loading saves.
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                JDG_Utils.ShadeSpirits.Add(pawn);
            }
        }
    }
}
