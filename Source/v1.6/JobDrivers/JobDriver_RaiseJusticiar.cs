using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace ArtificialBeings
{
    public class JobDriver_RaiseJusticiar : JobDriver
    {
        public Corpse Corpse => job.GetTarget(TargetIndex.A).Thing as Corpse;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            pawn.Reserve(Corpse, job);
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil raisingJusticiar = Toils_General.Wait(600).WithProgressBarToilDelay(TargetIndex.A);
            raisingJusticiar.initAction = delegate
            {
                BlackVeil veil = (BlackVeil)ThingMaker.MakeThing(JDG_ThingDefOf.ABF_Thing_BlackVeil);
                veil.Radius = 0.9f;
                veil.ticksLeft = 900;
                GenSpawn.Spawn(veil, Corpse.Position, Corpse.Map);
            };
            yield return raisingJusticiar;
            Toil raiseJusticiar = ToilMaker.MakeToil("MakeNewToils");
            raiseJusticiar.initAction = delegate
            {
                Pawn justiciar = Corpse.InnerPawn;
                // If the new justiciar has no path selected, they must choose one. If they have a path already, just revive them.
                if (!(justiciar.story.traits.GetTrait(JDG_TraitDefOf.ABF_Trait_Justiciar_Adherent) is Trait adherentTrait) || adherentTrait.Degree == 0)
                {
                    Find.WindowStack.Add(new Dialog_RaiseJusticiar(justiciar, Corpse.Position, Corpse.Map));
                    // Raising a brand new justiciar can fulfill an ambition for the person who raised them.
                    if (pawn.health.hediffSet.TryGetHediff<Hediff_Ambition_RecruitmentMotivation>(out var hediff) && !hediff.complete)
                    {
                        hediff.NotifySucceeded();
                    }
                }
                else
                {
                    ResurrectionUtility.TryResurrect(justiciar);
                    justiciar.drafter.Drafted = true; // Try to keep the justiciar still on spawn so they don't immediately run off.
                }
            };
            raiseJusticiar.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return raiseJusticiar;
        }
    }
}
