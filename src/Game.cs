﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Codingame
{
    public class Game
    {
        public Witch[] Witches { get; set; }
        public List<Recipe> Recipes { get; set; }
        public Witch Me { get; }
        public Witch Opponent { get; }

        public Game()
        {
            Me = new Witch();
            Opponent = new Witch();
            Witches = new Witch[] { Me, Opponent};
        }

        public void PrintGameState()
        {
            Console.Error.WriteLine($"{Recipes.Count} recipes at start of round:");
            foreach (var recipe in Recipes)
            {
                Console.Error.WriteLine(recipe.ToString());
            }
            Console.Error.WriteLine("Witches at start of round:");
            Console.Error.WriteLine($"Me: {Me}");
            Console.Error.WriteLine($"Opponent: {Opponent}");
        }

        public string DecideAction()
        {
            var brewable = Recipes.Where(r => r.Type == ActionType.BREW && Me.CanCook(r)).ToList();
            if (brewable.Count > 0)
            {
                var maxPrice = brewable.Max(b => b.Income);
                var selectedBrew = brewable?.FirstOrDefault(b => b.Income == maxPrice);

                // if there are brewable spells, brew the first one you can
                if (selectedBrew != null)
                    return $"BREW {selectedBrew.Id}";
            }

            var casts = Recipes.Where(r => r.Type == ActionType.CAST);


            // If any of the CAST spells are castable, AND the castable ones are not cookable REST 
            // OR if my last item is greater or equal to 4 and not all spells are castable
            if ((casts.Any(c => !c.IsCastable) && casts.Where(c => c.IsCastable).All(c => !Me.CanCook(c)))
                || Me.Inventory[3] >= 4 & !casts.All(c => c.IsCastable))
                return "REST";

            var castable = casts.FirstOrDefault(s => Me.CanCook(s) && s.IsCastable);
            return castable != null
                ? $"CAST {castable.Id}"
                : "WAIT";
        }


    }
}
