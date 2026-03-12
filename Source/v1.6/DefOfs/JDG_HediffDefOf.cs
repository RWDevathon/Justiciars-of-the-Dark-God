using RimWorld;
using Verse;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_HediffDefOf
    {
        static JDG_HediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_HediffDefOf));
        }

        public static HediffDef ABF_Hediff_Justiciar_Acolyte;

        [MayRequireAnomaly]
        public static HediffDef ABF_Hediff_Justiciar_BioferriteWeakness;

        [MayRequireOdyssey]
        public static HediffDef ABF_Hediff_Justiciar_ChokingDarkness;

        public static HediffDef ABF_Hediff_Justiciar_CrushingDespair;

        public static HediffDef ABF_Hediff_Justiciar_Inspiration_Kinslayer;

        public static HediffDef ABF_Hediff_Justiciar_Inspiration_Bondbreaker;

        public static HediffDef ABF_Hediff_Justiciar_Inspiration_VowOfSilence;

        public static HediffDef ABF_Hediff_Justiciar_MarkedOffering;

        public static HediffDef ABF_Hediff_Justiciar_ShadeSpirit;

        public static HediffDef ABF_Hediff_Justiciar_Ambition_CleansingFury;

        public static HediffDef ABF_Hediff_Justiciar_UmbralStride;


    }
}
