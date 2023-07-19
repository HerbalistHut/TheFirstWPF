using Engine.Models;
using Engine.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Engine.Factories
{
    static class RecipeFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Recipes.xml";
        private static readonly List<Recipe> _recipes = new List<Recipe>();

        static RecipeFactory()
        {
            if (!File.Exists(GAME_DATA_FILENAME))
            {
                throw new FileNotFoundException($"Missing file:{GAME_DATA_FILENAME}");
            }

            XmlDocument data = new XmlDocument();
            data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

            LoadRecipesFromNodes(data.SelectNodes("/Recipes/Recipe"));
        }

        private static void LoadRecipesFromNodes(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                Recipe newRecipe = new Recipe(node.AttributeAsInt("ID"), node.SelectSingleNode("Name").InnerText);
            
                foreach (XmlNode childNode in node.SelectNodes("Ingredients/Item"))
                {
                    newRecipe.AddIngredient(childNode.AttributeAsInt("ID"), childNode.AttributeAsInt("Quantity"));
                }

                foreach (XmlNode childNode in node.SelectNodes("OutputItems/Item"))
                {
                    newRecipe.AddOutputItems(childNode.AttributeAsInt("ID"), childNode.AttributeAsInt("Quantity"));
                }
                _recipes.Add(newRecipe);
            }
        }
        public static Recipe RecipeById(int id)
        {
            return _recipes.FirstOrDefault(x => x.ID == id);
        }
    }
}
