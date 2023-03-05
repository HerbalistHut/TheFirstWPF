﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Weapon : GameItem
    {
        public int MinDMG { get; set; }
        public int MaxDMG { get; set; }
        public Weapon(int id, string name, int price, int minDMG, int maxDMG) 
            : base(id, name, price) 
        {
            MinDMG = minDMG;
            MaxDMG = maxDMG;
        }

        public new Weapon Clone ()
        {
            return new Weapon(this.Id, this.Name, this.Price, MinDMG, MaxDMG);
        }
    }
}