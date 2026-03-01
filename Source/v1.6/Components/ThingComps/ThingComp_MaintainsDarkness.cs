using RimWorld;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    public class ThingComp_MaintainsDarkness : ThingComp
    {
        public IJusticiarMaintainable maintainee;

        public ThingCompProperties_MaintainsDarkness Props => (ThingCompProperties_MaintainsDarkness)props;

        public override void PostExposeData()
        {
            Scribe_References.Look(ref maintainee, "JDG_maintainee");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (maintainee != null)
                {
                    maintainee.Maintainer = parent;
                }
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (maintainee == null)
            {
                BlackVeil blackVeil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
                blackVeil.Radius = Props.radius;
                blackVeil.ticksLeft = 60;
                GenSpawn.Spawn(blackVeil, parent.Position, parent.Map);
                blackVeil.Maintainer = parent;
            }
            // Set the radius, in the event that the properties changed between saves.
            else if (maintainee is BlackVeil veil)
            {
                veil.Radius = Props.radius;
            }
        }

        public void NotifyNewMaintainee(IJusticiarMaintainable newMaintainee)
        {
            maintainee = newMaintainee;
            maintainee.Maintainer = parent;
        }
    }
}
