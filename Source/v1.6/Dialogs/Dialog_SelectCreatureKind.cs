using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace ArtificialBeings
{
    // This class is a modified replica of Dialog_ChangeDryadCaste. No attempts at changing the UI were made save what was absolutely necesssary.
    // This class has been left with significant hardcoding. This is to get it operational quicker, without worrying about extensibility.
    public class Dialog_SelectCreatureKind : Window
    {
        private ThingComp_UmbralAnchor umbralAnchor;

        private Vector2 scrollPosition;

        private PawnKindDef selectedKind;

        private PawnKindDef currentKind;

        private const float rightViewWidth = 210f;

        private const float HeaderHeight = 35f;

        private const float LeftRectWidth = 400f;

        private const float ChangeFormButtonHeight = 55f;

        private static readonly Vector2 OptionSize = new Vector2(190f, 46f);

        private static readonly Vector2 ButSize = new Vector2(200f, 40f);

        public override Vector2 InitialSize => new Vector2(Mathf.Min(900, UI.screenWidth), 650f);

        public Dialog_SelectCreatureKind(ThingComp_UmbralAnchor anchor)
        {
            umbralAnchor = anchor;
            currentKind = anchor.nextToSpawn;
            selectedKind = currentKind;
            forcePause = true;
            closeOnAccept = false;
            doCloseX = true;
            doCloseButton = true;
        }

        public override void PreOpen()
        {
            if (!ModLister.CheckIdeology("Dryad upgrades"))
            {
                Close();
            }
            base.PreOpen();
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            string label = (selectedKind != null) ? selectedKind.LabelCap : "JDG_SelectCreatureType".Translate();
            Widgets.Label(new Rect(inRect.x, inRect.y, inRect.width, HeaderHeight), label);
            Text.Font = GameFont.Small;
            float num = inRect.y + HeaderHeight + 10f;
            float curY = num;
            float num2 = inRect.height - num;
            num2 -= ButSize.y + 10f;
            DrawLeftRect(new Rect(inRect.xMin, num, LeftRectWidth, num2), ref curY);
            DrawRightRect(new Rect(inRect.x + LeftRectWidth + 17f, num, inRect.width - LeftRectWidth - 17f, num2));
        }

        private void DrawLeftRect(Rect rect, ref float curY)
        {
            Rect rect2 = new Rect(rect.x, curY, rect.width, rect.height);
            rect2.yMax = rect.yMax;
            Rect rect3 = rect2.ContractedBy(4f);
            if (selectedKind == null)
            {
                return;
            }
            Widgets.Label(rect3.x, ref curY, rect3.width, selectedKind.race.description);
            curY += 10f;
            if (HeraldCheck(selectedKind))
            {
                Widgets.Label(rect3.x, ref curY, rect3.width, "RequiredMemes".Translate() + ":");
                string text = "  - " + MemeDefOf.TreeConnection.LabelCap.ToString().Colorize(TreeConnectionCheck() ? Color.white : ColorLibrary.RedReadable);
                Widgets.Label(rect3.x, ref curY, rect3.width, text);
                curY += 10f;
            }
            Widgets.HyperlinkWithIcon(new Rect(rect3.x, curY, rect3.width, Text.LineHeight), new Dialog_InfoCard.Hyperlink(selectedKind.race));
            curY += Text.LineHeight + 10f;
            Rect rect4 = new Rect(rect3.x, rect3.yMax - ChangeFormButtonHeight, rect3.width, ChangeFormButtonHeight);
            if (selectedKind == currentKind)
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.DrawHighlight(rect4);
                Widgets.Label(rect4.ContractedBy(5f), "AlreadySelected".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
            }
            else if (HeraldCheck(selectedKind) && !TreeConnectionCheck())
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.DrawHighlight(rect4);
                Widgets.Label(rect4.ContractedBy(5f), "MissingRequiredMemes".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
            }
            else if (Widgets.ButtonText(rect4, "Accept".Translate()))
            {
                umbralAnchor.nextToSpawn = selectedKind;
                Close();
            }
        }

        private void DrawRightRect(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
            Rect outerRect = new Rect(0f, 0f, rightViewWidth, rect.height - 16f);
            Rect innerRect = outerRect.ContractedBy(10f);
            Widgets.ScrollHorizontal(rect, ref scrollPosition, outerRect);
            Widgets.BeginScrollView(rect, ref scrollPosition, outerRect);
            Widgets.BeginGroup(innerRect);
            for (int i = 0; i < JDG_Utils.darkCreatureKinds.Count; i++)
            {
                PawnKindDef kindDef = JDG_Utils.darkCreatureKinds[i];
                Vector2 position = new Vector2(0f, (rect.height - OptionSize.y) * (0.2f * i));
                Rect rect2 = new Rect(position.x, position.y, OptionSize.x, OptionSize.y);
                Widgets.DrawBoxSolidWithOutline(rect2, GetBoxColor(kindDef), GetBoxOutlineColor(kindDef));
                Rect rect3 = new Rect(rect2.x, rect2.y, rect2.height, rect2.height);
                Widgets.DefIcon(rect3.ContractedBy(4f), kindDef);
                GUI.color = (!HeraldCheck(kindDef) || TreeConnectionCheck()) ? Color.white : ColorLibrary.RedReadable;
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(new Rect(rect3.xMax, rect2.y, rect2.width - rect3.width, rect2.height).ContractedBy(4f), kindDef.LabelCap);
                Text.Anchor = TextAnchor.UpperLeft;
                GUI.color = Color.white;
                if (Widgets.ButtonInvisible(rect2))
                {
                    selectedKind = kindDef;
                    SoundDefOf.Click.PlayOneShotOnCamera();
                }
            }
            Widgets.EndGroup();
            Widgets.EndScrollView();
        }

        private Color GetBoxColor(PawnKindDef kind)
        {
            Color result = TexUI.AvailResearchColor;
            if (kind == currentKind)
            {
                result = TexUI.OldActiveResearchColor;
            }
            else if (HeraldCheck(kind) && !TreeConnectionCheck())
            {
                result = TexUI.LockedResearchColor;
            }
            if (selectedKind == kind)
            {
                result += TexUI.HighlightBgResearchColor;
            }
            return result;
        }

        private Color GetBoxOutlineColor(PawnKindDef kind)
        {
            if (selectedKind != null && selectedKind == kind)
            {
                return TexUI.HighlightBorderResearchColor;
            }
            return TexUI.DefaultBorderResearchColor;
        }

        private bool HeraldCheck(PawnKindDef kindDef)
        {
            return Find.IdeoManager.classicMode || kindDef == JDG_PawnKindDefOf.ABF_PawnKind_Justiciar_Player_CreatureHerald;
        }

        private bool TreeConnectionCheck()
        {
            return Faction.OfPlayer.ideos.PrimaryIdeo.HasMeme(MemeDefOf.TreeConnection);
        }
    }
}
