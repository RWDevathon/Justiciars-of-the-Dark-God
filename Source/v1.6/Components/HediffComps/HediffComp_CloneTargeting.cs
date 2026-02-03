using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class HediffComp_CloneTargeting : HediffComp, ITargetingSource
    {
        public Hediff_Justiciar Parent => parent as Hediff_Justiciar;

        public virtual TargetingParameters targetParams => new TargetingParameters
        {
            canTargetPawns = true,
            canTargetAnimals = true,
            canTargetMechs = true,
            canTargetItems = true,
            canTargetHumans = false,
            canTargetSubhumans = false,
            canTargetEntities = false,
            mustBeSelectable = true,
            mapObjectTargetsMustBeAutoAttackable = false,
            onlyTargetFactions = new List<Faction> { Faction.OfPlayer, },
        };

        public bool MultiSelect => false;

        public Thing Caster => parent.pawn;

        public Pawn CasterPawn => parent.pawn;

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

            return JDG_Utils.FavorCostToClone(target.Thing) > 0f;
        }

        // This one is responsible for whether the target is legal right now.
        public bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            return JDG_Utils.FavorCostToClone(target.Thing) <= Parent.FavorCurrent;
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
            if (CanHitTarget(target))
            {
                float favorCost = JDG_Utils.FavorCostToClone(target.Thing);
                string text = "JDG_InsufficientFavorToClone".Translate(favorCost.ToString("F2"));
                if (!ValidateTarget(target))
                {
                    icon = TexCommand.CannotShoot;
                    text = text.Colorize(Color.red);
                }
                else
                {
                    icon = TexCommand.Attack;
                }
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
            Job job = JobMaker.MakeJob(JDG_JobDefOf.ABF_Job_Justiciar_Clone, target);
            CasterPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }
    }
}
