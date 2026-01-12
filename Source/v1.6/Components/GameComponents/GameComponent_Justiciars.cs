using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class GameComponent_Justiciars : GameComponent
    {
        // HashSet of all known justiciars to be used for various lookups.
        public HashSet<Pawn> allJusticiars = new HashSet<Pawn>();

        public GameComponent_Justiciars(Game game)
        {

        }
    }
}
