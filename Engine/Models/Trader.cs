using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Trader : LivingEntity
    {
        public int ID { get; }
        public Trader(int id, string name) : base(name, 99999, 99999, 999999,90766898) 
        {
            ID = id;
        } 
    }
}
