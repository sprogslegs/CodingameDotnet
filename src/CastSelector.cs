﻿using System.Collections.Generic;
using System.Linq;

namespace Codingame
{
    public class CastSelector
    {
        public Recipe ComputeTargetBrew(IEnumerable<Recipe> brews,
                                        List<int> inventory)
        {
            var deltaMappings = new Dictionary<Recipe, (int, int)>();

            // calculate delta between brews and inventory
            foreach (var brew in brews)
            {
                var delta = new int[4];

                for (int i = 0; i < 4; i++)
                {
                    delta[i] = inventory[i] + brew.Ingredients[i];
                }

                var noOfPositives = delta.Count(value => value >= 0);
                var totalDeficit = delta.Where(value => value < 0).Sum();
                            
                // potential extensions: 
                // or work out sum of all positive ingredients (remainder) and filter on the ones with highest remainder

                deltaMappings.Add(brew, (noOfPositives, totalDeficit));
            }

            // select brews with the most positive values or the smallest deficit
            var highestPositives = int.MinValue;
            var smallestDeficit = int.MinValue;

            foreach (var mapping in deltaMappings)
            {
                var noOfPositives = mapping.Value.Item1;
                highestPositives = noOfPositives > highestPositives
                                    ? noOfPositives
                                    : highestPositives;

                var totalDeficit = mapping.Value.Item2;
                smallestDeficit = totalDeficit > smallestDeficit
                                   ? totalDeficit
                                   : smallestDeficit;
            }

            //var brewShortlist = deltaMappings.Where(m => m.Value.Item1 == highestPositives)
            //                                .Select(m => m.Key).ToList();

            var brewShortlist = deltaMappings.Where(m => m.Value.Item2 == smallestDeficit)
                                            .Select(m => m.Key).ToList();

            var highestPrice = brewShortlist.Max(b => b.Price);

            return brewShortlist.FirstOrDefault(b => b.Price == highestPrice)
                    ?? brewShortlist.FirstOrDefault();
        }

        public Recipe ComputeBestCastForBrew(IEnumerable<Recipe> cookableCasts,
                                             List<int> inventory,
                                             Recipe targetBrew)
        {
            var castMappings = new Dictionary<Recipe, (int, int)>();

            foreach (var cast in cookableCasts)
            {
                var roundEndState = new int[4];
                var brewSimulation = new int[4];

                for (int i = 0; i < 4; i++)
                {
                    // calculate delta between each cast and inventory- round end state
                    roundEndState[i] = inventory[i] + cast.Ingredients[i];

                    // calculate delta between each end state and targetbrew - brewSimulation
                    brewSimulation[i] = roundEndState[i] + targetBrew.Ingredients[i];
                }

                var numberOfZeros = brewSimulation.Count(item => item == 0);
                var deficit = brewSimulation.Where(s => s < 0).Sum();
                castMappings.Add(cast, (deficit, numberOfZeros));
            }

            int leastZeros = int.MaxValue;
            int smallestDeficit = int.MinValue;

            // return the cast associated with the simulation with lowest deficit
            // select cast associated with the brew simulation with the lowest number of zeros
            foreach (var mapping in castMappings)
            {
                var deficit = mapping.Value.Item1;
                var numberOfZeros = mapping.Value.Item2;

                smallestDeficit = deficit > smallestDeficit ? deficit : smallestDeficit;
                leastZeros = numberOfZeros < leastZeros ? numberOfZeros : leastZeros;
            }

            var deficitWinner = castMappings.FirstOrDefault(m => m.Value.Item1 == smallestDeficit).Key;
            var zerosWinner = castMappings.FirstOrDefault(m => m.Value.Item2 == leastZeros).Key;

            return zerosWinner
                ?? deficitWinner;
        }
    }
}
