using RimWorld;
using Verse;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_SoundDefOf
    {
        static JDG_SoundDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_SoundDefOf));
        }

        [MayRequireAnomaly]
        public static SoundDef MeatExplosionLarge;
    }
}
