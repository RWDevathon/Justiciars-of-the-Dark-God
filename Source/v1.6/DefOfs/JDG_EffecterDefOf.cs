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

        public static EffecterDef ABF_Effecter_Justiciar_DarkAnnihilation;

        public static EffecterDef ABF_Effecter_Justiciar_DarkAnnihilation_Mini;

        [MayRequireOdyssey]
        public static EffecterDef ABF_Effecter_Justiciar_MaliciousSpirit;
    }
}
