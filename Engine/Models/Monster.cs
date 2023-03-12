using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Monster : BaseNotification
    {
        private int _hitPoints;
        public string Name { get; set; }
        public string ImageName { get; set; }
        public int MaximumHitPoints { get; set; }
        public int HitPoints {
            get => _hitPoints;
            set
            {
                _hitPoints = value;
                OnPropertyChanged(nameof(HitPoints));
            }
        }

        public int RewardExperiencePoints { get; set; }
        public int RewardGold { get; set; }
        public int MinimumDamage { get; set; }
        public int MaximumDamage { get; set; }

        public ObservableCollection<ItemQuantity> Inventory { get; set; }

        public Monster(string name, string imageName, int maximumHitPoints, int hitPoints,int rewardExperiencePoints, int rewardGold, int minimumDamage, int maximumDamage)
        {
            Name = name;
            ImageName = string.Format($"/Engine;component/Images/Monsters/{imageName}") ;
            MaximumHitPoints = maximumHitPoints;
            HitPoints = hitPoints;
            RewardExperiencePoints = rewardExperiencePoints;
            RewardGold = rewardGold;
            MinimumDamage = minimumDamage;
            MaximumDamage = maximumDamage;
            Inventory = new ObservableCollection<ItemQuantity>();
        }
    }
}
