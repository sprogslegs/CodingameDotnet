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
            Cell selected = null;
            var corners = GetFreeCorners().ToList();

            if (corners.Count == 0)
            {
                selected = GetFirstFreeCell();
            }
            if (corners.Count == 1)
            {
                selected = corners[0];
            }
            if (corners.Count > 1)
            {
                corners.ForEach(c => LoadNavigationParams(c));

                selected = corners.OrderByDescending(c => c.Score).Take(1).First();
            }

            selected.Visited = selected != null;

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

        private IEnumerable<Cell> GetFreeCorners()
        {
            var lastCol = _gameState.MapWidth - 1;
            var lastRow = _gameState.MapHeight - 1;

            var corners = new List<Cell>
            {
                _gameState.CellMap[0, 0],
                _gameState.CellMap[0, lastCol],
                _gameState.CellMap[lastRow, 0],
                _gameState.CellMap[lastRow, lastCol]
            };

            return corners.Where(c => c.IsFree());
        }

        private Cell GetFirstFreeCell()
        {
            var width = _gameState.CellMap.GetLength(0);
            var height = _gameState.CellMap.GetLength(1);
            var freeCells = new List<Cell>();
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var cell = _gameState.CellMap[i, j];
                    if (cell.IsFree())
                    {
                        freeCells.Add(cell);
                        LoadNavigationParams(cell);
                    }
                }
            }

            return freeCells.OrderByDescending(c => c.Score).FirstOrDefault();
        }

        private void FindFreeNeighbours()
        {
            _freeNeighbours.Clear();

            var x = _gameState.Me.ColX;
            var y = _gameState.Me.RowY;

            // North
            if (y > 0 && _gameState.CellMap[y - 1, x].IsFree())
                _freeNeighbours.Add(_gameState.CellMap[y - 1, x]);

            // South
            if (y < (_gameState.MapHeight - 1) && _gameState.CellMap[y + 1, x].IsFree())
                _freeNeighbours.Add(_gameState.CellMap[y + 1, x]);

            // East
            if (x < (_gameState.MapWidth - 1) && _gameState.CellMap[y, x + 1].IsFree())
                _freeNeighbours.Add(_gameState.CellMap[y, x + 1]);

            // West
            if (x > 0 && _gameState.CellMap[y, x - 1].IsFree())
                _freeNeighbours.Add(_gameState.CellMap[y, x - 1]);
        }

        private void LoadNavigationParams(Cell cell)
        {
            var populated = new HashSet<Cell>();
            cell.Score = CalculateNavigationParams(cell, populated);
            populated.Clear();
        }

        private int CalculateNavigationParams(Cell cell, HashSet<Cell> populated)
        {
            var tempScore = 0;

            if (!populated.Contains(cell) && cell.Score == 0)
            {
                FindFreeNeighbours(cell);
                populated.Add(cell);

                foreach (var neighbour in cell.FreeNeighbours)
                {
                    CalculateNavigationParams(neighbour, populated);
                    tempScore += neighbour.Score;
                }

                cell.Score += cell.FreeNeighbours.Count;
            }

            else tempScore = cell.Score;

            return tempScore;
        }

        private void FindFreeNeighbours(Cell cell)
        {
            var x = cell.ColX;
            var y = cell.RowY;

            // North
            if (y > 0 && _gameState.CellMap[y - 1, x].IsFree())
                cell.FreeNeighbours.Add(_gameState.CellMap[y - 1, x]);

            // South
            if (y < (_gameState.MapHeight - 1) && _gameState.CellMap[y + 1, x].IsFree())
                cell.FreeNeighbours.Add(_gameState.CellMap[y + 1, x]);

            // East
            if (x < (_gameState.MapWidth - 1) && _gameState.CellMap[y, x + 1].IsFree())
                cell.FreeNeighbours.Add(_gameState.CellMap[y, x + 1]);

            // West
            if (x > 0 && _gameState.CellMap[y, x - 1].IsFree())
                cell.FreeNeighbours.Add(_gameState.CellMap[y, x - 1]);
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