using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    // Base class for Hediff_Justiciar and Hediff_Acolyte where functionality is shared.
    public class Hediff_Devotee : HediffWithComps
    {
        // Measurement of how pious the devotee has proven themselves to be as of this moment.
        protected float favorCurrent = 0f;

        // Tracker for when the devotee was last in pain. Used for various calculations.
        public int tickLastInPain = 0;

        // Tracker for how much favor to provide when next allocating. Doing so too frequently is unnecessary and could impact performance.
        public float painBaseFavorToProvide = 0f;
        public int nextPainFavorUpdateTick = 0;

        // Tracker for when it's possible to pick a new offering target for sacrifice.
        public int tickToNextMark = 0;

        public float FavorCurrent
        {
            get
            {
                return favorCurrent;
            }
        }

        // This should not remove itself from a pawn automatically.
        public override bool ShouldRemove => false;

        public override void TickInterval(int delta)
        {
            // Acolytes that are in pain gain favor. Those that have not been in pain for too long lose favor.
            if (!pawn.Dead)
            {
                float painTotal = pawn.health.hediffSet.PainTotal;
                int ticksGame = GenTicks.TicksGame;
                if (painTotal > 0.1f)
                {
                    tickLastInPain = ticksGame;
                    painBaseFavorToProvide += 25f / GenDate.TicksPerYear * painTotal * delta;
                }
                else if (ticksGame - tickLastInPain > GenDate.TicksPerQuadrum)
                {
                    painBaseFavorToProvide -= 50f / GenDate.TicksPerYear * delta;
                }
                if (nextPainFavorUpdateTick < ticksGame)
                {
                    if (painBaseFavorToProvide < 0)
                    {
                        NotifyFavorLost(painBaseFavorToProvide);
                    }
                    else if (painBaseFavorToProvide > 0)
                    {
                        NotifyFavorGained(painBaseFavorToProvide);
                    }
                    nextPainFavorUpdateTick = ticksGame + (GenDate.TicksPerHour * 12);
                }
            }
            base.TickInterval(delta);
        }

        // In the event of resurrection, the last tick in pain should be reset as a grace period.
        public override void Notify_Resurrected()
        {
            base.Notify_Resurrected();
            tickLastInPain = GenTicks.TicksGame;
        }

        // This hediff identifies acolytes, and they should be added to the cache of all known acolytes on being made one as well as receiving a pain grace period.
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            tickLastInPain = GenTicks.TicksGame;
        }

        // Killing entities provides favor.
        public override void Notify_KilledPawn(Pawn victim, DamageInfo? dinfo)
        {
            base.Notify_KilledPawn(victim, dinfo);
            if (victim.IsEntity)
            {
                NotifyFavorGained(2.5f * victim.BodySize);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref favorCurrent, "JDG_favorCurrent", 0f);
            Scribe_Values.Look(ref tickLastInPain, "JDG_tickLastInPain", 0);
            Scribe_Values.Look(ref painBaseFavorToProvide, "JDG_painBaseFavorToProvide", 0f);
            Scribe_Values.Look(ref nextPainFavorUpdateTick, "JDG_nextPainFavorUpdateTick", 0);
            Scribe_Values.Look(ref tickToNextMark, "JDG_tickToNextMark", 0);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            Command_Action markOffering = new Command_Action
            {
                defaultLabel = "JDG_MarkOffering".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner"),
                defaultDesc = "JDG_MarkOfferingDesc".Translate(),
                action = delegate
                {
                    Find.Targeter.BeginTargeting(new OfferingTargeting(pawn));
                }
            };
            if (tickToNextMark > GenTicks.TicksGame)
            {
                markOffering.Disabled = true;
                markOffering.disabledReason = "JDG_CannotMarkOfferingOnCooldown".Translate((tickToNextMark - GenTicks.TicksGame).ToStringTicksToPeriod());
            }
            yield return markOffering;

            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action addFavor = new Command_Action
                {
                    defaultLabel = "DEV: Add 10 favor",
                    action = delegate
                    {
                        favorCurrent += 10f;
                    }
                };
                yield return addFavor;

                Command_Action loseFavor = new Command_Action
                {
                    defaultLabel = "DEV: Lose 10 favor",
                    action = delegate
                    {
                        favorCurrent -= 10f;
                    }
                };
                yield return loseFavor;

                Command_Action resetFavor = new Command_Action
                {
                    defaultLabel = "DEV: Reset values",
                    action = delegate
                    {
                        favorCurrent = 0f;
                        tickLastInPain = 0;
                        painBaseFavorToProvide = 0f;
                        nextPainFavorUpdateTick = 0;
                        tickToNextMark = 0;
                    }
                };
                yield return resetFavor;
            }
            yield break;
        }

        // This should be used when accruing favor so that the gain rate modifier is applied.
        public virtual void NotifyFavorGained(float toGain)
        {
            favorCurrent += toGain * pawn.GetStatValue(JDG_StatDefOf.ABF_Stat_Justiciar_FavorGainRate, cacheStaleAfterTicks: GenDate.TicksPerHour);
        }

        // This should be used when losing favor so the loss rate modifier is applied.
        public virtual void NotifyFavorLost(float toLose)
        {
            favorCurrent -= toLose * pawn.GetStatValue(JDG_StatDefOf.ABF_Stat_Justiciar_FavorLossRate, cacheStaleAfterTicks: GenDate.TicksPerHour);
        }
    }
}
