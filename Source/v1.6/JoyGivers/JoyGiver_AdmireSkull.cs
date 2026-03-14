using RimWorld;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class JoyGiver_AdmireSkull : JoyGiver
    {
        public override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.Spawned && JDG_Utils.IsJusticiar(pawn) && GenClosest.ClosestThingReachable(pawn.PositionHeld, pawn.MapHeld, ThingRequest.ForDef(ThingDefOf.Skull), PathEndMode.OnCell, TraverseParms.For(pawn), validator: (thing) => !thing.IsForbidden(pawn) && pawn.CanReserve(thing, stackCount: 1)) is Thing skull)
            {
                return JobMaker.MakeJob(def.jobDef, skull);
            }
            return null;
        }
    }
}
