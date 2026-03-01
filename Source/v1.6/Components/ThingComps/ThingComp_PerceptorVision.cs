using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class ThingComp_PerceptorVision : ThingComp
    {
        public const float scryingRadius = 11f;

        public int ticksToNextScrying = 0;

        public int ticksToNextManualScrying = 0;

        public ThingCompProperties_PerceptorVision Props => (ThingCompProperties_PerceptorVision)props;

        public override void CompTickInterval(int delta)
        {
            if (ticksToNextScrying <= 0 && parent.Spawned)
            {
                List<Thing> viableScryingTargets = new List<Thing>();
                Room room = parent.GetRoom();
                foreach (Thing target in GenRadial.RadialDistinctThingsAround(parent.Position, parent.Map, scryingRadius, useCenter: true))
                {
                    if (target.GetRoom() != room)
                    {
                        continue;
                    }
                    if (CanScry(target))
                    {
                        viableScryingTargets.Add(target);
                    }
                }

                // In the event that nothing is scriable now, try again in an hour.
                if (viableScryingTargets.Count == 0)
                {
                    ticksToNextScrying = GenDate.TicksPerHour;
                    return;
                }

                if (viableScryingTargets.TryRandomElementByWeight(thing =>
                {
                    if (thing is Pawn pawn)
                    {
                        if (MetalhorrorUtility.IsInfected(pawn))
                        {
                            return 10f;
                        }
                        // Creepjoiners should not be automatically scried.
                        else if (pawn.IsCreepJoiner && !pawn.creepjoiner.Disabled)
                        {
                            return 0f;
                        }
                        else if (pawn.Faction != Faction.OfPlayer && pawn.guest?.Recruitable == true)
                        {
                            return 2f;
                        }
                        else
                        {
                            return 1f;
                        }
                    }
                    else
                    {
                        return 1f;
                    }
                }, out Thing scryingTarget))
                {
                    ticksToNextScrying = Props.daysBetweenScrying.RandomInRange;
                    DoScrying(scryingTarget);
                }
                // In the event that nothing was scried, try again in an hour.
                else
                {
                    ticksToNextScrying = GenDate.TicksPerHour;
                }
            }
            else
            {
                ticksToNextScrying = Math.Max(0, ticksToNextScrying - delta);
                ticksToNextManualScrying = Math.Max(0, ticksToNextManualScrying - delta);
            }
        }

        public override void PostPostMake()
        {
            ResetScryingTimer();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action ready = new Command_Action
                {
                    defaultLabel = "DEV: Ready",
                    action = delegate
                    {
                        ticksToNextScrying = 0;
                    }
                };
                yield return ready;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksToNextScrying, "JDG_ticksToNextScrying", 0);
            Scribe_Values.Look(ref ticksToNextManualScrying, "JDG_ticksToNextManualScrying", 0);
        }

        public void ResetScryingTimer()
        {
            ticksToNextScrying = Props.daysBetweenScrying.RandomInRange;
        }

        public bool CanScry(Thing thing)
        {
            if (thing is Pawn pawn && pawn.RaceProps.Humanlike && !pawn.HostileTo(parent) && (!(pawn.GetPerceptorTracker() is PerceptorTracker tracker) || GenTicks.TicksAbs - tracker.tickPerceived > GenDate.TicksPerDay * 5))
            {
                return true;
            }
            else if (thing.TryGetComp<CompFoodPoisonable>() is CompFoodPoisonable compFoodPoisonable && compFoodPoisonable.PoisonPercent > 0)
            {
                return true;
            }
            else if (thing.TryGetComp<CompMetalhorrorInfectible>() is CompMetalhorrorInfectible compMetalhorrorInfectible && compMetalhorrorInfectible.Infections > 0)
            {
                return true;
            }
            return false;
        }

        public void DoScrying(Thing thing)
        {
            if (thing is Pawn pawn)
            {
                if (MetalhorrorUtility.IsInfected(pawn))
                {
                    TaggedString taggedString = "JDG_MetalhorrorReasonPerceptorDetection".Translate(pawn.Named("INFECTED"));
                    TaggedString taggedString2 = "MetalhorrorNoticedDetailsAppended".Translate();
                    TaggedString taggedString3 = "JDG_PerceptorDetectedDesc".Translate(pawn.Named("INFECTED"));
                    taggedString3 += $"\n\n{taggedString2}";
                    MetalhorrorUtility.Detect(pawn, taggedString, taggedString3, 0f);
                }
                else if (pawn.IsCreepJoiner && !pawn.creepjoiner.Disabled && (pawn.creepjoiner.downside != null || pawn.creepjoiner.aggressive != null || pawn.creepjoiner.rejection != null))
                {
                    Find.LetterStack.ReceiveLetter("JDG_PerceptorDetectedCreepjoinerFlaws".Translate(pawn.Named("CREEPY")), "JDG_PerceptorDetectedCreepjoinerFlawsDesc".Translate(pawn.creepjoiner.downside.label, pawn.creepjoiner.aggressive, pawn.creepjoiner.rejection, pawn.Named("CREEPY")), LetterDefOf.NeutralEvent, thing);
                }
                else if (PawnUtility.ShouldSendNotificationAbout(pawn))
                {
                    pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(JDG_ThoughtDefOf.ABF_Thought_Justiciar_SecretRevealed);
                    Messages.Message("JDG_PerceptorDetectedMinorSecret".Translate(pawn.NameShortColored), pawn, MessageTypeDefOf.NegativeEvent);
                }
                JDG_Utils.UpdatePerceptorTracker(pawn, GenTicks.TicksAbs);
            }
            else if (thing.TryGetComp<CompFoodPoisonable>() is CompFoodPoisonable compFoodPoisonable && compFoodPoisonable.PoisonPercent > 0)
            {
                Find.LetterStack.ReceiveLetter("JDG_PerceptorDetectedPoison".Translate(), "JDG_PerceptorDetectedPoisonDesc".Translate(), LetterDefOf.ThreatSmall, thing);
            }
            else if (thing.TryGetComp<CompMetalhorrorInfectible>() is CompMetalhorrorInfectible compMetalhorrorInfectible && compMetalhorrorInfectible.Infections > 0)
            {
                Find.LetterStack.ReceiveLetter("JDG_PerceptorDetectedMetalhorrorFoodImplant".Translate(), "JDG_PerceptorDetectedMetalhorrorFoodImplantDesc".Translate(), LetterDefOf.ThreatSmall, thing);
            }
        }
    }
}
