using Verse;

namespace ArtificialBeings
{
    public class ThingCompProperties_NegativelyAffectJusticiars : CompProperties
    {
        public IntRange damageAmountRange;

        public DamageDef damageDef;

        public HediffDef addsHediff;

        public ThingCompProperties_NegativelyAffectJusticiars()
        {
            compClass = typeof(ThingComp_NegativelyAffectJusticiars);
        }
    }
}
