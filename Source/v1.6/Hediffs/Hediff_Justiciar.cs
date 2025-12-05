using RimWorld;
using Verse;

namespace ArtificialBeings
{
    // Justiciars have specific behaviors that need to happen only for them. They should ALWAYS have a Hediff with this class.
    public class Hediff_Justiciar : HediffWithComps
    {
        // This should not be removed.
        public override bool ShouldRemove => false;

        // Downed justiciars of non-player factions instantly die.
        public override void Notify_Downed()
        {
            base.Notify_Downed();
            if (pawn.Faction != Faction.OfPlayer)
            {
                pawn.Kill(null);
            }
        }

        // Justiciars have no corpse, and can accidentally delete their inventory on death in rare circumstances.
        // This will spawn a single tile of darkness where they died, and drop their inventory.
        public override void Notify_PawnKilled()
        {
            base.Notify_PawnKilled();
            pawn.DropAndForbidEverything();
            BlackVeil veil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
            veil.Radius = 0.9f;
            veil.ticksLeft = 600;
            GenSpawn.Spawn(veil, pawn.PositionHeld, pawn.MapHeld);
        }

        // Justiciars should have all tendable injuries automatically tended upon taking damage. The tend quality should always be a flat 100%.
        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
            foreach (Hediff hediff in pawn.health.hediffSet.GetHediffsTendable())
            {
                if (hediff is Hediff_Injury hediffInjury && !hediff.IsTended())
                {
                    // Set it to 200% quality, with 100% max quality, to avoid randomness setting it lower. Set a batchPosition of 1 to make it tend silently.
                    hediffInjury.Tended(2f, 1f, 1);
                }
            }
        }

        public override void TickInterval(int delta)
        {
            if (pawn.Spawned)
            {
                Severity = pawn.Map.glowGrid.GroundGlowAt(pawn.Position);
            }
            base.TickInterval(delta);
        }

        // If this is removed for some reason, put it back.
        public override void PostRemoved()
        {
            pawn.health.AddHediff(def);
        }
    }
}
