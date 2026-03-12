using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace ArtificialBeings
{
    // This hediff identifies the individual as being marked as an offering for an acolyte/justiciar. If this pawn dies before it disappears, they will get favor.
    public class Hediff_Offering : HediffWithComps
    {
        public List<Pawn> conspirators = new List<Pawn>();

        public float favorToAward = 0f;

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            base.Notify_PawnDied(dinfo, culprit);

            // Confirm that the conspirators can actually receive favor before partitioning the favor between them evenly.
            List<Hediff_Devotee> devoteeHediffs = new List<Hediff_Devotee>();
            for (int i = conspirators.Count - 1; i >= 0; i--)
            {
                if (conspirators[i].health.hediffSet.GetFirstHediff<Hediff_Devotee>() is Hediff_Devotee devoteeHediff)
                {
                    devoteeHediffs.Add(devoteeHediff);
                }
            }
            if (devoteeHediffs.Count > 0)
            {
                // Given this pawn is about to cease to exist, all favor they currently possess (assuming they are a devotee) should also be split among the conspirators.
                if (JDG_Utils.IsDevotee(pawn))
                {
                    favorToAward += pawn.health.hediffSet.GetFirstHediff<Hediff_Devotee>().FavorCurrent;
                }

                float splitFavor = favorToAward / devoteeHediffs.Count;
                for (int i = devoteeHediffs.Count - 1; i >= 0; i--)
                {
                    devoteeHediffs[i].NotifyFavorGained(splitFavor);
                }

                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("JDG_OfferingMadeDesc".Translate(favorToAward.ToString("F2")));
                for (int i = conspirators.Count - 1; i >= 0; --i)
                {
                    stringBuilder.AppendLine("JDG_OfferingFavorConspiratorListItem".Translate(conspirators[i].LabelCap));
                }
                Find.LetterStack.ReceiveLetter("JDG_OfferingMade".Translate(), stringBuilder.ToString(), LetterDefOf.PositiveEvent, conspirators);
            }
            pawn.health.RemoveHediff(this);
            pawn.equipment.DestroyAllEquipment();
            if (pawn.MapHeld != null)
            {
                JDG_EffecterDefOf.ABF_Effecter_Justiciar_DarkAnnihilation_Mini.Spawn(pawn.PositionHeld, pawn.MapHeld).Cleanup();
            }
            pawn.Corpse?.Destroy();
        }

        public override string TipStringExtra
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(base.TipStringExtra);
                if (conspirators.Count > 0)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.AppendLine();
                    }

                    float favorToSplit = favorToAward;
                    // Given this pawn is about to cease to exist, all favor they currently possess (assuming they are a devotee) should also be split among the conspirators.
                    if (JDG_Utils.IsDevotee(pawn))
                    {
                        favorToSplit += pawn.health.hediffSet.GetFirstHediff<Hediff_Devotee>().FavorCurrent;
                    }

                    if (conspirators.Count == 1)
                    {
                        stringBuilder.AppendLine("JDG_OfferingFavorSingleConspirator".Translate(conspirators[0].NameShortColored, favorToSplit.ToString("F2")));
                    }
                    else
                    {
                        stringBuilder.AppendLine("JDG_OfferingFavorMultipleConspirators".Translate(favorToSplit.ToString("F2")));
                        for (int i = conspirators.Count - 1; i >= 0; --i)
                        {
                            stringBuilder.AppendLine("JDG_OfferingFavorConspiratorListItem".Translate(conspirators[i].LabelCap));
                        }
                    }
                }
                return stringBuilder.ToString();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref favorToAward, "JDG_favorToAward");
            Scribe_Collections.Look(ref conspirators, "JDG_conspiratorsForOffering", LookMode.Reference);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                for (int i = conspirators.Count - 1; i >= 0; i--)
                {
                    if (!(conspirators[i] is Pawn pawn) || pawn.Dead || pawn.Destroyed)
                    {
                        conspirators.RemoveAt(i);
                    }
                }
            }
        }
    }
}
