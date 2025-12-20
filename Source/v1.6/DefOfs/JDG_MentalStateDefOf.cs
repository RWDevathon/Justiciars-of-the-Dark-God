using RimWorld;
using Verse;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_MentalStateDefOf
    {
        static JDG_MentalStateDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_MentalStateDefOf));
        }

        public static MentalStateDef Tantrum;

        public static MentalStateDef MurderousRage;

        public static MentalStateDef Binging_DrugExtreme;
    }
}
