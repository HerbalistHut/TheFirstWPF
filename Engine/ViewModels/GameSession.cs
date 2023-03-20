using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Engine.Factories;
using System.ComponentModel;
using Engine.EventArgs;
using System.Security.Permissions;

namespace Engine.ViewModels
{
    public class GameSession : BaseNotification
    {
        public event EventHandler<GameMessageEventArgs> OnMessageRaised;
        private Monster _currentMonster;
        private Location _currentLocation;
        private Trader _currentTrader;
        public World CurrentWorld { get; set; }
        public Player CurrentPlayer { get; set; }
        public Location CurrentLocation {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                OnPropertyChanged(nameof(CurrentLocation));
                OnPropertyChanged(nameof(HasLocationToEast));
                OnPropertyChanged(nameof(HasLocationToWest));
                OnPropertyChanged(nameof(HasLocationToSouth));
                OnPropertyChanged(nameof(HasLocationToNorth));

                CompleteQuestAtLocation();
                GivePlayerQuestAtLocation();
                GetMonsterAtLocation();

                CurrentTrader = CurrentLocation.TraderHere;
            } 
        }   
        public Monster CurrentMonster
        {
            get => _currentMonster;
            set
            {
                _currentMonster = value;
                OnPropertyChanged(nameof(CurrentMonster));
                OnPropertyChanged(nameof(HasMonster));

                if (_currentMonster != null)
                {
                    RaiseMessage("");
                    RaiseMessage($"You see {CurrentMonster.Name} here!");
                }
            }
        }
        public Weapon CurrentWeapon { get; set; } 
        public Trader CurrentTrader
        {
            get => _currentTrader;
            set
            {
                _currentTrader = value;

                OnPropertyChanged(nameof(CurrentTrader));
                OnPropertyChanged(nameof(HasTrader));
            }
        }
        public GameSession()
        {
            CurrentPlayer = new Player { Name = "Nikita", CharacterClass = "This method is not inp", ExperiencePoints = 0, Gold = 100, HitPoints = 10, Level = 1};
            
            if (!CurrentPlayer.Weapons.Any())
            {
                CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(1001));
            }

            CurrentWorld = WorldFactory.CreateWorld();

