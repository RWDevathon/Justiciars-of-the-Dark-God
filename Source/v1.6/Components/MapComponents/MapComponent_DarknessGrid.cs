using System.Collections;
using System.Collections.Generic;
using Verse;

namespace ArtificialBeings
{
    public class MapComponent_DarknessGrid : MapComponent
    {
        public List<BlackVeil> blackVeils;

        private readonly BitArray darknessByCell;

        public MapComponent_DarknessGrid(Map map) : base(map)
        {
            darknessByCell = new BitArray(map.cellIndices.NumGridCells);
            blackVeils = new List<BlackVeil>();
        }

        public override void ExposeData()
        {
            MapExposeUtility.ExposeUint(map, (IntVec3 c) => darknessByCell[map.cellIndices.CellToIndex(c)] ? 1u : 0, delegate (IntVec3 c, uint val)
            {
                darknessByCell[map.cellIndices.CellToIndex(c)] = val == 1u;
            }, "ABF_darknessByCell");
        }
        
        public bool IsDark(IntVec3 cell)
        {
            return darknessByCell[CellIndicesUtility.CellToIndex(cell, map.Size.x)];
        }

        public bool IsDark(int index)
        {
            return darknessByCell[index];
        }

        public void SetDark(IntVec3 cell, bool isDark)
        {
            darknessByCell[CellIndicesUtility.CellToIndex(cell, map.Size.x)] = isDark;
        }

        public void SetDark(int index, bool isDark)
        {
            darknessByCell[index] = isDark;
        }
    }
}
