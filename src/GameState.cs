﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("CodingameTests")]

namespace Codingame
{
    internal class GameState
    {
        internal int MapWidth { get; set; }
        internal int MapHeight { get; set; }
        internal Cell[,] CellMap { get; set; }
        internal List<Sector> SectorMap { get; set; }
        private List<Sector> _sectorMap;
        internal Cell Me { get; set; }
        internal int MyLife { get; set; }
        internal Sector EnemySector { get; set; }
        internal int OpponentLife { get; set; }
        internal string OpponentOrders { get; set; }
        internal bool PlayerStartsFirst { get; set; }
        internal int TorpedoCharge { get; set; }
    }
}