            CurrentLocation = CurrentWorld.LocationAt(0, -1);

        }

        public bool HasLocationToNorth 
            => (CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1) != null);
        public bool HasLocationToSouth 
            => (CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1) != null);
        public bool HasLocationToWest
            => (CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate) != null);
        public bool HasLocationToEast
            => (CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate) != null);
        public void MoveNorth()
            => CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1);
        public void MoveSouth() 
            => CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1);
        public void MoveWest() 
            => CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate);
        public void MoveEast() 
            => CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate);
        public bool HasMonster => CurrentMonster != null;
        public bool HasTrader => CurrentTrader != null;
        private void GivePlayerQuestAtLocation()
        {
            foreach(Quest quest in CurrentLocation.QuestsAvailableHere)
            {
                if (!CurrentPlayer.Quests.Any(q => q.PlayerQuest.Id == quest.Id))
                {
                    CurrentPlayer.Quests.Add(new QuestStatus(quest));

                    RaiseMessage("");
                    RaiseMessage($"You recieve the '{quest.Name}' quest");
                    RaiseMessage(quest.Description);
                    RaiseMessage("Return with:");
                    foreach (var i in quest.ItemsToComplete)
                    {
                        RaiseMessage($"    {i.Quantity} {ItemFactory.CreateGameItem(i.Id).Name}");
                    }
                    RaiseMessage("And you will receive:");
                    RaiseMessage($"{quest.RewardExperiencePoints} EXP");
                    RaiseMessage($"{quest.RewardGold} gold");
                    foreach(var r in quest.RewardItems)
                    {
                        RaiseMessage($"{r.Quantity} {ItemFactory.CreateGameItem(r.Id).Name}");
                    }
                    
                }
            }
        }
        private void CompleteQuestAtLocation()
        {
            foreach(Quest quest in CurrentLocation.QuestsAvailableHere)
            {
                QuestStatus questToComplete =
                    CurrentPlayer.Quests.FirstOrDefault(q => q.PlayerQuest.Id == quest.Id && !q.IsCompleted);

                if (questToComplete != null)
                {
                    if (CurrentPlayer.HasAllTheseItems(quest.ItemsToComplete))
                    {
                        foreach (ItemQuantity iq in quest.ItemsToComplete)
                        {
                            for (int i  = 0; i < iq.Quantity; i++)
                            {
                                CurrentPlayer.RemoveItemFromInventory(CurrentPlayer.Inventory.FirstOrDefault(item => item.Id == iq.Id));
                            }

                            RaiseMessage("");
                            RaiseMessage($"You completed {quest.Name} quest");

                            CurrentPlayer.ExperiencePoints += quest.RewardExperiencePoints;
                            RaiseMessage($"You receive {quest.RewardExperiencePoints} EXP");
                            CurrentPlayer.Gold += quest.RewardGold;
                            RaiseMessage($"You receive {quest.RewardGold} gold");
                            
                            foreach (var i in quest.RewardItems)
                            {
                                GameItem reward = ItemFactory.CreateGameItem(i.Id);
                                CurrentPlayer.Inventory.Add(reward);
                                RaiseMessage($"You receive {reward.Name}");
                            }

                            questToComplete.IsCompleted = true;
                        }
                    }
                }
            }
        }
        public void GetMonsterAtLocation()
        {
            CurrentMonster = CurrentLocation.GetMonster();
        }
        public void AttackCurrentMonster()
        {
            if (CurrentWeapon == null)
            {
                RaiseMessage("You must select a weapon first, dumbass!");
                return;
            }

            // Определяет урон по понстру
            int damageToMonster = RandomNumberGenerator.NumberBetween(CurrentWeapon.MinDMG, CurrentWeapon.MaxDMG);

            if (damageToMonster == 0)
            {
                RaiseMessage($"Oh god, how could you MISS THIS?! The {CurrentMonster.Name} took 0 dmg");
            }
            else
            {
                CurrentMonster.HitPoints -= damageToMonster;
                RaiseMessage($"You hit {CurrentMonster.Name} for {damageToMonster} HP");
            }

            //Если монстр убит, выдается опыт, золото, предметы
            if (CurrentMonster.HitPoints <= 0)
            {
                RaiseMessage("");
                RaiseMessage($"You defeated the {CurrentMonster.Name}");

                CurrentPlayer.ExperiencePoints += CurrentMonster.RewardExperiencePoints;
                RaiseMessage($"You receive {CurrentMonster.RewardExperiencePoints} experience points");

                CurrentPlayer.Gold += CurrentMonster.RewardGold;
                RaiseMessage($"You receive {CurrentMonster.RewardGold} gold");

                foreach(ItemQuantity itemQuantity in CurrentMonster.Inventory)
                {
                    GameItem item = ItemFactory.CreateGameItem(itemQuantity.Id);
                    CurrentPlayer.AddItemToInventory(item);                                             //количество предметов НИКАК НЕ РЕАЛИЗОВАНО
                    RaiseMessage($"You receive {itemQuantity.Quantity} {item.Name}");
                }
                
                //Переход к следующему моснтру в локации
                GetMonsterAtLocation();
            }
            else
            {
                int damageToPlayer = RandomNumberGenerator.NumberBetween(CurrentMonster.MinimumDamage, CurrentMonster.MaximumDamage);

                if (damageToPlayer == 0)
                {
                    RaiseMessage("The monster attacks, but misses you!");
                }
                else
                {
                    CurrentPlayer.HitPoints -= damageToPlayer;
                    RaiseMessage($"The {CurrentMonster.Name} hit you for {damageToPlayer} HP");
                }

                // Если игрока убили, возвращаем его домой
                if (CurrentPlayer.HitPoints <= 0)
                {
                    RaiseMessage("");
                    RaiseMessage($"The {CurrentMonster.Name} killed you :(");

                    CurrentLocation = CurrentWorld.LocationAt(0, -1); // Возврат домой
                    CurrentPlayer.HitPoints = CurrentPlayer.Level * 10; // восстанавление хп после смерти
                }

            }
            
        }
        private void RaiseMessage(string message)
        {
            OnMessageRaised?.Invoke(this, new GameMessageEventArgs(message));
        }
    }
}
