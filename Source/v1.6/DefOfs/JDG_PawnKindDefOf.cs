using RimWorld;
using Verse;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_PawnKindDefOf
    {
        static JDG_PawnKindDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_PawnKindDefOf));
        }

        public static PawnKindDef ABF_PawnKind_Justiciar_Player_DarkLurker;

        public static PawnKindDef ABF_PawnKind_Justiciar_Player_SoulHunter;

        [MayRequireIdeology]
        public static PawnKindDef ABF_PawnKind_Justiciar_Player_CreaturePerceptor;

        [MayRequireIdeology]
        public static PawnKindDef ABF_PawnKind_Justiciar_Player_CreatureWraith;

        [MayRequireIdeology]
        public static PawnKindDef ABF_PawnKind_Justiciar_Player_CreatureHerald;
    }
}
