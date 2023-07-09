using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class GameItem
    {
        public enum ItemCategory
        {
            Miscellaneous,
            Weapon
        }
        public ItemCategory Category { get; }
        public int Id { get; }
        public string Name { get; }
        public int Price { get; }
        public bool IsUnique { get; }
        public int MinimumDamage { get; set; }
        public int MaximumDamage { get; set; }

        public GameItem(ItemCategory category,int id, string name, int price, bool isUnique = false, int minimumDamage = 0, int maximumDamage = 0) 
        {
            Category = category;
            Id = id;
            Name = name;
            Price = price;
            IsUnique = isUnique; 
            MinimumDamage = minimumDamage;
            MaximumDamage = maximumDamage;
        }
        
        public GameItem Clone() 
        {
            return new GameItem(Category, Id, Name, Price, IsUnique, MinimumDamage, MaximumDamage);
        }


    }
}
