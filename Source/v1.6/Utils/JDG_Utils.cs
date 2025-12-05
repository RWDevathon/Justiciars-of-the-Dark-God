using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public static class JDG_Utils
    {
        // Given a pawn and the desired pawn kind to use for the justiciar, create a new pawn and copy select information from source into the justiciar.
        // Justiciars do not copy everything from the source pawn - some things are (intentionally) forever lost.
        public static Pawn GenerateJusticiarFor(Pawn source, PawnKindDef justiciarKind)
        {
            PawnGenerationRequest generationRequest = new PawnGenerationRequest(justiciarKind, source.Faction, forceGenerateNewPawn: true, colonistRelationChanceFactor: 0f, fixedGender: source.gender, fixedBiologicalAge: source.ageTracker.AgeBiologicalYearsFloat, fixedChronologicalAge: source.ageTracker.AgeChronologicalYearsFloat, fixedIdeo: source.Ideo, forceBaselinerChance: 1f);
            Pawn justiciar = PawnGenerator.GeneratePawn(generationRequest);

            InheritStory(ref source, ref justiciar);
            InheritSkills(source, justiciar);
            InheritIdeology(source, justiciar);
            InheritPlayerSettings(source, justiciar);

            // Give them the Justiciar trait
            Trait adherentTrait = new Trait(JDG_TraitDefOf.ABF_Trait_Justiciar_Adherent);
            justiciar.story.traits.GainTrait(adherentTrait);

            // Inherit the name for the Justiciar.
            NameTriple sourceName = (NameTriple)source.Name;
            justiciar.Name = new NameTriple(sourceName.First, sourceName.Nick, sourceName.Last);

            // Pawns may sometimes spawn with apparel somewhere in the generation process. Justiciars should NEVER spawn with apparel pre-configured.
            justiciar.apparel?.DestroyAll();

            // Force a re-render just in case any copied information alters graphics.
            justiciar.Drawer.renderer.SetAllGraphicsDirty();
            return justiciar;
        }

        // Inherit select details from the StoryTracker for the justiciar.
        public static void InheritStory(ref Pawn source, ref Pawn justiciar)
        {
            justiciar.story.favoriteColor = source.story.favoriteColor;
            justiciar.story.traits.allTraits.Clear();
            foreach (Trait sourceTrait in source.story.traits.allTraits)
            {
                if (!ModsConfig.BiotechActive || sourceTrait.sourceGene == null)
                {
                    justiciar.story.traits.GainTrait(new Trait(sourceTrait.def, sourceTrait.Degree, sourceTrait.ScenForced));
                }
            }
            justiciar.Notify_DisabledWorkTypesChanged();
            justiciar.skills.Notify_SkillDisablesChanged();
        }

        // Inherit all skill levels, xp gains, and passions for the justiciar.
        public static void InheritSkills(Pawn source, Pawn justiciar)
        {
            justiciar.skills.skills.Clear();
            foreach (SkillRecord skill in source.skills.skills)
            {
                SkillRecord item = new SkillRecord(justiciar, skill.def)
                {
                    levelInt = skill.levelInt,
                    passion = skill.passion,
                    xpSinceLastLevel = skill.xpSinceLastLevel,
                    xpSinceMidnight = skill.xpSinceMidnight
                };
                justiciar.skills.skills.Add(item);
            }
        }

        // Inherit ideology details for the justiciar.
        public static void InheritIdeology(Pawn source, Pawn justiciar)
        {
            if (ModsConfig.IdeologyActive)
            {
                // If source ideology is null, then the justiciar's ideology should also be null. Vanilla handles null ideoligions relatively gracefully.
                if (source.ideo == null)
                {
                    justiciar.ideo = null;
                }
                else
                {
                    justiciar.ideo = new Pawn_IdeoTracker(justiciar);
                    justiciar.ideo.SetIdeo(source.Ideo);
                    justiciar.ideo.OffsetCertainty(source.ideo.Certainty - justiciar.ideo.Certainty);
                    justiciar.ideo.joinTick = source.ideo.joinTick;
                }
            }
        }

        // Inherit all player settings, such as work assignments, area restrictions, and time tables for the justiciar.
        // Some player settings, such as medical care settings or outfit policies, are irrelevant and ignored.
        public static void InheritPlayerSettings(Pawn source, Pawn justiciar)
        {
            // This only needs to run if this justiciar belongs to the player.
            if (justiciar.Faction != null && justiciar.Faction.IsPlayer)
            {
                // Initialize destination work settings if not initialized.
                if (justiciar.workSettings == null)
                {
                    justiciar.workSettings = new Pawn_WorkSettings(justiciar);
                }
                justiciar.workSettings.EnableAndInitializeIfNotAlreadyInitialized();

                // Apply work settings to the destination from the source.
                if (source.workSettings != null && source.workSettings.EverWork)
                {
                    foreach (WorkTypeDef workTypeDef in DefDatabase<WorkTypeDef>.AllDefsListForReading)
                    {
                        if (!justiciar.WorkTypeIsDisabled(workTypeDef))
                            justiciar.workSettings.SetPriority(workTypeDef, source.workSettings.GetPriority(workTypeDef));
                    }
                }

                if (source.timetable != null)
                {
                    for (int i = 0; i != 24; i++)
                    {
                        justiciar.timetable.SetAssignment(i, source.timetable.GetAssignment(i));
                    }
                }

                if (source.playerSettings != null)
                {
                    justiciar.playerSettings.joinTick = source.playerSettings.joinTick;
                    justiciar.playerSettings.AreaRestrictionInPawnCurrentMap = source.playerSettings.AreaRestrictionInPawnCurrentMap;
                    justiciar.playerSettings.hostilityResponse = source.playerSettings.hostilityResponse;
                }
            }
        }
    }
}
