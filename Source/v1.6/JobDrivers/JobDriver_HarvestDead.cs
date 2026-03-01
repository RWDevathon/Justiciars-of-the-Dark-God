using RimWorld;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    public class JobDriver_HarvestDead : JobDriver_JusticiarBase
    {
        protected override void EndInitAction()
        {
            ThingComp_HeraldHarvest harvestComp = pawn.GetComp<ThingComp_HeraldHarvest>();
            if (TargetThing is Corpse corpse)
            {
                if (TargetThing is UnnaturalCorpse unnaturalCorpse)
                {
                    if (Rand.Chance(0.6f))
                    {
                        harvestComp.NotifyCorpseConsumed(corpse);
                        Messages.Message("JDG_UnnaturalCorpseHarvest_Successful".Translate(), pawn, MessageTypeDefOf.PositiveEvent);
                        if (unnaturalCorpse.Tracker.Haunted is Pawn victim)
                        {
                            Current.Game.GetComponent<GameComponent_Anomaly>().RemoveCorpseTracker(victim);
                        }
                        unnaturalCorpse.Destroy();
                    }
                    else
                    {
                        Messages.Message("JDG_UnnaturalCorpseHarvest_Failure".Translate(), TargetThing, MessageTypeDefOf.NegativeEvent);
                        GenExplosion.DoExplosion(TargetThing.Position, TargetThing.Map, 5.9f, DamageDefOf.Psychic, TargetThing, 15, explosionSound: JDG_SoundDefOf.MeatExplosionLarge, postExplosionSpawnThingDef: ThingDefOf.Filth_Blood, postExplosionSpawnChance: 0.6f, postExplosionSpawnThingCount: 3);
                        // Bring on the blood rain.
                        GameConditionManager gameConditionManager = TargetThing.Map.GameConditionManager;
                        GameConditionDef gameConditionDef = GameConditionDefOf.BloodRain;
                        int duration = Mathf.RoundToInt(new FloatRange(0.33f, 1.5f).RandomInRange * GenDate.TicksPerDay);
                        GameCondition gameCondition = GameConditionMaker.MakeCondition(gameConditionDef, duration);
                        gameConditionManager.RegisterCondition(gameCondition);
                        if (!pawn.Dead)
                        {
                            pawn.Kill(null);
                        }
                    }
                }
                else
                {
                    harvestComp.NotifyCorpseConsumed(corpse);
                    TargetThing.Destroy();
                }
            }
            else
            {
                harvestComp.NotifySkullsConsumed(TargetThing);
                TargetThing.Destroy();
            }
        }
    }
}
