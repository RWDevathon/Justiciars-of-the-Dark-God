using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    // Mod extension for ThingDefs to identify items which can be cloned via favors.
    public class UmbralClonableExtension : DefModExtension
    {
        public float favorCost = 0f;

        // If true, cloning this will generate a number equal to the current stack count. If false, it will only create 1, regardless of count.
        public bool useStackCount = false;

        public override IEnumerable<string> ConfigErrors()
        {
            if (favorCost <= 0f)
            {
                yield return "A ThingDef was identified as clonable but not assigned a non-zero, positive favor cost. It will never be clonable as a result.";
            }
        }
    }
}
