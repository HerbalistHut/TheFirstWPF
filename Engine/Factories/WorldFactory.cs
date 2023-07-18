using Engine.Models;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Actions;
using Engine.Shared;

namespace Engine.Factories
{
    static internal class WorldFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Locations.xml";

        static internal World CreateWorld()
        {

            var newWorld = new World();

            if (File.Exists(GAME_DATA_FILENAME))
            {
                XmlDocument data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                string rootImagePath = data.SelectSingleNode("/Locations").AttributeAsString("RootImagePath");

                LoadLocations(newWorld, rootImagePath, data.SelectNodes("/Locations/Location"));
            }
            else
            {
                throw new FileNotFoundException($"Missing file:{GAME_DATA_FILENAME}");
            }

            return newWorld;
        }

        private static void LoadLocations(World world, string  rootImagePath, XmlNodeList nodes)
        {
            if (nodes == null)
            {
                return;
            }
        
            foreach (XmlNode node in nodes)
            {
                Location location = new Location(node.AttributeAsInt("X"),
                                                 node.AttributeAsInt("Y"),
                                                 node.AttributeAsString("Name"),
                                                 node.SelectSingleNode("./Description")?.InnerText ?? "",
                                                 $"{rootImagePath}{node.AttributeAsString("ImageName")}");
                AddMonster(location, node.SelectNodes("./Monsters/Monster"));
                AddQuest(location, node.SelectNodes("./Quests/Quest"));
                AddTrader(location, node.SelectSingleNode("./Trader"));

                world.AddLocation(location);
            }
        }

        private static void AddMonster(Location location, XmlNodeList monstersNode)
        {
            if (monstersNode == null)
            {
                return;
            }

            foreach(XmlNode monsterNode in monstersNode)
            {
                location.AddMonster(monsterNode.AttributeAsInt("ID"),
                                    monsterNode.AttributeAsInt("Percent"));
            }
        }

        private static void AddQuest(Location location, XmlNodeList questsNode)
        {
            if (questsNode == null)
            {
                return;
            }

            foreach (XmlNode questNode in questsNode)
            {
                location.QuestsAvailableHere.Add(QuestFactory.GetQuestByID(questNode.AttributeAsInt("ID")));
            }
        }

        private static void AddTrader(Location location, XmlNode traderNode)
        {
            if (traderNode == null)
            {
                return;
            }

            location.TraderHere = TraderFactory.GetTraderByName(traderNode.AttributeAsString("Name"));
        }
    }
}
