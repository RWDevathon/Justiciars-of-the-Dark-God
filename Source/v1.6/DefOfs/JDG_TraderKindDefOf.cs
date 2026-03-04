using RimWorld;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_TraderKindDefOf
    {
        static JDG_TraderKindDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_TraderKindDefOf));
        }

        public static TraderKindDef ABF_TraderKind_Justiciar_Menagerie;
    }
}
