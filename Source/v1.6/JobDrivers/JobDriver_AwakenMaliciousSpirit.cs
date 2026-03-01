using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class JobDriver_AwakenMaliciousSpirit : JobDriver_JusticiarBase
    {
        protected override int WaitTicks => GenTicks.TicksPerRealSecond;

        protected override float VeilRadius => 1.4f;

        protected override void EndInitAction()
        {
            TargetThing.TryGetComp<ThingComp_MaliciousSpirit>().readyTick = GenTicks.TicksGame + GenDate.TicksPerTwelfth;
            foreach (Thing target in GenRadial.RadialDistinctThingsAround(TargetThing.Position, pawn.Map, 6.9f, useCenter: true))
            {
                if (target is Pawn pawn && !JDG_Utils.IsJusticiar(pawn) && !JDG_Utils.IsShadeSpirit(pawn))
                {
                    pawn.health.AddHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_ChokingDarkness);
                    pawn.stances.stunner.StunFor(GenTicks.TicksPerRealSecond * 5, TargetThing, false);
                }
            }
            BlackVeil veil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
            veil.Radius = 6.9f;
            veil.ticksLeft = GenDate.TicksPerDay / 2;
            GenSpawn.Spawn(veil, TargetThing.Position, TargetThing.Map);
            Effecter effecter = JDG_EffecterDefOf.ABF_Effecter_Justiciar_MaliciousSpirit.Spawn(TargetThing, TargetThing.Map);
            effecter.Cleanup();
        }
    }
}
