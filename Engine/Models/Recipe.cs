﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Recipe
    {
        public int ID { get; }
        public string Name { get; }
        public List<ItemQuantity> Ingredients { get; } = new List<ItemQuantity>();
        public List<ItemQuantity> OutputItems { get; } = new List<ItemQuantity>();

        public string ToolTipContents =>
            "To craft this:" + Environment.NewLine +
            "----------------------------------------------------" + Environment.NewLine +
            string.Join(Environment.NewLine, OutputItems.Select(i => i.QuantityItemDescription)) +
            Environment.NewLine + Environment.NewLine +
            "You need:" + Environment.NewLine +
            "----------------------------------------------------" + Environment.NewLine +
            string.Join(Environment.NewLine, Ingredients.Select(i => i.QuantityItemDescription));
        public Recipe(int id, string name)
        {
            ID = id;
            Name = name;
            name = OutputItems.Count() > 5 ? "asd" : "dsa";
        }

        public void AddIngredient (int id, int quantity)
        {
            if (!Ingredients.Any(x=> x.Id == id))
            {
                Ingredients.Add(new ItemQuantity(id, quantity));
            }
        }

        public void AddOutputItems(int id, int quantity)
        {
            if(!OutputItems.Any(x=> x.Id == id))
            {
                OutputItems.Add(new ItemQuantity (id, quantity));
            }
        }
    }
}
