﻿using System;
using System.Collections.Generic;

namespace Codingame
{
    class Player
    {
        static void Main(string[] args)
        {
            string[] inputs;
            var game = new Game();
            
            while (true)
            {
                game.Recipes = new List<Recipe>();
                var actionCount = int.Parse(Console.ReadLine()); // the number of spells and recipes in play

                for (int i = 0; i < actionCount; i++)
                {
                    inputs = Console.ReadLine().Split(' ');
                    var actionId = int.Parse(inputs[0]); // the unique ID of this spell or recipe
                    var actionType = inputs[1]; // use this variable from league 2 onwards
                    var delta0 = int.Parse(inputs[2]); // tier-0 ingredient change
                    var delta1 = int.Parse(inputs[3]); // tier-1 ingredient change
                    var delta2 = int.Parse(inputs[4]); // tier-2 ingredient change
                    var delta3 = int.Parse(inputs[5]); // tier-3 ingredient change
                    var price = int.Parse(inputs[6]); // the price in rupees if this is a potion
                    var castable = inputs[9] != "0"; // in the first league: always 0; later: 1 if this is a castable player spell
                    Console.Error.WriteLine($"castable? original: {castable}");

                    var recipe = new Recipe
                    {
                        Id = actionId,
                        Type = actionType,
                        Ingredients = new int[] { delta0, delta1, delta2, delta3 },
                        Price = price,
                        IsCastable = castable
                    };

                    Console.Error.WriteLine(recipe.ToString());
                    game.Recipes.Add(recipe);

                    int tomeIndex = int.Parse(inputs[7]); // in the first two leagues: always 0; later: the index in the tome if this is a tome spell, equal to the read-ahead tax
                    int taxCount = int.Parse(inputs[8]); // in the first two leagues: always 0; later: the amount of taxed tier-0 ingredients you gain from learning this spell
                    bool repeatable = inputs[10] != "0"; // for the first two leagues: always 0; later: 1 if this is a repeatable player spell
                }

                Console.Error.WriteLine($"Number of recipes: {game.Recipes.Count}");

                for (int i = 0; i < 2; i++)
                {
                    var inventory = new List<int>();
                    inputs = Console.ReadLine().Split(' ');
                    var inv0 = int.Parse(inputs[0]);
                    inventory.Add(inv0);
                    var inv1 = int.Parse(inputs[1]);
                    inventory.Add(inv1);
                    var inv2 = int.Parse(inputs[2]);
                    inventory.Add(inv2);
                    var inv3 = int.Parse(inputs[3]);
                    inventory.Add(inv3);

                    game.Witches[i].Inventory = inventory;

                    var score = int.Parse(inputs[4]);
                    game.Witches[i].Score = score;

                    Console.Error.WriteLine($"Witch {i}'s inventory: {inv0}, {inv1}, {inv2}, {inv3}");
                    Console.Error.WriteLine($"Witch {i}'s current score: {score}");
                }

                var output = game.DecideAction();

                // in the first league: BREW <id> | WAIT; later: BREW <id> | CAST <id> [<times>] | LEARN <id> | REST | WAIT
                Console.WriteLine($"{output}");
            }
        }
    }
}
