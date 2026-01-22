using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;

namespace ArtificialBeings
{
    // Justiciars have specific behaviors that need to happen only for them. They should ALWAYS have a Hediff with this class.
    public class Hediff_Justiciar : HediffWithComps
    {
        // Favor is a measurement of how pious the justiciar is. Favor is a resource that can be acquired and spent, but total gained over a lifespan also unlocks certain features.
        private float favorCurrent = 0f;

        // Favor accrued over the lifespan of the justiciar should never be manually manipulated by external code. It should never decrement.
        private float favorLifespan = 0f;

        // Tracker for when the Justiciar was last in pain. Used for various calculations.
        public int tickLastInPain = 0;

        // This should not remove itself from a pawn under any circumstances.
        public override bool ShouldRemove => false;

        public IJusticiarMaintainable maintainee;

        public float FavorCurrent
        {
            get
            {
                return favorCurrent;
            }
        }

        public float FavorTotalAccrued
        {
            get
            {
                return favorLifespan;
            }
        }

        // Justiciars stop maintaining things when they are downed.
        public override void Notify_Downed()
        {
            base.Notify_Downed();
            maintainee?.Terminate();
        }

        // Justiciars will spawn a single tile of darkness where they died.
        public override void Notify_PawnKilled()
        {
            base.Notify_PawnKilled();
            BlackVeil veil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
            veil.Radius = 0.9f;
            veil.ticksLeft = 600;
            GenSpawn.Spawn(veil, pawn.PositionHeld, pawn.MapHeld);

            // If the justiciar was maintaining something, it is instantly destroyed on death.
            maintainee?.Terminate();
        }

        // In the event of resurrection, the last tick in pain should be reset as a grace period.
        public override void Notify_Resurrected()
        {
            base.Notify_Resurrected();
            tickLastInPain = GenTicks.TicksGame;
        }

        public override void TickInterval(int delta)
        {
            if (pawn.Spawned)
            {
                Severity = pawn.Map.glowGrid.GroundGlowAt(pawn.Position);
            }
            // Justiciars that are in pain gain favor. Those that have not been in pain for too long lose favor.
            if (!pawn.Dead)
            {
                float painTotal = pawn.health.hediffSet.PainTotal;
                if (painTotal > 0.1f)
                {
                    tickLastInPain = GenTicks.TicksGame;
                    NotifyFavorGained(25f / GenDate.TicksPerYear * painTotal * delta);
                }
                else if (GenTicks.TicksGame - tickLastInPain > GenDate.TicksPerQuadrum)
                {
                    NotifyFavorLost(50f / GenDate.TicksPerYear * delta);
                }
            }
            base.TickInterval(delta);
        }

        // This hediff identifies justiciars, and they should be added to the cache of all known justiciars on being made one as well as receiving a pain grace period.
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            tickLastInPain = GenTicks.TicksGame;
            JDG_Utils.Justiciars.Add(pawn);

            // If the player has not yet learned about justiciars, they will also receive a learning helper tip about how favor and justiciars work.
            LessonAutoActivator.TeachOpportunity(JDG_ConceptDefOf.ABF_Concept_Justiciar_Characteristics, OpportunityType.Critical);
        }

