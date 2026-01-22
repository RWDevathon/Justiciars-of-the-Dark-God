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
