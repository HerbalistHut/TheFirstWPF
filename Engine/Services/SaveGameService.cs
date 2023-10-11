using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Engine.ViewModels;
using Engine.Models;
using Engine.Factories;
using System.Net.Http.Headers;

namespace Engine.Services
{
    public static class SaveGameService
    {
        private const string SAVE_GAME_FILE_NAME = "LastSession.json";

        public static void Save(GameSession session)
        {
            File.WriteAllText(SAVE_GAME_FILE_NAME, JsonConvert.SerializeObject(session, Formatting.Indented));
        }

        public static GameSession LoadLastSaveOrCreateNew()
        {
            if (!File.Exists(SAVE_GAME_FILE_NAME))
            {
                return new GameSession();
            }

            // If saved game file exists, create GameSession from it
            try
            {
                JObject data = JObject.Parse(File.ReadAllText(SAVE_GAME_FILE_NAME));

                Player player = CreatePlayer(data);

                int x = (int)data[nameof(GameSession.CurrentLocation)][nameof(Location.XCoordinate)];
                int y = (int)data[nameof(GameSession.CurrentLocation)][nameof(Location.YCoordinate)];

                return new GameSession(player, x, y);
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Failed to load saved game", ex);
            }
        }

        private static Player CreatePlayer(JObject data)
        {
            string fileVersion = (string)data[nameof(GameSession.Version)];

            Player player;

            switch (fileVersion)
            {
                case "0.1.000":
                    player =
                        new Player((string)data[nameof(GameSession.CurrentPlayer)][nameof(Player.Name)],
                                   (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.CurrentHitPoints)],
                                   (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.MaximumHitPoints)],
                                   (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.Gold)],
                                   (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.ExperiencePoints)],
                                   (int)data[nameof(GameSession.CurrentPlayer)][nameof(Player.Dexterity)]);
                    break;
                default:
                    throw new InvalidDataException($"File version {fileVersion} not supported");
            }

            PopulatePlayerInventory(data, player);

            PopulatePlayerQuests(data, player);

            PopulatePlayerRecipes(data, player);
        
            return player;
        }

        private static void PopulatePlayerInventory(JObject data, Player player)
        {
            string fileVersion = (string)data[nameof(GameSession.Version)];

            switch(fileVersion)
            {
                case "0.1.000":
                    foreach(JToken itemToken in (JArray)data[nameof(GameSession.CurrentPlayer)]
                                                            [nameof(Player.Inventory)]
                                                            [nameof(Inventory.Items)])
                        {
                        int itemId = (int)itemToken[nameof(GameItem.Id)];

                        player.AddItemToInventory(ItemFactory.CreateGameItem(itemId));
                        }
                    break;
                
                default:
                    throw new InvalidDataException($"File version {fileVersion} not supported");
            }
        }

        private static void PopulatePlayerQuests(JObject data, Player player)
        {
            string fileVersion = (string)data[nameof(GameSession.Version)];

            switch (fileVersion)
            {
                case "0.1.000":
                    foreach (JToken questToken in (JArray)data[nameof(GameSession.CurrentPlayer)]
                                                             [nameof(Player.Quests)])
                    {
                        int questId = (int)questToken[nameof(QuestStatus.PlayerQuest)][nameof(QuestStatus.PlayerQuest.Id)];

                        Quest quest = QuestFactory.GetQuestByID(questId);
                        QuestStatus questStatus = new QuestStatus(quest);
                        questStatus.IsCompleted = (bool)questToken[nameof(QuestStatus.IsCompleted)];
                        player.Quests.Add(questStatus);
                    }
                    break;
                default:
                    throw new InvalidDataException($"File version {fileVersion} not supported");
            }
        }

        private static void PopulatePlayerRecipes(JObject data, Player player)
        {
            string fileVersion = (string)data[nameof(GameSession.Version)];

            switch(fileVersion)
            {
                case "0.1.000":
                    foreach (JToken recipeToken in (JArray)data[nameof(GameSession.CurrentPlayer)]
                                                                [nameof(Player.Recipes)])
                        {
                            int recipeId = (int)recipeToken[nameof(Recipe.ID)];
                            
                            Recipe recipe = RecipeFactory.RecipeById(recipeId);
                            player.Recipes.Add(recipe);
                        }
                    break;
                default:
                    throw new InvalidDataException($"File version {fileVersion} not supported");
            }
        }
    }
}
