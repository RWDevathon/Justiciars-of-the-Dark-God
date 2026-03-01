using RimWorld;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    public class StatPart_WearingBlack : StatPart
    {
        public float requiredPercentage = 0.6f;

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
                foreach (Apparel item in pawn.apparel.WornApparel)
                {
                    CompColorable compColorable = item.TryGetComp<CompColorable>();
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
                return numberBlack / numberTotal > requiredPercentage;
            }
            return false;
        }
    }
}
