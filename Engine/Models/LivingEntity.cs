using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Services;

namespace Engine.Models
{
    public abstract class LivingEntity : BaseNotification
    {
        private string _name;
        private int _currentHitPoints;
        private int _maximumHitPoints;
        private int _gold;
        private int _level;
        private int _dexterity;
        private GameItem _currentWeapon;
        private GameItem _currentConsumable;
        private Inventory _inventory;
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
        public int Dexterity
        {
            get => _dexterity;
            private set
            {
                _dexterity = value;
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
        public bool IsAlive => CurrentHitPoints > 0;
        public bool IsDead => !IsAlive;

        // На данный момент надо сделать ещё несколько вещей, прежде чем убирать свойство Inventory, поэтому сейчас тут существует кое-какое дублирование кода
        public Inventory Inventory 
        {
            get => _inventory;
            set
            {
                _inventory = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler OnKilled;
        public event EventHandler<string> OnActionPerformed;
        protected LivingEntity(string name, int currentHitPoints, int maximumHitPoints, int gold, int dexterity, int level = 1)
        {
            Name = name;
            CurrentHitPoints = currentHitPoints;
            MaximumHitPoints = maximumHitPoints;
            Gold = gold;
            Level = level;
            Dexterity = dexterity;

            Inventory = new Inventory();
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
            Inventory = Inventory.AddItem(item);
        }

        public void AddItemToInventory(int id)
        {
            Inventory = Inventory.AddItem(id);
        }

        public void RemoveItemFromInventory(GameItem item)
        {
            Inventory = Inventory.RemoveItem(item);
        }

        public void RemoveItemFromInventory(List<ItemQuantity> itemQuantities)
        {
            Inventory = Inventory.RemoveItem(itemQuantities);
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
