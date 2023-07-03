﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public abstract class LivingEntity : BaseNotification
    {
        private string _name;
        private int _currentHitPoints;
        private int _maximumHitPoints;
        private int _gold;

        public string Name
        {
            get => _name;
            private set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public int CurrentHitPoints
        {
            get => _currentHitPoints;
            private set
            {
                _currentHitPoints = value;
                OnPropertyChanged(nameof(CurrentHitPoints));
            }
        }
        public int MaximumHitPoints
        {
            get => _maximumHitPoints;
            private set
            {
                _maximumHitPoints = value;
                OnPropertyChanged(nameof(MaximumHitPoints));
            }
        }
        public int Gold
        {
            get => _gold;
            private set
            {
                _gold = value;
                OnPropertyChanged(nameof(Gold));
            }
        }
        public bool IsDead => CurrentHitPoints <= 0;

        // На данный момент надо сделать ещё несколько вещей, прежде чем убирать свойство Inventory, поэтому сейчас тут существует кое-какое дублирование кода
        public ObservableCollection<GameItem> Inventory { get; set; }
        public ObservableCollection<GroupedInventoryItem> GroupedInventory { get; set; }
        public List<GameItem> Weapons =>
            Inventory.Where(i => i is Weapon).ToList();

        
        public event EventHandler OnKilled;
        protected LivingEntity(string name, int currentHitPoints, int maximumHitPoints, int gold)
        {
            Name = name;
            CurrentHitPoints = currentHitPoints;
            MaximumHitPoints = maximumHitPoints;
            Gold = gold;

            Inventory = new ObservableCollection<GameItem>();
            GroupedInventory = new ObservableCollection<GroupedInventoryItem>();
        }

        public void TakeDamege (int hitPointsOfDamage)
        {
            CurrentHitPoints -= hitPointsOfDamage;
            if (CurrentHitPoints < 0)
            {
                CurrentHitPoints = 0;
                RaisedOnKillEvent();
            }
        }

        public void Heal (int hitPointsToHeal)
        {
            CurrentHitPoints += hitPointsToHeal;
            if (CurrentHitPoints > MaximumHitPoints)
            {
                CurrentHitPoints = MaximumHitPoints; 
            }
        }

        public void CompletelyHeal()
        {
            CurrentHitPoints = MaximumHitPoints;
        }

        public void ReceiveGold(int gold)
        {
            Gold += gold;
        }

        public void SpendGold(int amountOfGold)
        {
            if (amountOfGold > Gold)
            {
                throw new ArgumentOutOfRangeException($"{Name} only has {Gold} gold, you tried to spend {amountOfGold} gold");
            }
            Gold -= amountOfGold;
        }
        //Здесь ОРС эвент вызывается для Weapons, чтобы UI при добавлении предмета в инвентарь обновлял и интерфейс с оружием. 
        //OPC вызывается только для Weapons, так как Inventory и так сам обновляет свой UI при изменение в своей коллексии из-за ObservableCollection
        public void AddItemToInventory(GameItem item)
        {
            if(item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Inventory.Add(item);

            if (item.IsUnique)
            {
                GroupedInventory.Add(new GroupedInventoryItem(item, 1));
            }
            else
            {
                // Если предмета не было в инвентаре до этого момента,то в сгруппированный инвентарь сначало добавляется предмет в количестве 0 штук,
                // это сделанлано потому, что прибавление количества(Quantity) предмета выполнится в любом случае
                if (!GroupedInventory.Any(gi=>gi.Item.Id == item.Id))
                {
                    GroupedInventory.Add(new GroupedInventoryItem(item, 0));
                }
                GroupedInventory.First(gi => gi.Item.Id == item.Id).Quantity++;
            }

            OnPropertyChanged(nameof(Weapons));
        }

        public void RemoveItemFromInventory(GameItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (Inventory.FirstOrDefault(i=>i.Id == item.Id) == null)
            {
                throw new ArgumentException(nameof(item),"This item doesn't exist in Inventory");
            }

            Inventory.Remove(item);

            GroupedInventoryItem groupedInventoryItem = GroupedInventory.FirstOrDefault(gi => gi.Item == item);

            if (groupedInventoryItem == null)
            {
                throw new ArgumentException($"{nameof(item)} doesn't exist in GroupedInventory");
            }

            if (groupedInventoryItem.Quantity == 1)
            {
                GroupedInventory.Remove(groupedInventoryItem);
            }
            else
            {
                groupedInventoryItem.Quantity--;
            }
        }

        private void RaisedOnKillEvent()
        {
            OnKilled?.Invoke(this, new System.EventArgs());
        }
    }
}
