using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class ThingComp_NegativelyAffectJusticiars : ThingComp
    {
        public ThingCompProperties_NegativelyAffectJusticiars Props => (ThingCompProperties_NegativelyAffectJusticiars)props;

        public override void PrePostIngested(Pawn ingester)
        {
            base.PrePostIngested(ingester);
            if (JDG_Utils.IsJusticiar(ingester))
            {
                if (Props.damageDef != null && Props.damageAmountRange != null)
                {
                    DamageInfo damageInfo = new DamageInfo(Props.damageDef, Props.damageAmountRange.RandomInRange);
                    damageInfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Inside);
                    ingester.TakeDamage(damageInfo);
                }
                if (Props.addsHediff != null)
                {
                    ingester.health.AddHediff(Props.addsHediff);
                }
                if (ingester.Spawned)
                {
                    EffecterDefOf.MeatExplosion.Spawn(ingester.Position, ingester.Map).Cleanup();
                    CellRect cellRect = CellRect.CenteredOn(ingester.Position, 3, 3).ClipInsideMap(ingester.Map);
                    for (int i = Rand.Range(6, 18); i >= 0; i--)
                    {
                        IntVec3 randomCell = cellRect.RandomCell;
                        GenSpawn.Spawn(ingester.RaceProps.BloodDef ?? ThingDefOf.Filth_Blood, randomCell, ingester.Map);
                    }
                }
                if (PawnUtility.ShouldSendNotificationAbout(ingester))
                {
                    Messages.Message("JDG_JusticiarNegativelyAffected".Translate(parent.LabelShort, ingester.NameShortColored), ingester, MessageTypeDefOf.NegativeEvent, false);
                }
            }
        }
    }
}
