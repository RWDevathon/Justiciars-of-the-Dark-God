using RimWorld;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_PawnRelationDefOf
    {
        static JDG_PawnRelationDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_PawnRelationDefOf));
        }

        public static PawnRelationDef ABF_PawnRelation_Justiciar_Arbiter;

        public static PawnRelationDef ABF_PawnRelation_Justiciar_Neophyte;
    }
}
