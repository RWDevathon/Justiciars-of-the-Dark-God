using Verse;

namespace ArtificialBeings
{
    public class ThingCompProperties_ObsidianOrb : CompProperties
    {
        // If true, these two fields will eliminate relationships this person has of that type with others.
        public bool forgetAllRelationships = true;
        public bool forgetNonBiologicalRelationships = true;

        // If true, eliminate all memories, both good and bad. The Dark God's displeasure, if it is present, is not removed.
        public bool forgetMemories = true;

        // Sets the range of skill points that can be lost in each skill.
        public FloatRange skillsLostPercentageRange = new FloatRange(0.25f, 0.75f);

        // Chance of passion point reallocation.
        public float chancePassionsReallocated = 0.3f;

        // If true, these two fields will eliminate addictions and obsessions that are curable.
        public bool forgetCurableAddictions = true;
        public bool forgetObsessions = true;

        // If true, Ideological certainty of the pawn will be set to 0 so next attempt will guarantee conversion - only applied if the pawn does not have the player's main ideology.
        public bool forgetIdeoCertainty = true;

        // If true, eliminate all inspirations and Justiciar ambitions.
        public bool forgetInspirations = true;

        // If true, remove prisoner resistance, will, and faction details. Only applicable to prisoners and slaves.
        public bool forgetForeignAllegiances = true;

        // Sets the amount of favor that a devotee will receive for using this item.
        public float favorOnUse = 25f;

        // Configures the HediffDef that will be applied to the pawn when used, and which prevents using this item again until removed.
        public HediffDef postContemplationHediff;

        public ThingCompProperties_ObsidianOrb()
        {
            compClass = typeof(ThingComp_ObsidianOrb);
        }
    }
}
