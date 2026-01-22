using RimWorld;
using Verse;

namespace ArtificialBeings
{
    [DefOf]
    public class JDG_JobDefOf
    {
        static JDG_JobDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(JDG_JobDefOf));
        }

        public static JobDef ABF_Job_Justiciar_Clone;

        public static JobDef ABF_Job_Justiciar_Raise;

        public static JobDef ABF_Job_Justiciar_SummonAcolyte;

        public static JobDef ABF_Job_Justiciar_Induct;

        public static JobDef ABF_Job_Justiciar_StealFavor;
    }
}
