using Engine.Factories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Monster : LivingEntity
    {
        private readonly List<ItemPercentage> _lootTable = new List<ItemPercentage>();
        public int ID { get; }
        public string ImageName { get; }
        public int RewardExperiencePoints { get; }
        public Monster(int id, string name, string imageName, int maximumHitPoints, GameItem currentWeapon, int rewardExperiencePoints, int rewardGold, int dexterity) :
            base(name, maximumHitPoints, maximumHitPoints, rewardGold, dexterity)
        {
            ID = id;
            ImageName = imageName;
            RewardExperiencePoints = rewardExperiencePoints;
            CurrentWeapon = currentWeapon;
        }

        public void AddItemToLootTable(int id, int percentage)
        {
            //Remove the entrt from the loot table if an entry with this ID already exists
            _lootTable.RemoveAll(x => x.ID == id);

            _lootTable.Add(new ItemPercentage(id, percentage));
        }

        public Monster GetNewMonster()
        {
            //Clone this monster
            Monster newMonster = new Monster(ID, Name, ImageName, MaximumHitPoints, CurrentWeapon, RewardExperiencePoints, Gold, Dexterity);

            foreach (ItemPercentage itemPercentage in _lootTable)
            {
                newMonster.AddItemToLootTable(itemPercentage.ID, itemPercentage.Percentage);

                if (RandomNumberGenerator.NumberBetween(1,100) <= itemPercentage.Percentage)
                {
                    newMonster.AddItemToInventory(ItemFactory.CreateGameItem(itemPercentage.ID));
                }
            }

            return newMonster;
        }
    }
}
