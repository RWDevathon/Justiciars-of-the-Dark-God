using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class JobDriver_AdmireSkull : JobDriver_VisitJoyThing
    {
        private Thing Skull => job.GetTarget(TargetIndex.A).Thing;

        protected override void WaitTickAction(int delta)
        {
            float extraJoyGainFactor = 1f + (0.02f * Skull.stackCount);
            pawn.GainComfortFromCellIfPossible(delta);
            JoyUtility.JoyTickCheckEnd(pawn, delta, JoyTickFullJoyAction.EndJob, extraJoyGainFactor);
        }
    }
}
