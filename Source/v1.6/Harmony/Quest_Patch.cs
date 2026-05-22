using HarmonyLib;
using RimWorld;

namespace ArtificialBeings
{
    public class Quest_Patch
    {
        // The Dark God may pass judgement on how certain quests are ended.
        [HarmonyPatch(typeof(Quest), "End")]
        public static class Quest_End_Patch
        {
            [HarmonyPostfix]
            public static void Listener(QuestEndOutcome outcome, Quest __instance)
            {
                // Succeeding in charitable quests is frowned upon.
                if (__instance.root.defaultCharity && (outcome == QuestEndOutcome.Success || outcome == QuestEndOutcome.Unknown))
                {
                    JDG_Utils.DispleaseJusticiars("JDG_CharityPermitted", 10f);
                }
            }
        }
    }
}
