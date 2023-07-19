﻿using Engine.Models;
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
    internal static class QuestFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\Quests.xml";
        public static readonly List<Quest> _quests = new List<Quest>();

        static QuestFactory()
        {
            if (!File.Exists(GAME_DATA_FILENAME))
            {
                throw new FileNotFoundException($"Missing file:{GAME_DATA_FILENAME}");
            }

            XmlDocument data = new XmlDocument();
            data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

            LoadQuestsFromNodes(data.SelectNodes("/Quests/Quest"));
        }

        private static void LoadQuestsFromNodes(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                List<ItemQuantity> itemsToComplete = new List<ItemQuantity>();
                List<ItemQuantity> rewardItems = new List<ItemQuantity>();

                foreach (XmlNode childNode in node.SelectNodes("./ItemsToComplete/Item"))
                {
                    itemsToComplete.Add(new ItemQuantity(childNode.AttributeAsInt("ID"), childNode.AttributeAsInt("Quantity")));
                }

                foreach (XmlNode childNode in node.SelectNodes("./RewardItems/Item"))
                {
                    rewardItems.Add(new ItemQuantity(childNode.AttributeAsInt("ID"), childNode.AttributeAsInt("Quantity")));
                }

                Quest quest = new Quest(node.AttributeAsInt("ID"),
                                        node.SelectSingleNode("Name").InnerText,
                                        node.SelectSingleNode("Description").InnerText,
                                        itemsToComplete,
                                        node.AttributeAsInt("RewardXP"),
                                        node.AttributeAsInt("RewardGold"),
                                        rewardItems);
                _quests.Add(quest);
            }
        }

        internal static Quest GetQuestByID(int id)
        {
            return _quests.FirstOrDefault(quest => quest.Id == id);
        }
    }
}
