using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class JobDriver_SiphonPsychicEnergy : JobDriver_JusticiarBase
    {
        protected override int WaitTicks => GenDate.TicksPerDay;

        protected override float VeilRadius => 1.9f;

        private int ticksToNextCheck = GenTicks.TicksPerRealSecond;

        protected override void WaitTickIntervalAction()
        {
            if (ticksToNextCheck-- < 0)
            {
                ticksToNextCheck = GenTicks.TicksPerRealSecond;
                if (pawn.needs.rest is Need_Rest restNeed)
                {
                    restNeed.CurLevelPercentage -= GenTicks.TicksPerRealSecond / GenDate.TicksPerHour;
                }
                pawn.psychicEntropy.GainPsyfocus_NewTemp(10 * GenTicks.TicksPerRealSecond, TargetThing);
                TargetThing.TakeDamage(new DamageInfo(DamageDefOf.Blunt, 1f, instigator: pawn));
                if (!TargetThing.Spawned || pawn.needs.rest?.CurLevelPercentage == 0f)
                {
                    pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
                }
            }
        }
    }
}
