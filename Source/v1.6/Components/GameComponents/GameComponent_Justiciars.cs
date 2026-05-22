using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class GameComponent_Justiciars : GameComponent
    {
        // Dictionary of all known justiciars and their tracker hediff to be used for various lookups.
        public Dictionary<Pawn, Hediff_Justiciar> allJusticiars = new Dictionary<Pawn, Hediff_Justiciar>();

        // Dictionary of all known acolytes and their tracker hediff to be used for various lookups.
        public Dictionary<Pawn, Hediff_Acolyte> allAcolytes = new Dictionary<Pawn, Hediff_Acolyte>();

        // Dictionary of all known shadespirits and their tracker hediff to be used for various lookups.
        public Dictionary<Pawn, Hediff_ShadeSpirit> allShadeSpirits = new Dictionary<Pawn, Hediff_ShadeSpirit>();

        // This is the Menagerie Smuggler Network. It is only saved and referenced here - generated as needed and then saved from there.
        private SmugglerNetwork menagerieNetwork;

        public bool everSentMenagerieNotification = false;

        // Int storing a tick. If the current game tick is less than this value, Anomaly Harbinger trees may not spawn via their normal incident.
        public int tickProtectedAgainstHarbingerTreeSpawnsUntil = 0;

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
            Scribe_Values.Look(ref tickProtectedAgainstHarbingerTreeSpawnsUntil, "JDG_tickProtectedAgainstHarbingerTreeSpawnsUntil", 0);
        }
    }
}
