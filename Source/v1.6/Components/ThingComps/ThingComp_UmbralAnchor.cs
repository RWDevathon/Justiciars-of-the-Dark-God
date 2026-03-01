using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class ThingComp_UmbralAnchor : ThingComp
    {
        public PawnKindDef nextToSpawn;

        public Pawn currentCreature;

        public bool notificationSent = false;

        public int ticksToNextReady = 0;

        public bool ShouldNotify => currentCreature == null && !notificationSent;

        public ThingCompProperties_UmbralAnchor Props => (ThingCompProperties_UmbralAnchor)props;

        public override void PostPostMake()
        {
            ResetTimer();
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (nextToSpawn == null)
            {
                yield return new FloatMenuOption("JDG_SelectCreatureType".Translate(), delegate
                {
                    Find.WindowStack.Add(new Dialog_SelectCreatureKind(this));
                });
            }

            if (ticksToNextReady <= 0)
            {
                yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("JDG_SummonCreature".Translate(), delegate
                {
                    Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_SummonCreature, parent);
                    selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                }), selPawn, parent);
            }
            else
            {
                yield return new FloatMenuOption("JDG_SpawnReadyIn".Translate(ticksToNextReady.ToStringTicksToPeriod()), null);
            }
            yield break;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action
            {
                defaultLabel = "JDG_SelectCreatureType".Translate(),
                defaultDesc = "JDG_SelectCreatureTypeDesc".Translate(),
                icon = (nextToSpawn != null) ? Widgets.GetIconFor(nextToSpawn.race) : ContentFinder<Texture2D>.Get("UI/Gizmos/UpgradeDryads"),
                action = delegate
                {
                    Find.WindowStack.Add(new Dialog_SelectCreatureKind(this));
                }
            };

            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action ready = new Command_Action
                {
                    defaultLabel = "DEV: Ready",
                    action = delegate
                    {
                        ticksToNextReady = 0;
                    }
                };
                yield return ready;
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (currentCreature != null && !currentCreature.Dead)
            {
                currentCreature.Kill(null);
            }
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            ticksToNextReady -= GenTicks.TickRareInterval;
            if (ticksToNextReady <= 0 && ShouldNotify)
            {
                Messages.Message("JDG_CreatureSpawnReady".Translate(), parent, MessageTypeDefOf.PositiveEvent);
                notificationSent = true;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref nextToSpawn, "JDG_nextToSpawn");
            Scribe_Values.Look(ref ticksToNextReady, "JDG_ticksToNextReady");
            Scribe_Values.Look(ref notificationSent, "JDG_notificationSent", false);
        }

        public override string CompInspectStringExtra()
        {
            if (ticksToNextReady <= 0)
            {
                return "JDG_SpawnReady".Translate();
            }
            return "JDG_SpawnReadyIn".Translate(ticksToNextReady.ToStringTicksToPeriod());
        }

        public void ResetTimer()
        {
            ticksToNextReady = (int)(Props.daysBetweenSpawns * GenDate.TicksPerDay);
        }
    }
}
