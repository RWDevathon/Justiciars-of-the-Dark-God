using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    // Class for justiciar invisibility. Instantly ends if stepping out of conjured darkness.
    public class Hediff_UmbralStride : HediffWithComps
    {
        public override bool ShouldRemove => false;

        public override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (pawn.Spawned && !JDG_Utils.IsDark(pawn.Position, pawn.Map))
            {
                pawn.health.RemoveHediff(this);
            }
        }

        // Allow the player to manually end the umbral stride effect, if they so desire.
        public override IEnumerable<Gizmo> GetGizmos()
        {
            Command_Action endStride = new Command_Action
            {
                defaultLabel = "JDG_EndUmbralStride".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Abilities/Darkness"),
                defaultDesc = "JDG_EndUmbralStrideDesc".Translate(),
                action = delegate
                {
                    pawn.health.RemoveHediff(this);
                }
            };
            yield return endStride;
            yield break;
        }
    }
}
