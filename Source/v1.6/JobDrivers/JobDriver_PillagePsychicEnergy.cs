using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class JobDriver_PillagePsychicEnergy : JobDriver_JusticiarBase
    {
        protected override int WaitTicks => GenDate.TicksPerHour * 2;

        protected override float VeilRadius => 2.9f;

        protected override void EndInitAction()
        {
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PsychicAmplifier);
            if (hediff == null)
            {
                Hediff_Level hediff_Level = HediffMaker.MakeHediff(HediffDefOf.PsychicAmplifier, pawn, pawn.health.hediffSet.GetBrain()) as Hediff_Level;
                pawn.health.AddHediff(hediff_Level);
                hediff_Level.SetLevelTo(1);
            }
            else
            {
                Hediff_Level psylink = hediff as Hediff_Level;
                psylink.SetLevelTo(psylink.level + 1);
            }
            BlackVeil veil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
            veil.Radius = 7.9f;
            veil.ticksLeft = GenDate.TicksPerDay * 10;
            GenSpawn.Spawn(veil, TargetThing.Position, TargetThing.Map);
            TargetThing.Kill();
        }
    }
}
