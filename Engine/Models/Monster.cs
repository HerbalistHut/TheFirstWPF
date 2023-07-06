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
        public string ImageName { get; }
        public int RewardExperiencePoints { get; }
        public int MinimumDamage { get; }
        public int MaximumDamage { get; }
        public Monster(string name, string imageName, int maximumHitPoints, int currentHitPoints,int rewardExperiencePoints, int rewardGold, int minimumDamage, int maximumDamage):
            base(name, currentHitPoints, maximumHitPoints, rewardGold)
        {
            ImageName = $"/Engine;component/Images/Monsters/{imageName}";
            RewardExperiencePoints = rewardExperiencePoints;
            MinimumDamage = minimumDamage;
            MaximumDamage = maximumDamage;
        }
    }
}
