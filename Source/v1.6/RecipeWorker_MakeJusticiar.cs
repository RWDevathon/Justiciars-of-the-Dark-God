using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class RecipeWorker_MakeJusticiar : RecipeWorker
    {
        public override void Notify_IterationCompleted(Pawn billDoer, List<Thing> ingredients)
        {
            Find.WindowStack.Add(new Dialog_RaiseJusticiar(billDoer, billDoer.Position, billDoer.Map));
        }
    }
}
