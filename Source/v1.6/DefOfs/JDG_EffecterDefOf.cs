using RimWorld;
using Verse;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_EffecterDefOf
    {
        static JDG_EffecterDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_EffecterDefOf));
        }

        [MayRequireOdyssey]
        public static EffecterDef ABF_Effecter_Justiciar_MaliciousSpirit;
    }
}
