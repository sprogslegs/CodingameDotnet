﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Codingame
{
    internal class PlayerActions
    {
        internal List<string> Actions { get; set; }
        internal string PreviousDirection { get; set; }
        private readonly List<Cell> _freeNeighbours;
        private GameState _gameState;

        internal PlayerActions(GameState gameState)
        {
            _gameState = gameState;
            Actions = new List<string>();
            _freeNeighbours = new List<Cell>();
        }

        internal void SetStartingPosition()
        {
            var corners = GetMapCorners();

            var selected = corners.Where(c => c.IsFree()).FirstOrDefault() 
                ?? GetFirstFreeCell();

            selected.Visited = true;

            Actions.Add($"{selected.ColX} {selected.RowY}");
        }

        internal void Act()
        {
            _gameState.Me.Visited = true;

            FindFreeNeighbours();

            if (_freeNeighbours.Count > 0)
                Move();
            else Surface();

            // if torpedo charge == 3 Torpedo
        }

        private List<Cell> GetMapCorners()
        {
            var lastCol = _gameState.MapWidth - 1;
            var lastRow = _gameState.MapHeight - 1;

            return new List<Cell>
            {
                _gameState.CellMap[0, 0],
                _gameState.CellMap[0, lastCol],
                _gameState.CellMap[lastRow, 0],
                _gameState.CellMap[lastRow, lastCol]
            };
        }

        private Cell GetFirstFreeCell()
        {
            var width = _gameState.CellMap.GetLength(0);
            var height = _gameState.CellMap.GetLength(1);
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var cell = _gameState.CellMap[i, j];
                    if (cell.IsFree())
                        return _gameState.CellMap[i, j];
                }
            }

            return null;
        }

        private void FindFreeNeighbours()
        {
            _freeNeighbours.Clear();

            var x = _gameState.Me.ColX;
            var y = _gameState.Me.RowY;

            var north = y > 0
                ? _gameState.CellMap[y - 1, x]
                : null;
            if (north != null && north.IsFree()) _freeNeighbours.Add(north);

            var south = y < (_gameState.MapHeight - 1)
                ? _gameState.CellMap[y + 1, x]
                : null;
            if (south != null && south.IsFree()) _freeNeighbours.Add(south);

            var east = x < (_gameState.MapWidth - 1)
                ? _gameState.CellMap[y, x + 1]
                : null;
            if (east != null && east.IsFree()) _freeNeighbours.Add(east);

            var west = x > 0
                ? _gameState.CellMap[y, x - 1]
                : null;
            if (west != null && west.IsFree()) _freeNeighbours.Add(west);
        }

        private void Move()
        {
            var nextCellInPath = PreviousDirection != null ? FindNextCell(PreviousDirection) : null;

            // if I can move in current path, do so
            if (PreviousDirection != null && _freeNeighbours.Contains(nextCellInPath))
            {
                Actions.Add($"MOVE {PreviousDirection} TORPEDO");
            }

            // if only one free neighbour or can't move in current path or haven't moved before, move to first free neighbour
            else
            {
                PreviousDirection = GetRelativeDirection(_freeNeighbours[0]);
                Actions.Add($"MOVE {PreviousDirection} TORPEDO");
            }
        }

        // handle cells not on the map
        private Cell FindNextCell(string direction)
        {
            var myRow = _gameState.Me.RowY;
            var myColumn = _gameState.Me.ColX;
            var height = _gameState.MapHeight;
            var width = _gameState.MapWidth;

            if (direction.Equals("W") && myColumn > 0)
            {
                return _gameState.CellMap[myRow, myColumn - 1];
            }
            
            if (direction.Equals("E") && myColumn < height - 1)
            {
                return _gameState.CellMap[myRow, myColumn + 1];
            }
            
            if (direction.Equals("S") && myRow < width - 1)
            {
                return _gameState.CellMap[myRow + 1, myColumn];
            }
            
            if (direction.Equals("N") && myRow > 0)
            {
                return _gameState.CellMap[myRow - 1, myColumn];
            }

            else
                return null;
        }

        private string GetRelativeDirection(Cell neighbour)
        {
            return neighbour.RowY < _gameState.Me.RowY
                ? "N"
                : neighbour.RowY > _gameState.Me.RowY
                    ? "S"
                    : neighbour.ColX > _gameState.Me.ColX
                        ? "E"
                        : neighbour.ColX < _gameState.Me.ColX
                            ? "W"
                            : null;
        }

        private void Surface()
        {
            Actions.Add("SURFACE");

            foreach (var cell in _gameState.CellMap)
            {
                cell.Visited = false;
            }
        }

        private void Torpedo()
        {
            // Check for enemy position
            // Compile torpedo action
            // add to Actions
            // re-set charges
        }
    }
}