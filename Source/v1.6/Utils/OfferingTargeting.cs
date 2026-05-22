using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    public class OfferingTargeting : ITargetingSource
    {
        public Pawn caster;

        public OfferingTargeting(Pawn caster)
        {
            this.caster = caster;
        }

        public virtual TargetingParameters targetParams => new TargetingParameters
        {
            canTargetPawns = true,
            canTargetAnimals = false,
            canTargetMechs = false,
            canTargetItems = false,
            canTargetHumans = true,
            canTargetSubhumans = false,
            canTargetEntities = false,
            mustBeSelectable = true,
            canTargetSelf = false,
            mapObjectTargetsMustBeAutoAttackable = false,
            onlyTargetFactions = new List<Faction> { Faction.OfPlayer, },
        };

        public bool MultiSelect => false;

        public Thing Caster => caster;

        public Pawn CasterPawn => caster;

        public Verb GetVerb => null;

        public bool CasterIsPawn => true;

        public bool IsMeleeAttack => false;

        public bool Targetable => true;

        public Texture2D UIIcon => BaseContent.BadTex;

        public ITargetingSource DestinationSelector => null;

        public bool HidePawnTooltips => true;

        // This one is responsible for whether the target is legal ever.
        public bool CanHitTarget(LocalTargetInfo target)
        {
            if (!target.IsValid)
            {
                return false;
            }

            return target.Pawn != null;
        }

        // This one is responsible for whether the target is legal right now.
        public bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            if (target.Pawn is Pawn pawn && GenSight.LineOfSight(Caster.Position, pawn.Position, Caster.Map))
            {
                return pawn.IsFreeNonSlaveColonist || (pawn.HostFaction == Faction.OfPlayer && pawn.GuestStatus == GuestStatus.Guest);
            }
            return false;
        }

        public void DrawHighlight(LocalTargetInfo target)
        {
            if (target.IsValid)
            {
                GenDraw.DrawTargetHighlight(target);
            }
        }

        public void OnGUI(LocalTargetInfo target)
        {
            Texture2D icon;
            if (ValidateTarget(target) && target.Thing is Pawn pawn)
            {
                icon = TexCommand.Attack;
                float favorToAward = 25f;
                if (pawn.IsFreeNonSlaveColonist)
                {
                    favorToAward = 50f;
                }

                // Youths are worth vastly less as a base. They are too young and innocent to understand the depths of pain.
                switch (pawn.DevelopmentalStage)
                {
                    case DevelopmentalStage.Newborn:
                        favorToAward *= 0.05f;
                        break;
                    case DevelopmentalStage.Baby:
                        favorToAward *= 0.1f;
                        break;
                    case DevelopmentalStage.Child:
                        favorToAward *= 0.2f;
                        break;
                }

                if (pawn.health.hediffSet.GetFirstHediff<Hediff_Devotee>() is Hediff_Devotee devoteeHediff)
                {
                    favorToAward += devoteeHediff.FavorCurrent;
                }

                string text = "JDG_OfferingFavorSingleConspirator".Translate(CasterPawn, favorToAward.ToString("F2"));
                Widgets.MouseAttachedLabel(text);
            }
            else
            {
                icon = TexCommand.CannotShoot;
            }
            GenUI.DrawMouseAttachment(icon);
        }

        public void OrderForceTarget(LocalTargetInfo target)
        {
            if (!(target.Pawn is Pawn pawn))
            {
                return;
            }

            if (!(pawn.health.hediffSet.GetFirstHediff<Hediff_Offering>() is Hediff_Offering offeringHediff))
            {
                offeringHediff = (Hediff_Offering)pawn.health.AddHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_MarkedOffering);
                if (pawn.IsFreeNonSlaveColonist)
                {
                    offeringHediff.favorToAward = 50f;
                }
                else
                {
                    offeringHediff.favorToAward = 25f;
                }

                // Youths are worth vastly less. They are too young and innocent to understand the depths of pain.
                switch (pawn.DevelopmentalStage)
                {
                    case DevelopmentalStage.Newborn:
                        offeringHediff.favorToAward *= 0.05f;
                        break;
                    case DevelopmentalStage.Baby:
                        offeringHediff.favorToAward *= 0.1f;
                        break;
                    case DevelopmentalStage.Child:
                        offeringHediff.favorToAward *= 0.2f;
                        break;
                }
            }
            offeringHediff.conspirators.Add(CasterPawn);

            // The caster should not be able to mark another target again for one day.
            CasterPawn.health.hediffSet.GetFirstHediff<Hediff_Devotee>().tickToNextMark = GenTicks.TicksGame + GenDate.TicksPerDay;
        }
    }
}
