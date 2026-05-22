using Verse;

namespace ArtificialBeings
{
    // A simple comp that applies a mirror necklace hediff to a justiciar when equipped. It does nothing if the pawn isn't one.
    public class ThingComp_MirrorNecklace : ThingComp
    {
        private Hediff_MirrorNecklace hediff;

        private ThingCompProperties_MirrorNecklace Props => (ThingCompProperties_MirrorNecklace)props;

        public override void Notify_Equipped(Pawn pawn)
        {
            if (!JDG_Utils.IsJusticiar(pawn))
            {
                return;
            }

            BodyPartRecord bodyPartRecord = null;
            if (Props.bodyPartGroup != null)
            {
                foreach (BodyPartRecord part in pawn.health.hediffSet.GetNotMissingParts())
                {
                    if (part.IsInGroup(Props.bodyPartGroup))
                    {
                        bodyPartRecord = part;
                        break;
                    }
                }
            }
            Hediff targetHediff = pawn.health.AddHediff(Props.hediff, bodyPartRecord);
            if (targetHediff != null && targetHediff is Hediff_MirrorNecklace hediffMirrorNecklace)
            {
                hediffMirrorNecklace.connectedThing = parent;
                hediff = hediffMirrorNecklace;
            }
        }

        public override void PostExposeData()
        {
            Scribe_References.Look(ref hediff, "JDG_mirrorNecklaceHediff", false);
        }
    }
}
