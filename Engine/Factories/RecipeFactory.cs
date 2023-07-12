using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Factories
{
    static class RecipeFactory
    {
        private static readonly List<Recipe> _recipes = new List<Recipe>();

        static RecipeFactory()
        {
            Recipe healingPotion = new Recipe(1, "Small Healing Potion");
            healingPotion.AddIngredient(3001, 2);
            healingPotion.AddIngredient(3002, 1);
            healingPotion.AddIngredient(3003, 1);
            healingPotion.AddOutputItems(2001, 2);

            _recipes.Add(healingPotion);
        }

        public static Recipe RecipeById(int id)
        {
            return _recipes.FirstOrDefault(x => x.ID == id);
        }
    }
}
