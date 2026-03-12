using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    // Justiciars have specific behaviors that need to happen only for them. They should ALWAYS have a Hediff with this class.
    public class Hediff_Justiciar : Hediff_Devotee
    {
        // Favor accrued over the lifespan of the justiciar should never be manually manipulated by external code. It should never decrement.
        private float favorLifespan = 0f;

        // Exposure to bioferrite causes illness. High ranking justiciars can even spontaneously ignite while wearing it.
        public int nextBioferriteCheck = 0;

        public IJusticiarMaintainable maintainee;

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

        public override void TickInterval(int delta)
        {
            // The bonus/malus from being in darkness/light needs to be checked consistently.
            if (pawn.Spawned)
            {
                Severity = pawn.Map.glowGrid.GroundGlowAt(pawn.Position);
                // Justiciars are weak to bioferrite. Make sure it applies correctly.
                if (GenTicks.TicksGame >= nextBioferriteCheck)
                {
                    if (!ModsConfig.AnomalyActive)
                    {
                        nextBioferriteCheck = GenTicks.TicksGame + GenDate.TicksPerQuadrum;
                    }

                    nextBioferriteCheck = GenTicks.TicksGame;
                    bool foundBioferrite = false;

                    if (pawn.apparel?.WornApparel is List<Apparel> apparelList)
                    {
                        foreach (Apparel apparel in apparelList)
                        {
                            if (apparel.Stuff == ThingDefOf.Bioferrite)
                            {
                                foundBioferrite = true;
                                if (FavorTotalAccrued > 400f && Rand.Bool)
                                {
                                    // Fifty fifty chance for spontaneous ignition or for cancer.
                                    if (Rand.Bool)
                                    {

                                    }
                                    pawn.TryAttachFire(Rand.Range(0.8f, 1.4f), apparel);
                                    GenExplosion.DoExplosion(pawn.Position, pawn.Map, 2.9f, DamageDefOf.Flame, pawn, 5);
                                    if (PawnUtility.ShouldSendNotificationAbout(pawn))
                                    {
                                        Messages.Message("JDG_BioferriteIgnition".Translate(pawn.NameShortColored), pawn, MessageTypeDefOf.NegativeEvent, false);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    if (pawn.equipment != null)
                    {
                        foreach (ThingWithComps equipment in pawn.equipment.AllEquipmentListForReading)
                        {
                            if (equipment.Stuff == ThingDefOf.Bioferrite)
                            {
                                foundBioferrite = true;
                                if (FavorTotalAccrued > 400f && Rand.Bool)
                                {
                                    pawn.mindState.mentalStateHandler.TryStartMentalState(JDG_MentalStateDefOf.InsaneRamblings, "JDG_BreakBecauseBioferriteEquipment".Translate(), true, true);
                                }
                            }
                        }
                    }
                    foreach (Thing item in pawn.Map.listerThings.ThingsOfDef(ThingDefOf.BioferriteGenerator))
                    {
                        CompNoiseSource compNoiseSource = item.TryGetComp<CompNoiseSource>();
                        if (compNoiseSource.Active && pawn.Position.InHorDistOf(item.Position, compNoiseSource.Props.radius))
                        {
                            foundBioferrite = true;
                            if (FavorTotalAccrued > 400f && Rand.Bool)
                            {
                                pawn.mindState.mentalStateHandler.TryStartMentalState(JDG_MentalStateDefOf.InsaneRamblings, "JDG_BreakBecauseBioferriteEquipment".Translate(), true, true);
                            }
                        }
                    }

                    if (foundBioferrite)
                    {
                        if (pawn.health.hediffSet.GetFirstHediffOfDef(JDG_HediffDefOf.ABF_Hediff_Justiciar_BioferriteWeakness) == null)
                        {
                            pawn.health.AddHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_BioferriteWeakness);
                            if (PawnUtility.ShouldSendNotificationAbout(pawn))
                            {
                                Messages.Message("JDG_BioferriteWeakness".Translate(pawn.NameShortColored), pawn, MessageTypeDefOf.NegativeEvent, false);
                            }
                        }
                        nextBioferriteCheck = GenTicks.TicksGame + Rand.Range(GenDate.TicksPerHour * 6, GenDate.TicksPerDay);
                    }
                    else
                    {
                        if (pawn.health.hediffSet.GetFirstHediffOfDef(JDG_HediffDefOf.ABF_Hediff_Justiciar_BioferriteWeakness) is Hediff hediff)
                        {
                            pawn.health.RemoveHediff(hediff);
                        }
                        nextBioferriteCheck = GenTicks.TicksGame + Rand.Range(GenDate.TicksPerTwelfth, GenDate.TicksPerQuadrum);
                    }
                }
            }
            base.TickInterval(delta);
        }

        // This hediff identifies justiciars, and they should be added to the cache of all known justiciars on being made one as well as receiving a pain grace period.
        public override void PostAdd(DamageInfo? dinfo)
        {
            base.PostAdd(dinfo);
            JDG_Utils.Justiciars.Add(pawn);

            // If the player has not yet learned about justiciars, they will also receive a learning helper tip about how favor and justiciars work.
            LessonAutoActivator.TeachOpportunity(JDG_ConceptDefOf.ABF_Concept_Justiciar_Characteristics, OpportunityType.Critical);
            if (Current.Game.GetComponent<GameComponent_Justiciars>() is GameComponent_Justiciars gameCompJusticiars && !gameCompJusticiars.everSentMenagerieNotification)
            {
                gameCompJusticiars.everSentMenagerieNotification = true;
                Find.LetterStack.ReceiveLetter("JDG_MenagerieInvitation".Translate(), "JDG_MenagerieInvitationDesc".Translate(), LetterDefOf.NeutralEvent, delayTicks: GenDate.TicksPerDay);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref favorLifespan, "JDG_favorLifespan", 0f);
            Scribe_Values.Look(ref nextBioferriteCheck, "JDG_nextBioferriteCheck", 0);
            Scribe_References.Look(ref maintainee, "JDG_maintainee");
            // This hediff identifies justiciars, and they should be re-added to the cache of all known justiciars on loading saves.
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                JDG_Utils.Justiciars.Add(pawn);

                // If the player has not yet learned about justiciars, they will also receive a learning helper tip about how favor and justiciars work.
                LessonAutoActivator.TeachOpportunity(JDG_ConceptDefOf.ABF_Concept_Justiciar_Characteristics, OpportunityType.Critical);
                if (Current.Game.GetComponent<GameComponent_Justiciars>() is GameComponent_Justiciars gameCompJusticiars && !gameCompJusticiars.everSentMenagerieNotification)
                {
                    gameCompJusticiars.everSentMenagerieNotification = true;
                    Find.LetterStack.ReceiveLetter("JDG_MenagerieInvitation".Translate(), "JDG_MenagerieInvitationDesc".Translate(pawn.NameShortColored), LetterDefOf.NeutralEvent, delayTicks: GenDate.TicksPerDay);
                }

                if (maintainee != null)
                {
                    maintainee.Maintainer = pawn;
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
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }

            if (FavorTotalAccrued >= 100f)
            {
                if (ModLister.CheckRoyaltyOrAnomaly("Invisibility hediff") && !pawn.health.hediffSet.HasHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_UmbralStride))
                {
                    Command_Action umbralStride = new Command_Action
                    {
                        defaultLabel = "JDG_BeginUmbralStride".Translate(),
                        icon = ContentFinder<Texture2D>.Get("Justiciars/UI/Abilities/Darkness"),
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
                        icon = ContentFinder<Texture2D>.Get("Justiciars/UI/Abilities/ShadeStep"),
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
                Command_Action addTotalFavor = new Command_Action
                {
                    defaultLabel = "DEV: Add 10 favor to total accrued",
                    action = delegate
                    {
                        favorLifespan += 10f;
                    }
                };
                yield return addTotalFavor;

                Command_Action resetBioferriteTimer = new Command_Action
                {
                    defaultLabel = "DEV: Reset Bioferrite timer",
                    action = delegate
                    {
                        nextBioferriteCheck = 0;
                    }
                };
                yield return resetBioferriteTimer;
            }
            yield break;
        }

        // This should be used when accruing favor, so that both the current and lifespan values are updated and thresholds are checked.
        public override void NotifyFavorGained(float toGain)
        {
            toGain *= pawn.GetStatValue(JDG_StatDefOf.ABF_Stat_Justiciar_FavorGainRate, cacheStaleAfterTicks: GenDate.TicksPerHour);

            favorLifespan += toGain;
            favorCurrent += toGain;

            // If the player has not yet learned about new mechanics from each threshold, they will also receive a learning helper tip about how they work.
            if (favorLifespan >= 100f)
            {
                LessonAutoActivator.TeachOpportunity(JDG_ConceptDefOf.ABF_Concept_Justiciar_FirstTierUnlocks, OpportunityType.Critical);
            }
        }

        public void NotifyNewMaintainee(IJusticiarMaintainable newMaintainee)
        {
            maintainee?.Terminate();
            maintainee = newMaintainee;
            maintainee.Maintainer = pawn;
        }
    }
}
