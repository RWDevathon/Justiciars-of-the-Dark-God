using RimWorld;
using Verse;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_BackstoryDefOf
    {
        static JDG_BackstoryDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_BackstoryDefOf));
        }

        public static BackstoryDef ABF_Backstory_Justiciar_Childhood_Risen;

        public static BackstoryDef ABF_Backstory_Justiciar_Adulthood_Risen;
    }
}
