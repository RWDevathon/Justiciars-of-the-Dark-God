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

        [MayRequireOdyssey]
        public static JobDef ABF_Job_Justiciar_AwakenMaliciousSpirit;

        public static JobDef ABF_Job_Justiciar_Clone;

        public static JobDef ABF_Job_Justiciar_CorruptThing;

        [MayRequireIdeology]
        public static JobDef ABF_Job_Justiciar_DoManualScrying;

        public static JobDef ABF_Job_Justiciar_DominateAnimal;

        [MayRequireIdeology]
        public static JobDef ABF_Job_Justiciar_HarvestDead;

        public static JobDef ABF_Job_Justiciar_Induct;

        [MayRequireAnomaly]
        public static JobDef ABF_Job_Justiciar_InterrogateEntity;

        [MayRequireRoyalty]
        public static JobDef ABF_Job_Justiciar_PillagePsychicEnergy;

        public static JobDef ABF_Job_Justiciar_Raise;

        [MayRequireBiotech]
        public static JobDef ABF_Job_Justiciar_ReapFavor;

        public static JobDef ABF_Job_Justiciar_ShatterShard;

        [MayRequireRoyalty]
        public static JobDef ABF_Job_Justiciar_SiphonPsychicEnergy;

        public static JobDef ABF_Job_Justiciar_StealFavor;

        public static JobDef ABF_Job_Justiciar_SummonAcolyte;

        [MayRequireIdeology]
        public static JobDef ABF_Job_Justiciar_SummonCreature;
    }
}
