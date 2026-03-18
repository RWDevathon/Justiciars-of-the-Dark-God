using HarmonyLib;
using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class Pawn_MutantTracker_Patch
    {
        // Justiciars that turn into mutants are instantly destroyed in a visceral explosion of gore and shadow. The Dark God does not tolerate heresy or traitors.
        // An exception is the Awoken corpse, as that is merely a profane copy of a Justiciar rather than a Justiciar itself.
        [HarmonyPatch(typeof(Pawn_MutantTracker), nameof(Pawn_MutantTracker.Turn))]
        public class Pawn_MutantTracker_Turn_Patch
        {
            [HarmonyPostfix]
            public static void Listener(Pawn ___pawn, MutantDef ___def)
            {
                if (JDG_Utils.IsJusticiar(___pawn))
                {
                    if (___def == MutantDefOf.AwokenCorpse)
                    {
                        Hediff_Justiciar justiciarHediff = ___pawn.health.hediffSet.GetFirstHediff<Hediff_Justiciar>();
                        if (justiciarHediff != null)
                        {
                            JDG_Utils.Justiciars.Remove(___pawn);
                            ___pawn.health.RemoveHediff(justiciarHediff);
                            return;
                        }
                    }

                    IntVec3 position = ___pawn.Position;
                    Map map = ___pawn.Map;
                    JDG_EffecterDefOf.ABF_Effecter_Justiciar_DarkAnnihilation.Spawn(position, map).Cleanup();
                    EffecterDefOf.MeatExplosionExtraLarge.Spawn(position, map).Cleanup();
                    CellRect cellRect = CellRect.CenteredOn(position, 3, 3).ClipInsideMap(map);
                    for (int i = Rand.Range(1, 4); i >= 0; i--)
                    {
                        IntVec3 randomCell = cellRect.RandomCell;
                        if (!map.thingGrid.ThingsListAtFast(randomCell).ContainsAny(thing => thing.def == JDG_ThingDefOf.ABF_Thing_BlackVeil))
                        {
                            BlackVeil veil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
                            veil.Radius = Rand.Range(0.4f, 1.4f);
                            veil.ticksLeft = 1200;
                            veil.opacityRange = new FloatRange(0.2f, 0.5f);
                            GenSpawn.Spawn(veil, randomCell, map);
                        }
                    }
                    ___pawn.Destroy();
                    GenExplosion.DoExplosion(position, map, 2.9f, DamageDefOf.Psychic, ___pawn, 5, 0f, JDG_SoundDefOf.MeatExplosionLarge, preExplosionSpawnThingDef: ThingDefOf.Filth_TwistedFlesh, preExplosionSpawnChance: 0.5f, postExplosionSpawnThingDef: ThingDefOf.Filth_Blood, postExplosionSpawnChance: 0.5f);
                    if (PawnUtility.ShouldSendNotificationAbout(___pawn))
                    {
                        Find.LetterStack.ReceiveLetter("JDG_Smited".Translate(___pawn.LabelShortCap), "JDG_SmitedDesc".Translate(___pawn.LabelShortCap), LetterDefOf.NegativeEvent);
                    }
                }
            }
        }

        // Justiciars that turn into mutants are instantly destroyed in a visceral explosion of gore and shadow.
        // Their clothes which are dropped should always be tainted, even if the mutant def suggests they shouldn't.
        [HarmonyPatch(typeof(Pawn_MutantTracker), "HandleEquipment")]
        public class Pawn_MutantTracker_HandleEquipment_Patch
        {
            [HarmonyPrefix]
            public static bool Prefix(Pawn ___pawn, MutantDef ___def)
            {
                if (___pawn.apparel != null && JDG_Utils.IsJusticiar(___pawn) && !___def.isConsideredCorpse)
                {
                    foreach (Apparel item in ___pawn.apparel.WornApparel)
                    {
                        if (item.def.apparel.careIfWornByCorpse)
                        {
                            item.WornByCorpse = true;
                        }
                    }
                }
                return true;
            }
        }
    }
}