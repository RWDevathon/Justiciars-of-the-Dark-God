using RimWorld;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_PsychicRitualDefOf
    {
        static JDG_PsychicRitualDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_TraitDefOf));
        }

        public static PsychicRitualDef_VoidProvocation VoidProvocation;
    }
}
