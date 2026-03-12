using HarmonyLib;
using System.Reflection;
using Verse;

namespace ArtificialBeings
{
    public class Justiciars : Mod
    {
        public Justiciars(ModContentPack content) : base(content)
        {
            new Harmony("Justiciars").PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [StaticConstructorOnStartup]
    public static class Justiciars_PostInit
    {
        static Justiciars_PostInit()
        {
            // Some patches can't be run with the other harmony patches as Defs aren't loaded yet. So we patch them here.
            if (PregnancyUtility_Patch.PregnancyUtility_ApplyBirthOutcome_Patch.Prepare())
            {
                new Harmony("Justiciars").CreateClassProcessor(typeof(PregnancyUtility_Patch.PregnancyUtility_ApplyBirthOutcome_Patch)).Patch();
            }
        }
    }
}