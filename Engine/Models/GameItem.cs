using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class GameItem
    {
        public int Id { get; }
        public string Name { get; }
        public int Price { get; }
        public bool IsUnique { get; }

        public GameItem(int id, string name, int price, bool isUnique = false) 
        {
            Id = id;
            Name = name;
            Price = price;
            IsUnique = isUnique; 
        }
        
        public GameItem Clone() 
        {
            return new GameItem(Id, Name, Price);
        }


    }
}
