using RimWorld;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    // Conjures a cloud of darkness at a specified location.
    public class CompAbilityEffect_ConjureDarkness : CompAbilityEffect
    {
        public new CompProperties_EffectWithDest Props => (CompProperties_EffectWithDest)props;

        private Pawn Caster => parent.pawn;

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (!target.IsValid)
            {
                return;
            }
            BlackVeil blackVeil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
            Hediff_Justiciar justiciarHediff = Caster.health.hediffSet.GetFirstHediff<Hediff_Justiciar>();
            blackVeil.Radius = 2.49f + Mathf.Clamp(justiciarHediff.FavorCurrent / 100f, -2f, 4f);
            blackVeil.ticksLeft = 60;
            justiciarHediff.NotifyNewMaintainee(blackVeil);
            GenSpawn.Spawn(blackVeil, target.Cell, Caster.Map);
            if (Props.destClamorType != null)
            {
                GenClamor.DoClamor(Caster, target.Cell, Props.destClamorRadius, Props.destClamorType);
            }
        }
    }
}
