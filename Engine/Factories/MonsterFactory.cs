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
    static public class MonsterFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Monsters.xml";
        private static readonly List<Monster> _baseMonsters = new List<Monster>();

        static MonsterFactory()
        {
            if (!File.Exists(GAME_DATA_FILENAME))
            {
                 throw new FileNotFoundException($"Missing file : {GAME_DATA_FILENAME}");
            }

            XmlDocument data = new XmlDocument();
            data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

            string rootImagePath = data.SelectSingleNode("/Monsters").AttributeAsString("RootImagePath");

            LoadMonstersFromNodes(data.SelectNodes("/Monsters/Monster"), rootImagePath);
        }

        private static void LoadMonstersFromNodes(XmlNodeList nodes, string rootImagePath)
        {
            foreach (XmlNode node in nodes)
            {
                Monster monster = new Monster(node.AttributeAsInt("ID"),
                                              node.AttributeAsString("Name"),
                                              $"{rootImagePath}{node.AttributeAsString("ImageName")}",
                                              node.AttributeAsInt("MaximumHitPoints"),
                                              ItemFactory.CreateGameItem(node.AttributeAsInt("WeaponID")),
                                              node.AttributeAsInt("RewardXP"),
                                              node.AttributeAsInt("Gold"),
                                              Convert.ToInt32(node.SelectSingleNode("Dexterity").InnerText));

                XmlNodeList lootItemNodes = node.SelectNodes("LootItems/LootItem");
                
                foreach (XmlNode node2 in lootItemNodes)
                {
                    monster.AddItemToLootTable(node2.AttributeAsInt("ID"),
                                               node2.AttributeAsInt("Percentage"));
                }

                _baseMonsters.Add(monster);
            }
        }

        public static Monster GetMonster(int id)
        {
            return _baseMonsters.FirstOrDefault(x => x.ID == id).GetNewMonster();
        }
    }
}
