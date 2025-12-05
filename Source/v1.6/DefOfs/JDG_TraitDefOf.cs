using RimWorld;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_TraitDefOf
    {
        static JDG_TraitDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_TraitDefOf));
        }

        public static TraitDef ABF_Trait_Justiciar_Adherent;
    }
}
