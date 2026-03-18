using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace ArtificialBeings
{
    // A smuggler network should exist only once per game, never disappears, and has no faction or map.
    public class SmugglerNetwork : IExposable, ICommunicable, ITrader, IThingHolder, ILoadReferenceable
    {
        private ThingOwner<Thing> stock;

        public bool tradedWithPlayerThisQuadrum = false;

        public string TraderName => "JDG_MenagerieCallLabel".Translate();

        public bool CanTradeNow => true;

        public float TradePriceImprovementOffsetForPlayer => 0f;

        public TradeCurrency TradeCurrency => TradeCurrency.Silver;

        public Faction Faction => null;

        public IThingHolder ParentHolder => null;

        public TraderKindDef TraderKind => JDG_TraderKindDefOf.ABF_TraderKind_Justiciar_Menagerie;

        public int RandomPriceFactorSeed => 0;

        private ThingOwner<Thing> Stock
        {
            get
            {
                if (stock == null)
                {
                    stock = new ThingOwner<Thing>();
                }
                return stock;
            }
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref tradedWithPlayerThisQuadrum, "JDG_tradedWithPlayerThisQuadrum", false);
            Scribe_Deep.Look(ref stock, "JDG_smugglerNetworkStock", this);
        }

        public string GetCallLabel()
        {
            return TraderName;
        }

        public string GetInfoText()
        {
            return "JDG_MenagerieInfoText".Translate();
        }

        public string GetUniqueLoadID()
        {
            return "SmugglerNetwork";
        }

        public IEnumerable<Thing> Goods
        {
            get
            {
                for (int i = 0; i < Stock.Count; i++)
                {
                    yield return stock[i];
                }
            }
        }

        public bool CanCommunicateWith(Pawn negotiator)
        {
            if (negotiator.skills == null || negotiator.skills.GetSkill(SkillDefOf.Social).TotallyDisabled)
            {
                return new AcceptanceReport("IncapableOfCapacity".Translate(SkillDefOf.Social.label));
            }
            if (!JDG_Utils.IsJusticiar(negotiator))
            {
                return new AcceptanceReport("JDG_CannotTradeNotJusticiar".Translate());
            }
            return AcceptanceReport.WasAccepted;
        }

        public void TryOpenComms(Pawn negotiator)
        {
            if (JDG_Utils.IsJusticiar(negotiator))
            {
                if (!tradedWithPlayerThisQuadrum)
                {
                    ThingSetMakerParams parms = new ThingSetMakerParams
                    {
                        traderDef = TraderKind,
                    };
                    Stock.TryAddRangeOrTransfer(ThingSetMakerDefOf.TraderStock.root.Generate(parms));
                    foreach (Thing stockItem in Stock)
                    {
                        // All "stock" generated for smuggler networks are shadespirits.
                        if (stockItem is Pawn pawn)
                        {
                            pawn.health.AddHediff(JDG_HediffDefOf.ABF_Hediff_Justiciar_ShadeSpirit);
                        }
                    }
                }
                tradedWithPlayerThisQuadrum = true;
                Find.WindowStack.Add(new Dialog_Trade(negotiator, this));
                LessonAutoActivator.TeachOpportunity(ConceptDefOf.BuildOrbitalTradeBeacon, OpportunityType.Critical);
                TutorUtility.DoModalDialogIfNotKnown(ConceptDefOf.TradeGoodsMustBeNearBeacon);
            }
        }

        public FloatMenuOption CommFloatMenuOption(Building_CommsConsole console, Pawn negotiator)
        {
            string label = "CallOnRadio".Translate(GetCallLabel());
            Action action = null;
            AcceptanceReport canCommunicate = CanCommunicateWith(negotiator);
            if (!canCommunicate.Accepted)
            {
                if (!canCommunicate.Reason.NullOrEmpty())
                {
                    action = delegate
                    {
                        Messages.Message(canCommunicate.Reason, console, MessageTypeDefOf.RejectInput, historical: false);
                    };
                }
            }
            else
            {
                action = delegate
                {
                    if (!Building_OrbitalTradeBeacon.AllPowered(console.Map).Any())
                    {
                        Messages.Message("JDG_NeedBeaconToTradeWithNetwork".Translate(), console, MessageTypeDefOf.RejectInput, historical: false);
                    }
                    else
                    {
                        console.GiveUseCommsJob(negotiator, this);
                    }
                };
            }
            return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action, MenuOptionPriority.InitiateSocial), negotiator, console);
        }

        public Faction GetFaction()
        {
            return null;
        }

        // Smuggler Networks only accept silver.
        public IEnumerable<Thing> ColonyThingsWillingToBuy(Pawn playerNegotiator)
        {
            foreach (Thing item in TradeUtility.AllLaunchableThingsForTrade(playerNegotiator.Map, this))
            {
                if (item.def == ThingDefOf.Silver)
                {
                    yield return item;
                }
            }
        }

        public void GiveSoldThingToTrader(Thing toGive, int countToGive, Pawn playerNegotiator)
        {
            Thing thing = toGive.SplitOff(countToGive);
            thing.PreTraded(TradeAction.PlayerSells, playerNegotiator, this);
            Thing thing2 = TradeUtility.ThingFromStockToMergeWith(this, thing);
            if (thing2 != null)
            {
                if (!thing2.TryAbsorbStack(thing, respectStackLimit: false))
                {
                    thing.Destroy();
                }
                return;
            }
            stock.TryAdd(thing, canMergeWithExistingStacks: false);
        }

        public void GiveSoldThingToPlayer(Thing toGive, int countToGive, Pawn playerNegotiator)
        {
            Thing thing = toGive.SplitOff(countToGive);
            thing.PreTraded(TradeAction.PlayerBuys, playerNegotiator, this);
            // "Stock" comes fully trained, but we cannot assign training until it belongs to the player, which PreTraded handles.
            if (thing is Pawn pawn)
            {
                foreach (TrainableDef trainableDef in TrainableUtility.TrainableDefsInListOrder)
                {
                    if (pawn.training.CanAssignToTrain(trainableDef))
                    {
                        pawn.training.Train(trainableDef, null, true);
                        pawn.training.SetWantedRecursive(trainableDef, true);
                    }
                }
            }
            TradeUtility.SpawnDropPod(DropCellFinder.TradeDropSpot(playerNegotiator.Map), playerNegotiator.Map, thing);
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return stock;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }

        public void ClearStock()
        {
            if (tradedWithPlayerThisQuadrum)
            {
                Messages.Message("JDG_SmugglerNetworkStockReset".Translate(GetCallLabel()), MessageTypeDefOf.PositiveEvent);
                tradedWithPlayerThisQuadrum = false;
            }
            Stock.ClearAndDestroyContentsOrPassToWorld();
        }
    }
}
