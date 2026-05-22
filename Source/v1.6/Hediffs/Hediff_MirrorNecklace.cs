using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    // This Hediff dynamically adjusts a pain offset based on a gizmo set value by the player. It also provides a means to completely disable pain for a time before destroying itself.
    public class Hediff_MirrorNecklace : Hediff
    {
        // Once the pain transmutation begins, this hediff prevents all pain and ticks down a destruction timer.
        public bool painTransmutationStarted = false;
        public int ticksUntilDestroyed = 0;
        public const float painTransmutationFavorCost = 10f;
        public const int painTransmutationDurationHours = 4;

        public Thing connectedThing;

        // This should only remove itself from the pawn when the apparel informs it to do so.
        public override bool ShouldRemove => false;

        public override HediffStage CurStage => null;

        public override float PainFactor => painTransmutationStarted ? 0f : 1f;

        public override float PainOffset => painTransmutationStarted ? 0f : Severity;

        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (painTransmutationStarted)
            {
                ticksUntilDestroyed -= delta;
                if (ticksUntilDestroyed <= 0)
                {
                    pawn.health.RemoveHediff(this);
                }
            }
            // Just in case this is equipped on a non-player pawn, make absolutely sure they are not harming themselves.
            else if (!pawn.IsFreeColonist || !pawn.IsPlayerControlled)
            {
                pawn.health.RemoveHediff(this);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref painTransmutationStarted, "JDG_painTransmutationStarted", false);
            Scribe_Values.Look(ref ticksUntilDestroyed, "JDG_ticksUntilDestroyed", 0);
            Scribe_References.Look(ref connectedThing, "JDG_connectedNecklace", false);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (pawn.IsFreeColonist && pawn.IsPlayerControlled)
            {
                Command_Action setPainLevel = new Command_Action
                {
                    defaultLabel = "JDG_SetPainLevel".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner"),
                    defaultDesc = "JDG_SetPainLevelDesc".Translate(),
                    action = delegate
                    {
                        Dialog_Slider dialog_Slider = new Dialog_Slider(severity => "JDG_SetPainLevelPercent".Translate(severity), 0, 100, delegate (int value)
                        {
                            Severity = value / 100f;
                            // Force a recache and recalculation
                            pawn.health.hediffSet.DirtyPainCache();
                            pawn.health.CheckForStateChange(null, this);
                        }, (int)(Severity * 100));
                        Find.WindowStack.Add(dialog_Slider);
                    }
                };
                yield return setPainLevel;

                if (!painTransmutationStarted)
                {
                    Command_Action startPainTransmutation = new Command_Action
                    {
                        defaultLabel = "JDG_BeginPainTransmutation".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner"),
                        defaultDesc = "JDG_BeginPainTransmutationDesc".Translate(painTransmutationFavorCost.ToString("F0"), painTransmutationDurationHours),
                        action = delegate
                        {
                            painTransmutationStarted = true;
                            ticksUntilDestroyed = GenDate.TicksPerHour * 4;
                            connectedThing?.Destroy();
                            // Force a recache and recalculation
                            pawn.health.hediffSet.DirtyPainCache();
                            pawn.health.CheckForStateChange(null, this);
                            pawn.health.hediffSet.GetFirstHediff<Hediff_Devotee>()?.NotifyFavorLost(painTransmutationFavorCost);
                        }
                    };
                    if (pawn.health.hediffSet.GetFirstHediff<Hediff_Devotee>().FavorCurrent < painTransmutationFavorCost)
                    {
                        startPainTransmutation.Disabled = true;
                        startPainTransmutation.disabledReason = "JDG_CannotBeginTransmutationInsufficientFavor".Translate(painTransmutationFavorCost.ToString("F0"));
                    }
                    yield return startPainTransmutation;
                }
            }

            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action resetFavor = new Command_Action
                {
                    defaultLabel = "DEV: Set destruction imminent",
                    action = delegate
                    {
                        connectedThing?.Destroy();
                        painTransmutationStarted = true;
                        ticksUntilDestroyed = 60;
                    }
                };
                yield return resetFavor;
            }
            yield break;
        }

        public override string GetTooltip(Pawn pawn, bool showHediffsDebugInfo)
        {
            if (painTransmutationStarted)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(base.GetTooltip(pawn, showHediffsDebugInfo));
                stringBuilder.Append("\n\n" + "JDG_TimeUntilDestroyed".Translate(ticksUntilDestroyed.ToStringTicksToPeriod()));
                return stringBuilder.ToString();
            }
            return base.GetTooltip(pawn, showHediffsDebugInfo);
        }
    }
}
