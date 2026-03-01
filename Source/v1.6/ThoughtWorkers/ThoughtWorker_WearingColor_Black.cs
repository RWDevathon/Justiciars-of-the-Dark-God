using RimWorld;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    public class ThoughtWorker_WearingColor_Black : ThoughtWorker_WearingColor
    {
        protected override Color? Color(Pawn p)
        {
            return new Color(0.2f, 0.2f, 0.2f); // This is what RW uses for black. The ColorDef lives in Ideology, which isn't helpful for us.
        }
    }
}
