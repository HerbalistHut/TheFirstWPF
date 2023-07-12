using System;
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
        private int _level;
        private GameItem _currentWeapon;
        private GameItem _currentConsumable;
        public string Name
        {
            get => _name;
            private set
            {
                _name = value;
                OnPropertyChanged();
            }
        }
        public int CurrentHitPoints
        {
            get => _currentHitPoints;
            private set
            {
                _currentHitPoints = value;
                OnPropertyChanged();
            }
        }
        public int MaximumHitPoints
        {
            get => _maximumHitPoints;
            protected set
            {
                _maximumHitPoints = value;
                OnPropertyChanged();
            }
        }
        public int Gold
        {
            get => _gold;
            private set
            {
                _gold = value;
                OnPropertyChanged();
            }
        }
        public int Level
        {
            get { return _level; }
            protected set
            {
                _level = value;
                OnPropertyChanged();
            }
        }
        public GameItem CurrentWeapon
        {
            get => _currentWeapon;
            set
            {
                if (_currentWeapon != null)
                {
                    _currentWeapon.Action.OnActionPerformed -= RaisedOnActionPerformedEvent;
                }

                _currentWeapon = value;

                if (_currentWeapon != null)
                {
                    _currentWeapon.Action.OnActionPerformed += RaisedOnActionPerformedEvent;
                }

                OnPropertyChanged();
            }
        }
        public GameItem CurrentConsumable
        {
            get => _currentConsumable;
            set
            {
                if (_currentConsumable != null)
                {
                    _currentConsumable.Action.OnActionPerformed -= OnActionPerformed;
                }
                
                _currentConsumable = value;
                
                if (_currentConsumable != null)
                {
                    _currentConsumable.Action.OnActionPerformed += OnActionPerformed;
                }

                OnPropertyChanged();
            }
        }
        public bool IsDead => CurrentHitPoints <= 0;

        // На данный момент надо сделать ещё несколько вещей, прежде чем убирать свойство Inventory, поэтому сейчас тут существует кое-какое дублирование кода
        public ObservableCollection<GameItem> Inventory { get; }
        public ObservableCollection<GroupedInventoryItem> GroupedInventory { get; }
        
        public List<GameItem> Weapons =>
            Inventory.Where(i => i.Category == GameItem.ItemCategory.Weapon).ToList();
        
        public List<GameItem> Consumables =>
            Inventory.Where(i => i.Category == GameItem.ItemCategory.Consumable).ToList();
        
        public bool HasConsumable => Consumables.Any();

        public event EventHandler OnKilled;
        public event EventHandler<string> OnActionPerformed;
        protected LivingEntity(string name, int currentHitPoints, int maximumHitPoints, int gold, int level = 1)
        {
            Name = name;
            CurrentHitPoints = currentHitPoints;
            MaximumHitPoints = maximumHitPoints;
            Gold = gold;
            Level = level;

            Inventory = new ObservableCollection<GameItem>();
            GroupedInventory = new ObservableCollection<GroupedInventoryItem>();
        }

        public void UseCurrentWeaponOn(LivingEntity target)
        {
            CurrentWeapon.PerformAction(this, target);
        }

        public void UseCurrentConsumable()
        {
            if(CurrentConsumable != null)
            {
                CurrentConsumable.PerformAction(this, this);
                RemoveItemFromInventory(CurrentConsumable);
            }
        }
        public void TakeDamege(int hitPointsOfDamage)
        {
            CurrentHitPoints -= hitPointsOfDamage;
            if (CurrentHitPoints <= 0)
            {
                CurrentHitPoints = 0;
                RaisedOnKillEvent();
            }
        }

        public void Heal(int hitPointsToHeal)
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

        public void AddItemToInventory(GameItem item)
        {
            if (item == null)
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
                // If the item was not in the inventory before this moment, then first of all the item is added to the GroupedInventory in the amount of 0
                if (!GroupedInventory.Any(gi => gi.Item.Id == item.Id))
                {
                    GroupedInventory.Add(new GroupedInventoryItem(item, 0));
                }
                GroupedInventory.First(gi => gi.Item.Id == item.Id).Quantity++;
            }

            OnPropertyChanged(nameof(Weapons));
            OnPropertyChanged(nameof(Consumables));
            OnPropertyChanged(nameof(HasConsumable));
        }

        public void RemoveItemFromInventory(GameItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (Inventory.FirstOrDefault(i => i.Id == item.Id) == null)
            {
                throw new ArgumentException(nameof(item), "This item doesn't exist in Inventory");
            }

            Inventory.Remove(item);

            GroupedInventoryItem groupedInventoryItem = item.IsUnique ?
                GroupedInventory.FirstOrDefault(gi => gi.Item == item) :
                GroupedInventory.FirstOrDefault(gi => gi.Item.Id == item.Id); // Several non-unique item must be removed by thair ID's, otherwise that won't work even if those                                                                    items are 100% identical (why? idk) 

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
            OnPropertyChanged(nameof(Weapons));
            OnPropertyChanged(nameof(Consumables));
            OnPropertyChanged(nameof(HasConsumable));
        }

        public void RemoveItemFromInventory(List<ItemQuantity> itemQuantities)
        {
            foreach (var itemQuantity in itemQuantities)
            {
                for (int i = 0; i < itemQuantity.Quantity; i++)
                {
                    RemoveItemFromInventory(Inventory.FirstOrDefault(item => item.Id == itemQuantity.Id));
                }
            }
        }

        public bool HasAllTheseItems(List<ItemQuantity> items)
        {
            foreach (var item in items)
            {
                if (Inventory.Count(i => i.Id == item.Id) < item.Quantity)
                {
                    return false;
                }
            }
            return true;
        }

        private void RaisedOnKillEvent()
        {
            OnKilled?.Invoke(this, new System.EventArgs());
        }

        private void RaisedOnActionPerformedEvent(object sender, string result)
        {
            OnActionPerformed?.Invoke(this, result);
        }
    }
}
