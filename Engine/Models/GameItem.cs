using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class GameItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }

        public GameItem(int id, string name, int price) 
        {
            Id = id;
            Name = name;
            Price = price;
        }
        
        public GameItem Clone() 
        {
            return new GameItem(Id, Name, Price);
        }


    }
}
