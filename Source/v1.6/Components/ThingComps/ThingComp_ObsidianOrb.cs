using RimWorld;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    // A simple comp that allows for being contemplated and inducing a debilitating effect on the user.
    public class ThingComp_ObsidianOrb : ThingComp
    {
        private ThingCompProperties_ObsidianOrb Props => (ThingCompProperties_ObsidianOrb)props;

        public override void CompTickInterval(int delta)
        {
            if ((delta > 10 || parent.IsHashIntervalTick(10)) && (parent.Spawned || parent.ParentHolder is Pawn_CarryTracker))
            {
                Map map = parent.MapHeld;
                if (map != null)
                {
                    Vector3 location = parent.DrawPos;
                    if (location.ShouldSpawnMotesAt(map))
                    {
                        FleckCreationData dataStatic = FleckMaker.GetDataStatic(location, map, JDG_FleckDefOf.ABF_Fleck_Justiciar_Smoke, Rand.Range(0.5f, 1.6f) * parent.Graphic.drawSize.magnitude);
                        dataStatic.rotationRate = Rand.Range(-90f, 90f);
                        dataStatic.velocityAngle = Rand.Range(0, 360);
                        dataStatic.velocitySpeed = Rand.Range(0.05f, 0.2f);
                        map.flecks.CreateFleck(dataStatic);
                    }
                }
            }
            base.CompTickInterval(delta);
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (!CanBeContemplatedBy(selPawn))
            {
                yield return new FloatMenuOption("JDG_CannotContemplateNow".Translate(), null);
            }
            else
            {
                if (DebugSettings.ShowDevGizmos)
                {
                    yield return new FloatMenuOption("Debug: Contemplate Immediately", delegate
                    {
                        Notify_Used(selPawn);
                    });
                }

                yield return new FloatMenuOption("JDG_ContemplateOrb".Translate(), delegate
                {
                    Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_ContemplateOrb, parent);
                    selPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                });
            }
            yield break;
        }

        public bool CanBeContemplatedBy(Pawn pawn)
        {
            if (!pawn.IsColonistPlayerControlled && !pawn.IsSlaveOfColony && !pawn.IsPrisonerOfColony)
            {
                return false;
            }

            if (pawn.health.hediffSet.HasHediff(Props.postContemplationHediff))
            {
                return false;
            }

            return true;
        }

        // When used, apply all applicable effects to the user.
        public void Notify_Used(Pawn pawn)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("JDG_BaseForgotten".Translate(pawn.NameShortColored));

            if (Props.forgetAllRelationships)
            {
                pawn.relations.ClearAllRelations();
                stringBuilder.AppendLine("JDG_AllRelationsForgotten".Translate());
            }
            if (Props.forgetNonBiologicalRelationships)
            {
                pawn.relations.ClearAllNonBloodRelations();
                stringBuilder.AppendLine("JDG_NonBiologicalRelationsForgotten".Translate());
            }

            if (Props.forgetMemories && pawn.needs?.mood?.thoughts?.memories is MemoryThoughtHandler memoryHandler)
            {
                List<Thought_Memory> memories = memoryHandler.Memories;
                for (int i = memories.Count - 1; i >= 0; i--)
                {
                    if (memories[i].def != JDG_ThoughtDefOf.ABF_Thought_Justiciar_Displeased)
                    {
                        memoryHandler.RemoveMemory(memories[i]);
                    }
                }
                stringBuilder.AppendLine("JDG_MemoriesForgotten".Translate());
            }

            if (pawn.skills is Pawn_SkillTracker skillTracker)
            {
                if (Props.skillsLostPercentageRange.max > 0)
                {
                    foreach (SkillRecord skillRecord in skillTracker.skills)
                    {
                        skillRecord.Learn(skillRecord.XpTotalEarned * -Mathf.Clamp01(Props.skillsLostPercentageRange.RandomInRange), direct: true);
                    }
                    stringBuilder.AppendLine("JDG_SkillsForgotten".Translate(Props.skillsLostPercentageRange.min.ToStringPercent(), Props.skillsLostPercentageRange.max.ToStringPercent()));
                }

                if (Rand.Chance(Props.chancePassionsReallocated))
                {
                    List<SkillRecord> skills = skillTracker.skills;
                    Dictionary<SkillRecord, int> newPassions = new Dictionary<SkillRecord, int>();
                    int passionLevels = 0;
                    foreach (SkillRecord skillRecord in skills)
                    {
                        passionLevels += (int)skillRecord.passion;
                        newPassions[skillRecord] = 0;
                        skillRecord.passion = Passion.None;
                    }

                    while (passionLevels-- > 0)
                    {
                        if (!newPassions.TryRandomElement(passionRecord => passionRecord.Value < 2, out KeyValuePair<SkillRecord, int> recordPassionPair))
                        {
                            break;
                        }
                        newPassions[recordPassionPair.Key]++;
                    }

                    foreach (SkillRecord skillRecord in skills)
                    {
                        skillRecord.passion = (Passion)newPassions[skillRecord];
                    }
                    stringBuilder.AppendLine("JDG_PassionsForgotten".Translate());
                }

                if (Props.forgetCurableAddictions)
                {
                    bool addictionsForgotten = false;
                    List<Hediff> curableAddictions = new List<Hediff>();
                    pawn.health.hediffSet.GetHediffs(ref curableAddictions, hediff => hediff.def.IsAddiction && hediff.def.everCurableByItem);

                    for (int i = curableAddictions.Count - 1; i >= 0; i--)
                    {
                        addictionsForgotten = true;
                        pawn.health.RemoveHediff(curableAddictions[i]);
                    }
                    if (addictionsForgotten)
                    {
                        stringBuilder.AppendLine("JDG_AddictionsForgotten".Translate());
                    }
                }

                if (Props.forgetObsessions)
                {
                    // The Golden Cube
                    if (ModsConfig.AnomalyActive)
                    {
                        bool cubeForgotten = false;
                        Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.CubeInterest);
                        if (hediff != null)
                        {
                            cubeForgotten = true;
                            pawn.health.RemoveHediff(hediff);
                        }

                        hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.CubeWithdrawal);
                        if (hediff != null)
                        {
                            cubeForgotten = true;
                            pawn.health.RemoveHediff(hediff);
                        }

                        hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.CubeRage);
                        if (hediff != null)
                        {
                            cubeForgotten = true;
                            pawn.health.RemoveHediff(hediff);
                        }

                        hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.CubeComa);
                        if (hediff != null)
                        {
                            cubeForgotten = true;
                            pawn.health.RemoveHediff(hediff);
                        }

                        if (cubeForgotten)
                        {
                            stringBuilder.AppendLine("JDG_CubeForgotten".Translate());
                        }
                    }
                }

                if (Props.forgetIdeoCertainty && ModsConfig.IdeologyActive && pawn.Ideo is Ideo ideo && ideo != Faction.OfPlayer.ideos.PrimaryIdeo)
                {
                    // This method ignores certainty change factor, so it should go to 0.
                    pawn.ideo.Debug_ReduceCertainty(pawn.ideo.Certainty);
                    stringBuilder.AppendLine("JDG_IdeologyForgotten".Translate());
                }

                if (Props.forgetInspirations)
                {
                    if (pawn.Inspired)
                    {
                        // Some Justiciar inspirations need additional cleanup
                        if (pawn.Inspiration is Inspiration_Justiciar_Kinslayer)
                        {
                            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(JDG_HediffDefOf.ABF_Hediff_Justiciar_Inspiration_Kinslayer);

                            if (hediff != null)
                            {
                                pawn.health.RemoveHediff(hediff);
                            }
                        }
                        else if (pawn.Inspiration is Inspiration_Justiciar_Bondbreaker)
                        {
                            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(JDG_HediffDefOf.ABF_Hediff_Justiciar_Inspiration_Bondbreaker);

                            if (hediff != null)
                            {
                                pawn.health.RemoveHediff(hediff);
                            }
                        }
                        else if (pawn.Inspiration is Inspiration_Justiciar_VowOfSilence)
                        {
                            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(JDG_HediffDefOf.ABF_Hediff_Justiciar_Inspiration_VowOfSilence);

                            if (hediff != null)
                            {
                                pawn.health.RemoveHediff(hediff);
                            }
                        }

                        // Explicitly do not end the inspiration normally. That would trigger Justiciar inspirations to terminate incorrectly.
                        pawn.mindState.inspirationHandler.Reset();
                        stringBuilder.AppendLine("JDG_InspirationForgotten".Translate());
                    }

                    bool ambitionForgotten = false;
                    List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
                    for (int i = hediffs.Count - 1; i >= 0; i--)
                    {
                        if (hediffs[i] is Hediff_Ambition ambition && !ambition.complete)
                        {
                            ambitionForgotten = true;
                            pawn.health.RemoveHediff(hediffs[i]);
                        }
                    }
                    if (ambitionForgotten)
                    {
                        stringBuilder.AppendLine("JDG_AmbitionForgotten".Translate());
                    }
                }

                if (Props.forgetForeignAllegiances && (pawn.IsPrisonerOfColony || pawn.IsSlaveOfColony))
                {
                    pawn.guest.resistance = 0;
                    pawn.guest.will = 0;
                    pawn.guest.Recruitable = true;
                    pawn.SetFaction(Faction.OfPlayer);
                    stringBuilder.AppendLine("JDG_AllegianceForgotten".Translate());
                }

                stringBuilder.Append("\n");
                if (Props.favorOnUse > 0 && JDG_Utils.IsDevotee(pawn))
                {
                    JDG_Utils.GetDevoteeHediff(pawn)?.NotifyFavorGained(Props.favorOnUse);
                    stringBuilder.AppendLine("JDG_FavorGained".Translate(Props.favorOnUse.ToString("F2")));
                }

                if (Props.postContemplationHediff != null)
                {
                    pawn.health.AddHediff(Props.postContemplationHediff);
                    stringBuilder.Append("JDG_ContemplationHediffGained".Translate(pawn.NameShortColored, Props.postContemplationHediff.label));
                }

                Find.LetterStack.ReceiveLetter("JDG_OrbContemplated".Translate(pawn.NameShortColored), stringBuilder.ToString(), LetterDefOf.NeutralEvent);
            }
        }
    }
}
