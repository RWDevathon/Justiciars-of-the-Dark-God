using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class ThingComp_HeraldHarvest : ThingComp
    {
        public const int BodySizeForSeed = 40;

        public const float SkullsToBodySizeRatio = 0.2f;

        public float progressToSeed = 0f;

        public bool notificationSent = false;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (BodySizeForSeed <= progressToSeed)
            {
                Command_Action dropSeed = new Command_Action
                {
                    defaultLabel = "JDG_DropSeed".Translate(),
                    defaultDesc = "JDG_DropSeedDesc".Translate(),
                    icon = Widgets.GetIconFor(JDG_ThingDefOf.ABF_Thing_Justiciar_CorruptedGauranlenSeed),
                    action = delegate
                    {
                        GenSpawn.Spawn(JDG_ThingDefOf.ABF_Thing_Justiciar_CorruptedGauranlenSeed, parent.Position, parent.Map);
                        progressToSeed -= BodySizeForSeed;
                        notificationSent = false;
                    }
                };
                yield return dropSeed;
            }

            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action ready = new Command_Action
                {
                    defaultLabel = "DEV: Ready",
                    action = delegate
                    {
                        progressToSeed = BodySizeForSeed;
                    }
                };
                yield return ready;
            }
        }

        public override string CompInspectStringExtra()
        {
            if (BodySizeForSeed <= progressToSeed)
            {
                return "JDG_SeedReady".Translate();
            }
            return "JDG_ProgressToNextSeed".Translate(progressToSeed.ToString("F2"), BodySizeForSeed);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref progressToSeed, "JDG_progressToSeed", 0f);
            Scribe_Values.Look(ref notificationSent, "JDG_notificationSent", false);
        }

        // Missing body parts affect the total harvest, which is based on body size.
        public void NotifyCorpseConsumed(Corpse corpse)
        {
            // Unnatural corpses are worth an entire seed.
            if (corpse is UnnaturalCorpse)
            {
                progressToSeed += BodySizeForSeed;
            }
            else
            {
                progressToSeed += corpse.InnerPawn.health.hediffSet.GetCoverageOfNotMissingNaturalParts(corpse.InnerPawn.RaceProps.body.corePart) * corpse.InnerPawn.BodySize;
            }
            if (BodySizeForSeed <= progressToSeed && !notificationSent)
            {
                Messages.Message("JDG_CreatureSpawnReady".Translate(), parent, MessageTypeDefOf.PositiveEvent);
                notificationSent = true;
            }
        }

        public void NotifySkullsConsumed(Thing thing)
        {
            if (thing.def != ThingDefOf.Skull)
            {
                Log.Warning($"A herald was told it ate skulls, but was provided {thing.def} instead. It is displeased.");
                return;
            }
            progressToSeed += SkullsToBodySizeRatio * thing.stackCount;
            if (BodySizeForSeed <= progressToSeed && !notificationSent)
            {
                Messages.Message("JDG_CreatureSpawnReady".Translate(), parent, MessageTypeDefOf.PositiveEvent);
                notificationSent = true;
            }
        }
    }
}
