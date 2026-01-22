using RimWorld;
using Verse;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_LetterDefOf
    {
        static JDG_LetterDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_LetterDefOf));
        }

        public static LetterDef ABF_Letter_Justiciar_ChoosePath;

        public static LetterDef ABF_Letter_Justiciar_AcolyteWhispers;
    }
}
