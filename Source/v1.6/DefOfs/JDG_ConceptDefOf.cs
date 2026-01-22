using RimWorld;
using Verse;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_ConceptDefOf
    {
        static JDG_ConceptDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_ConceptDefOf));
        }

        public static ConceptDef ABF_Concept_Justiciar_Attainment;

        public static ConceptDef ABF_Concept_Justiciar_Characteristics;

        public static ConceptDef ABF_Concept_Justiciar_Ambitions;

        public static ConceptDef ABF_Concept_Justiciar_Inspirations;

        public static ConceptDef ABF_Concept_Justiciar_FirstTierUnlocks;
    }
}
