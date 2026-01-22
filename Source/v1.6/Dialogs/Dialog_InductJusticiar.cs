using RimWorld;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    public class Dialog_InductJusticiar : Window
    {
        private Pawn pawn;

        public Dialog_InductJusticiar(Pawn target)
        {
            forcePause = true;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnCancel = false;
            pawn = target;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect TitleRect = new Rect(inRect);
            TitleRect.height = 2f * Text.LineHeight;
            Widgets.Label(TitleRect, "JDG_ChooseJusticiarPath".Translate() + ": " + Find.ActiveLanguageWorker.WithDefiniteArticle(pawn.Name.ToStringShort, pawn.gender, plural: false, name: true).ApplyTag(TagType.Name));
            inRect.yMin = TitleRect.yMax;
            Widgets.DrawLineHorizontal(inRect.x, inRect.y, inRect.width);
            Widgets.DrawLineVertical(inRect.width / 2, inRect.y + Margin, inRect.height - Margin - (2f * Text.LineHeight));

            // Darklurker Path
            Rect leftRect = new Rect(inRect);
            leftRect.xMax = (inRect.width / 2) - Margin;
            Rect leftHeaderRect = new Rect(leftRect);
            leftHeaderRect.height = 2f * Text.LineHeight;
            Widgets.Label(leftHeaderRect, "JDG_JusticiarPath_DarkLurker".Translate());
            Rect leftContentRect = new Rect(leftRect);
            leftContentRect.yMin = leftHeaderRect.yMax;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(leftContentRect, "JDG_JusticiarPath_DarkLurker_Desc".Translate());
            if (Widgets.ButtonText(new Rect(leftRect.x, leftRect.yMax - (2f * Text.LineHeight), leftRect.width, 2f * Text.LineHeight), "Accept".Translate()))
            {
                FinalizeChoice(JDG_PawnKindDefOf.ABF_PawnKind_Justiciar_Player_DarkLurker);
            }
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.MiddleCenter;

            // Soulhunter Path
            Rect rightRect = new Rect(leftRect);
            rightRect.xMax = inRect.xMax;
            rightRect.xMin = leftRect.xMax + (2 * Margin);
            Rect rightHeaderRect = new Rect(rightRect);
            rightHeaderRect.height = 2f * Text.LineHeight;
            Widgets.Label(rightHeaderRect, "JDG_JusticiarPath_SoulHunter".Translate());
            Rect rightContentRect = new Rect(rightRect);
            rightContentRect.yMin = rightHeaderRect.yMax;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperCenter;
            Widgets.Label(rightContentRect, "JDG_JusticiarPath_SoulHunter_Desc".Translate());
            if (Widgets.ButtonText(new Rect(rightRect.x, rightRect.yMax - (2f * Text.LineHeight), rightRect.width, 2f * Text.LineHeight), "Accept".Translate()))
            {
                FinalizeChoice(JDG_PawnKindDefOf.ABF_PawnKind_Justiciar_Player_SoulHunter);
            }
            Text.Font = GameFont.Medium;
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void FinalizeChoice(PawnKindDef justiciarChoice)
        {
            JDG_Utils.ConvertIntoJusticiar(pawn, justiciarChoice);
            Close();
        }
    }
}
