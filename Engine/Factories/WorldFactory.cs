﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;

namespace Engine.Factories
{
    static internal class WorldFactory
    {
        static internal World CreateWorld()
        {
            var newWorld = new World();

            newWorld.AddLocation(0, -1, 
                "Home",
                "This is your home",
                "/Engine;component/Images/Locations/Tent.png");

            newWorld.AddLocation(-1, -1, "Farmer's House",
                "This is the house of your neighbor, Farmer Ted.",
                "/Engine;component/Images/Locations/fermer's house.png");

            newWorld.AddLocation(-2, -1, "Farmer's Field",
               "There are rows of corn growing here, with giant rats hiding between them.",
               "/Engine;component/Images/Locations/farm field.png");

            newWorld.AddLocation(-1, 0, "Trading Shop",
               "The shop of Susan, the trader.",
               "/Engine;component/Images/Locations/shop.png");

            newWorld.AddLocation(0, 0, "Town square",
                "You see a fountain here.",
                "/Engine;component/Images/Locations/TownSquare.png");

            newWorld.AddLocation(1, 0, "Town Gate",
                "There is a gate here, protecting the town from giant spiders.",
                "/Engine;component/Images/Locations/Ev0pqzyWgAQa7LN.png");

            newWorld.AddLocation(2, 0, "Spider Forest",
                "The trees in this forest are covered with spider webs.",
                "/Engine;component/Images/Locations/SpiderForest.png");

            newWorld.AddLocation(0, 1, "Herbalist's hut",
                "You see a small hut, with plants drying from the roof.",
                "/Engine;component/Images/Locations/hut.png");

            newWorld.AddLocation(0, 2, "Herbalist's garden",
                "There are many plants here, with snakes hiding behind them.",
                "/Engine;component/Images/Locations/herbarist.png");

            return newWorld;
        }
    }
}