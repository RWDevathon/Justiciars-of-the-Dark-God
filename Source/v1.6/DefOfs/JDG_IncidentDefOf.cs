using RimWorld;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_IncidentDefOf
    {
        static JDG_IncidentDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_IncidentDefOf));
        }

        public static IncidentDef CropBlight;
    }
}
