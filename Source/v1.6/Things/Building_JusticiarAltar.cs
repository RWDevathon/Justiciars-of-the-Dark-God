using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class Building_JusticiarAltar : Building_WorkTable
    {
        // The violent destruction of an altar is an affront to the Dark God.
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (mode == DestroyMode.KillFinalize)
            {
                JDG_Utils.DispleaseJusticiars("JDG_AltarDestruction", 15f);
            }
            base.Destroy(mode);
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption option in base.GetFloatMenuOptions(selPawn)) 
            {
                yield return option;
            }

            if (!JDG_Utils.IsJusticiar(selPawn))
            {
                yield break;
            }
            Hediff_Justiciar justiciarHediff = JDG_Utils.GetJusticiarHediff(selPawn);
        }
    }
}
