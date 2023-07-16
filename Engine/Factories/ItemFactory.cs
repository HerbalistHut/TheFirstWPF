using Engine.Models;
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Actions;

namespace Engine.Factories
{
    public static class ItemFactory
    {
        private const string GAME_DATA_FILENAME = ".\\GameData\\GameItems.xml";
        public static readonly List<GameItem> _standardGameItems = new List<GameItem>();

        static ItemFactory()
        {
            if (File.Exists(GAME_DATA_FILENAME))
            {
                XmlDocument data = new XmlDocument();
                data.LoadXml(File.ReadAllText(GAME_DATA_FILENAME));

                LoadItemsFromNodes(data.SelectNodes("/GameItems/Weapons/Weapon"));
                LoadItemsFromNodes(data.SelectNodes("/GameItems/HealingItems/HealingItem"));
                LoadItemsFromNodes(data.SelectNodes("/GameItems/MiscellaneousItems/MiscellaneousItem"));
            }
            else
            {
                throw new FileNotFoundException($"Missing file:{GAME_DATA_FILENAME}");
            }
        }

        static void LoadItemsFromNodes(XmlNodeList nodes)
        {
            if (nodes == null)
            {
                return;
            }

            foreach (XmlNode node in nodes)
            {
                GameItem.ItemCategory category = DetermineItemCategory(node.Name);

                GameItem gameItem = new GameItem(
                    category,
                    GetXmlAttributeAsInt(node, "ID"),
                    GetXmlAttribute(node, "Name"),
                    GetXmlAttributeAsInt(node, "Price"),
                    category == GameItem.ItemCategory.Weapon);

                switch (category)
                {
                    case GameItem.ItemCategory.Weapon:
                        gameItem.Action = new AttacWithWeapon(gameItem,
                                                              GetXmlAttributeAsInt(node, "MinimumDamage"),
                                                              GetXmlAttributeAsInt(node, "MaximumDamage"));
                        break;
                    case GameItem.ItemCategory.Consumable:
                        gameItem.Action = new Heal(gameItem,
                                                   GetXmlAttributeAsInt(node, "HitPointsToHeal"));
                        break;
                    case GameItem.ItemCategory.Miscellaneous:
                        break;
                    default:
                        throw new ArgumentException($"Item category {category} not implemented in ItemFactory");
                }
                _standardGameItems.Add(gameItem);
            }
        }

        public static GameItem CreateGameItem(int id)
        {
            return _standardGameItems.FirstOrDefault(item => item.Id == id)?.Clone();
        }

        public static string ItemName(int itemID)
        {
            return _standardGameItems.FirstOrDefault(i => i.Id == itemID)?.Name ?? "NULL";
        }

        private static GameItem.ItemCategory DetermineItemCategory(string itemCategory)
        {
            switch (itemCategory)
            {
                case "Weapon":
                    return GameItem.ItemCategory.Weapon;
                case "HealingItem":
                    return GameItem.ItemCategory.Consumable;
                default:
                    return GameItem.ItemCategory.Miscellaneous;
            }
        }

        private static int GetXmlAttributeAsInt(XmlNode node, string attributeName)
        {
            return Convert.ToInt32(GetXmlAttribute(node, attributeName));
        }

        private static string GetXmlAttribute(XmlNode node, string attributeName)
        {
            XmlAttribute attribute = node.Attributes?[attributeName];

            if (attribute == null)
            {
                throw new Exception($"Attrebute {attributeName} doesn't exist");
            }

            return attribute.Value;
        }
    }
}
