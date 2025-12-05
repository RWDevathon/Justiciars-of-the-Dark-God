using RimWorld;
using Verse;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_ThingDefOf
    {
        static JDG_ThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_ThingDefOf));
        }

        public static ThingDef ABF_Thing_BlackVeil;
    }
}
