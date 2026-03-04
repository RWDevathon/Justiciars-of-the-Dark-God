using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class GameComponent_Justiciars : GameComponent
    {
        // HashSet of all known justiciars to be used for various lookups.
        public HashSet<Pawn> allJusticiars = new HashSet<Pawn>();

        // HashSet of all known acolytes to be used for various lookups.
        public HashSet<Pawn> allAcolytes = new HashSet<Pawn>();

        // HashSet of all known shadespirits to be used for various lookups.
        public HashSet<Pawn> allShadeSpirits = new HashSet<Pawn>();

        // This is the Menagerie Smuggler Network. It is only saved and referenced here - generated as needed and then saved from there.
        private SmugglerNetwork menagerieNetwork;

        public bool everSentMenagerieNotification = false;

        public SmugglerNetwork MenagerieNetwork
        {
            get
            {
                if (menagerieNetwork == null)
                {
                    menagerieNetwork = new SmugglerNetwork();
                }
                return menagerieNetwork;
            }
        }

        public GameComponent_Justiciars(Game game)
        {

        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();
            if (menagerieNetwork != null && GenTicks.TicksGame % GenDate.TicksPerQuadrum == 0)
            {
                MenagerieNetwork.ClearStock();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref menagerieNetwork, "JDG_menagerieNetwork");
            Scribe_Values.Look(ref everSentMenagerieNotification, "JDG_everSentMenagerieNotification");
        }
    }
}
