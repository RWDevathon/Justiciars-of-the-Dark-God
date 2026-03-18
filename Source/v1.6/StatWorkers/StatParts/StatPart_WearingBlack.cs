using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    public class StatPart_WearingBlack : StatPart
    {
        public float requiredPercentage = 0.4f;

        public float factor = 1.0f;

        public override void TransformValue(StatRequest req, ref float val)
        {
            if (SurpassingNeededThreshold(req))
            {
                val *= factor;
            }
        }

        public override string ExplanationPart(StatRequest req)
        {
            if (SurpassingNeededThreshold(req))
            {
                return "JDG_Favor_WearingBlack".Translate(requiredPercentage.ToStringPercent(), factor.ToStringPercent());
            }
            return null;
        }

        private bool SurpassingNeededThreshold(StatRequest req)
        {
            if (req.HasThing && req.Thing is Pawn pawn)
            {
                Color black = new Color(0.2f, 0.2f, 0.2f);
                int numberBlack = 0;
                int numberTotal = 0;
                List<Apparel> wornApparel = pawn.apparel.WornApparel;
                for (int i = wornApparel.Count - 1; i >= 0; i--)
                {
                    CompColorable compColorable = wornApparel[i].TryGetComp<CompColorable>();
                    if (compColorable != null)
                    {
                        if (compColorable.Color.IndistinguishableFrom(black))
                        {
                            numberBlack++;
                        }
                        numberTotal++;
                    }
                }
                if (numberTotal <= 0)
                {
                    return false;
                }
                return (float)numberBlack / numberTotal > requiredPercentage;
            }
            return false;
        }
    }
}
