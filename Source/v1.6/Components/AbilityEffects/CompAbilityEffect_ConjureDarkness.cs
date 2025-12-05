using RimWorld;
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
            GenSpawn.Spawn(JDG_ThingDefOf.ABF_Thing_BlackVeil, target.Cell, Caster.Map);
            if (Props.destClamorType != null)
            {
                GenClamor.DoClamor(Caster, target.Cell, Props.destClamorRadius, Props.destClamorType);
            }
        }
    }
}
