﻿using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Actions;

namespace Engine.Factories
{
    public static class ItemFactory
    {
        public static readonly List<GameItem> _standardGameItems = new List<GameItem>();

        static ItemFactory()
        {
            BuildWeaponItem(0, "Cheat", 0, 1000, 1000);
            BuildWeaponItem(1001, "Pointy Stick", 1, 1, 2);
            BuildWeaponItem(1002, "Rusty Sword", 5, 5, 12);
            BuildMiscellaneousItem(9001, "Snake fang", 1);
            BuildMiscellaneousItem(9002, "Snakeskin", 2);
            BuildMiscellaneousItem(9003, "Rat tail", 1);
            BuildMiscellaneousItem(9004, "Rat fur", 2);
            BuildMiscellaneousItem(9005, "Spider fang", 1);
            BuildMiscellaneousItem(9006, "Spider silk", 2);
        }

        public static GameItem CreateGameItem (int id)
        {
            return _standardGameItems.FirstOrDefault(item => item.Id == id)?.Clone();
        }
        private static void BuildMiscellaneousItem(int id, string name, int price)
        {
            _standardGameItems.Add(new GameItem(GameItem.ItemCategory.Miscellaneous, id, name, price));
        }
        private static void BuildWeaponItem(int id, string name, int price, int minimumDamage, int maximumDamage) 
        {
            GameItem weapon = new GameItem(GameItem.ItemCategory.Weapon, id, name, price, true);

            weapon.Action = new AttacWithWeapon(weapon, minimumDamage, maximumDamage);

            _standardGameItems.Add(weapon);
        }
    }
}
