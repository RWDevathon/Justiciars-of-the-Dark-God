using RimWorld;
using System;
using System.Security.Cryptography;
using UnityEngine;
using Verse;

namespace ArtificialBeings
{
    // Teleports the caster to the target destination instantly. Can only be used while in darkness, and to destinations that are also dark.
    public class CompAbilityEffect_Shadestep : CompAbilityEffect
    {
        public new CompProperties_EffectWithDest Props => (CompProperties_EffectWithDest)props;

        private Pawn Caster => parent.pawn;

        public override bool CanCast
        {
            get
            {
                if (!Caster.Spawned || Caster.Map.glowGrid.GroundGlowAt(Caster.Position) >= 0.3)
                {
                    return false;
                }
                return base.CanCast;
            }
        }

        public override bool GizmoDisabled(out string reason)
        {
            reason = null;
            if (!Caster.Spawned || Caster.Map.glowGrid.GroundGlowAt(Caster.Position) >= 0.3)
            {
                reason = "JDG_PositionTooBright".Translate();
                return true;
            }
            return false;
        }

        public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
        {
            base.Apply(target, dest);
            if (!target.IsValid)
            {
                return;
            }
            BlackVeil veil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
            veil.Radius = 0.9f;
            veil.ticksLeft = 300;
            GenSpawn.Spawn(veil, Caster.Position, Caster.Map);
            Caster.Position = target.Cell;
            veil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
            veil.Radius = 0.9f;
            veil.ticksLeft = 300;
            GenSpawn.Spawn(veil, Caster.Position, Caster.Map);
            if ((Caster.Faction == Faction.OfPlayer || Caster.IsPlayerControlled) && Caster.Position.Fogged(Caster.Map))
            {
                FloodFillerFog.FloodUnfog(Caster.Position, Caster.Map);
            }
            Caster.stances.stunner.StunFor(1, Caster, addBattleLog: false, showMote: false);
            Caster.Notify_Teleported();
            if (Props.destClamorType != null)
            {
                GenClamor.DoClamor(Caster, target.Cell, Props.destClamorRadius, Props.destClamorType);
            }
        }

        public override bool Valid(LocalTargetInfo target, bool showMessages = true)
        {
            if (!target.IsValid || !target.Cell.StandableBy(Caster.Map, Caster))
            {
                return false;
            }
            if (Caster.Map.glowGrid.GroundGlowAt(target.Cell) >= 0.3)
            {
                return false;
            }
            return base.Valid(target, showMessages);
        }

        public override string ExtraLabelMouseAttachment(LocalTargetInfo target)
        {
            return CanShadestepTo(target).Reason;
        }

        private AcceptanceReport CanShadestepTo(LocalTargetInfo target)
        {
            if (target.IsValid && Caster.Map.glowGrid.GroundGlowAt(target.Cell) >= 0.3)
            {
                return "JDG_PositionTooBright".Translate();
            }
            return true;
        }
    }
}
