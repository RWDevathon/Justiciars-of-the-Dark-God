using RimWorld;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_StatDefOf
    {
        static JDG_StatDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_StatDefOf));
        }

        public static StatDef ABF_Stat_Justiciar_FavorGainRate;

        public static StatDef ABF_Stat_Justiciar_FavorLossRate;
    }
}
