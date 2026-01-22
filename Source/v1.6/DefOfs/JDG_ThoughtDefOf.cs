using RimWorld;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_ThoughtDefOf
    {
        static JDG_ThoughtDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_ThoughtDefOf));
        }

        public static ThoughtDef ABF_Thought_Justiciar_Displeased;
    }
}
