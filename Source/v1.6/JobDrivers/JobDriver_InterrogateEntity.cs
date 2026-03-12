using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class JobDriver_InterrogateEntity : JobDriver_JusticiarBase
    {
        public Pawn Interrogee => TargetThing as Pawn;

        protected override void WaitInitAction()
        {
            PawnUtility.ForceWait(Interrogee, WaitTicks, pawn);
        }

        protected override void EndInitAction()
        {
            // Reward handling
            if (Interrogee.TryGetComp<CompStudiable>() is CompStudiable studyComp && Find.ResearchManager.GetProject(studyComp.KnowledgeCategory) != null)
            {
                float researchToAward = studyComp.AnomalyKnowledge * GenDate.DaysPerTwelfth * 2;
                KnowledgeCategoryDef researchCategory = studyComp.KnowledgeCategory;
                if (researchToAward > 0 && researchCategory != null)
                {
                    Find.StudyManager.StudyAnomaly(Interrogee, pawn, researchToAward, researchCategory);
                    Find.LetterStack.ReceiveLetter("JDG_InterrogationResearch".Translate(pawn.LabelShortCap), "JDG_InterrogationResearchDesc".Translate(pawn.LabelShortCap, researchToAward, researchCategory), LetterDefOf.PositiveEvent);
                }
            }
            else
            {
                float favorToAward = 15f * Interrogee.BodySize;
                pawn.health.hediffSet.GetFirstHediff<Hediff_Devotee>()?.NotifyFavorGained(favorToAward);
                Find.LetterStack.ReceiveLetter("JDG_InterrogationFavor".Translate(pawn.LabelShortCap), "JDG_InterrogationFavorDesc".Translate(pawn.LabelShortCap, favorToAward), LetterDefOf.PositiveEvent);

            }

            // Visual and Audio effects
            IntVec3 position = Interrogee.Position;
            Map map = Interrogee.Map;
            JDG_EffecterDefOf.ABF_Effecter_Justiciar_DarkAnnihilation_Mini.Spawn(position, map).Cleanup();
            if (Interrogee.RaceProps.BloodDef == ThingDefOf.Filth_Blood)
            {
                EffecterDefOf.MeatExplosion.Spawn(position, map).Cleanup();
            }
            BlackVeil veil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
            veil.Radius = 0.9f;
            veil.ticksLeft = 1200;
            veil.opacityRange = new FloatRange(0.2f, 0.5f);
            GenSpawn.Spawn(veil, position, map);

            // Cleanup
            Interrogee.Destroy();
            if (pawn.needs?.rest is Need_Rest restNeed)
            {
                restNeed.CurLevelPercentage = 0;
            }
            if (pawn.needs?.mood is Need_Mood moodNeed)
            {
                moodNeed.CurLevel -= 0.25f;
            }
        }
    }
}
