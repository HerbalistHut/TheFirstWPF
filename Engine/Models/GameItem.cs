﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Actions;

namespace Engine.Models
{
    public class GameItem
    {
        public enum ItemCategory
        {
            Miscellaneous,
            Weapon,
            Consumable
        }
        public ItemCategory Category { get; }
        public int Id { get; }
        public string Name { get; }
        public int Price { get; }
        public bool IsUnique { get; }
        public IAction Action { get; set; }

        public GameItem(ItemCategory category,int id, string name, int price, bool isUnique = false, IAction action = null) 
        {
            Category = category;
            Id = id;
            Name = name;
            Price = price;
            IsUnique = isUnique; 
            Action = action;   
        }
        public void PerformAction(LivingEntity actor, LivingEntity target)
        {
            Action?.Execute(actor, target);
        }
        public GameItem Clone() 
        {
            return new GameItem(Category, Id, Name, Price, IsUnique, Action);
        }


    }
}
