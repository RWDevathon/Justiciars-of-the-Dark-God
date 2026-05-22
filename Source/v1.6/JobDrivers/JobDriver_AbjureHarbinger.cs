using RimWorld;
using Verse;

namespace ArtificialBeings
{
    public class JobDriver_AbjureHarbinger : JobDriver_JusticiarBase
    {
        protected override void EndInitAction()
        {
            // Visual / Audio effects
            IntVec3 position = TargetThing.Position;
            Map map = TargetThing.Map;
            JDG_EffecterDefOf.ABF_Effecter_Justiciar_DarkAnnihilation_Mini.Spawn(position, map).Cleanup();
            BlackVeil veil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
            veil.Radius = 0.9f;
            veil.ticksLeft = 600;
            veil.opacityRange = new FloatRange(0.2f, 0.5f);
            GenSpawn.Spawn(veil, position, map);

            GameComponent_Justiciars gameComp = Current.Game.GetComponent<GameComponent_Justiciars>();
            int newTick = gameComp.tickProtectedAgainstHarbingerTreeSpawnsUntil;
            int currentTick = GenTicks.TicksGame;
            if (newTick < currentTick)
            {
                newTick = currentTick;
            }
            newTick += GenDate.TicksPerYear * 1;
            gameComp.tickProtectedAgainstHarbingerTreeSpawnsUntil = newTick;
            Find.LetterStack.ReceiveLetter("JDG_ProtectedAgainstHarbingerSpawns".Translate(), "JDG_ProtectedAgainstHarbingerSpawnsDesc".Translate(pawn.LabelShortCap, (newTick - currentTick).ToStringTicksToPeriod()), LetterDefOf.PositiveEvent);
            TargetThing.Destroy();
            JDG_Utils.GetJusticiarHediff(pawn).NotifyFavorLost(JDG_Utils.favorCostToAbjure);
        }
    }
}