        // Killing a fellow faction member, a guest, or an entity provides favor.
        public override void Notify_KilledPawn(Pawn victim, DamageInfo? dinfo)
        {
            base.Notify_KilledPawn(victim, dinfo);
            if (pawn.Faction != null)
            {
                if (victim.IsFreeColonist)
                {
                    NotifyFavorGained(50f);
                }
                else if (victim.HostFaction == pawn.Faction && !victim.IsPrisoner)
                {
                    NotifyFavorGained(25f);
                }
            }
            if (victim.IsEntity)
            {
                NotifyFavorGained(2.5f * victim.BodySize);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref favorCurrent, "JDG_favorCurrent", 0f);
            Scribe_Values.Look(ref favorLifespan, "JDG_favorLifespan", 0f);
            Scribe_Values.Look(ref tickLastInPain, "JDG_tickLastInPain", 0);
            Scribe_References.Look(ref maintainee, "JDG_maintainee");
            // This hediff identifies justiciars, and they should be re-added to the cache of all known justiciars on loading saves.
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                JDG_Utils.Justiciars.Add(pawn);

                // If the player has not yet learned about justiciars, they will also receive a learning helper tip about how favor and justiciars work.
                LessonAutoActivator.TeachOpportunity(JDG_ConceptDefOf.ABF_Concept_Justiciar_Characteristics, OpportunityType.Critical);

                if (maintainee != null)
                {
                    maintainee.Maintainer = this;
                }
            }
        }

        public override string GetTooltip(Pawn pawn, bool showHediffsDebugInfo)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(base.GetTooltip(pawn, showHediffsDebugInfo));
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine("JDG_JusticiarFavor".Translate(favorCurrent.ToString("F0"), favorLifespan.ToString("F0")));
            return stringBuilder.ToString();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            if (FavorTotalAccrued >= 100f)
            {
                if (ModLister.CheckRoyaltyOrAnomaly("Invisibility hediff") && !pawn.health.hediffSet.HasHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_UmbralStride))
                {
                    Command_Action umbralStride = new Command_Action
                    {
                        defaultLabel = "JDG_BeginUmbralStride".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Abilities/Darkness"),
                        defaultDesc = "JDG_BeginUmbralStrideDesc".Translate(),
                        action = delegate
                        {
                            pawn.health.AddHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_UmbralStride);
                            NotifyFavorLost(10f);
                        }
                    };
                    if (!pawn.Spawned || !JDG_Utils.IsDark(pawn.Position, pawn.Map))
                    {
                        umbralStride.Disabled = true;
                        umbralStride.disabledReason = "JDG_CannotUmbralStrideNotInDarkness".Translate();
                    }
                    yield return umbralStride;
                }
            }
            if (FavorTotalAccrued >= 100f)
            {
                if (!pawn.Drafted)
                {
                    Command_Action createClone = new Command_Action
                    {
                        defaultLabel = "JDG_CreateUmbralClone".Translate(),
                        icon = ContentFinder<Texture2D>.Get("UI/Abilities/ShadeStep"),
                        defaultDesc = "JDG_CreateUmbralCloneDesc".Translate(),
                        action = delegate
                        {
                            Find.Targeter.BeginTargeting(GetComp<HediffComp_CloneTargeting>());
                        }
                    };
                    yield return createClone;
                }
            }

            if (maintainee != null)
            {
                Command_Action dropMaintainee = new Command_Action
                {
                    defaultLabel = "JDG_StopConcentration".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Icons/HostilityResponse/Ignore"),
                    defaultDesc = "JDG_StopConcentrationDesc".Translate(),
                    action = delegate
                    {
                        maintainee?.Terminate();
                        maintainee = null;
                    }
                };
                yield return dropMaintainee;
            }

            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action addFavor = new Command_Action
                {
                    defaultLabel = "DEV: Add 10 favor",
                    action = delegate
                    {
                        NotifyFavorGained(10f);
                    }
                };
                yield return addFavor;

                Command_Action loseFavor = new Command_Action
                {
                    defaultLabel = "DEV: Lose 10 favor",
                    action = delegate
                    {
                        NotifyFavorLost(10f);
                    }
                };
                yield return loseFavor;

                Command_Action resetFavor = new Command_Action
                {
                    defaultLabel = "DEV: Reset favor",
                    action = delegate
                    {
                        favorCurrent = 0f;
                        favorLifespan = 0f;
                    }
                };
                yield return resetFavor;


            }
            yield break;
        }

        // This should be used when accruing favor, so that both the current and lifespan values are updated and thresholds are checked.
        public void NotifyFavorGained(float toGain)
        {
            favorLifespan += toGain;
            favorCurrent += toGain;

            // If the player has not yet learned about new mechanics from each threshold, they will also receive a learning helper tip about how they work.
            if (favorLifespan >= 100f)
            {
                LessonAutoActivator.TeachOpportunity(JDG_ConceptDefOf.ABF_Concept_Justiciar_FirstTierUnlocks, OpportunityType.Critical);
            }
        }

        // This should be used when losing favor. It does *not* update the lifespan value.
        public void NotifyFavorLost(float toLose)
        {
            favorCurrent -= toLose;
        }

        public void NotifyNewMaintainee(IJusticiarMaintainable newMaintainee)
        {
            maintainee?.Terminate();
            maintainee = newMaintainee;
            maintainee.Maintainer = this;
        }
    }
}
