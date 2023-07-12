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
        private Player _currentPlayer;
        public World CurrentWorld { get; }
        public Player CurrentPlayer {
            get => _currentPlayer;
            set
            {
                //This is used just for optimization despite the fact that the programm is small
                if (_currentPlayer != null)
                {
                    _currentPlayer.OnActionPerformed -= OnCurrentPlayerActionPerformed;
                    _currentPlayer.OnKilled -= OnCurrentPlayerKilled;
                    _currentPlayer.OnLeveledUp -= OnCurrentPlayerLeveledUp;
                }

                _currentPlayer = value;

                if (_currentPlayer != null)
                {
                    _currentPlayer.OnActionPerformed += OnCurrentPlayerActionPerformed;
                    _currentPlayer.OnKilled += OnCurrentPlayerKilled;
                    _currentPlayer.OnLeveledUp += OnCurrentPlayerLeveledUp;
                }
            } 
        }
        public Location CurrentLocation {
            get { return _currentLocation; }
            set
            {
                _currentLocation = value;
                OnPropertyChanged();
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
                if (_currentMonster != null)
                {
                    _currentMonster.OnActionPerformed -= OnCurrentMonsterActionPerformed;
                    _currentMonster.OnKilled -= OnCurrentMonsterKilled;
                }
                _currentMonster = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasMonster));

                if (_currentMonster != null)
                {
                    _currentMonster.OnActionPerformed += OnCurrentMonsterActionPerformed;
                    _currentMonster.OnKilled += OnCurrentMonsterKilled;

                    RaiseMessage("");
                    RaiseMessage($"You see {CurrentMonster.Name} here!");
                }
            }
        }
        public Trader CurrentTrader
        {
            get => _currentTrader;
            set
            {
                _currentTrader = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(HasTrader));
            }
        }
       
        public GameSession()
        {
            CurrentPlayer = new Player("Nikitka", 10, 10, 1, 95);
            
            if (!CurrentPlayer.Weapons.Any())
            {
                CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(1001));
            }

            CurrentWorld = WorldFactory.CreateWorld();

            CurrentLocation = CurrentWorld.LocationAt(0, 0);

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

                            CurrentPlayer.AddExperience(quest.RewardExperiencePoints);
                            RaiseMessage($"You receive {quest.RewardExperiencePoints} EXP");

                            CurrentPlayer.ReceiveGold(quest.RewardGold);
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
            if (CurrentPlayer.CurrentWeapon == null)
            {
                RaiseMessage("You must select a weapon first, dumbass!");
                return;
            }

            CurrentPlayer.UseCurrentWeaponOn(CurrentMonster);

            if (CurrentMonster.IsDead)
            {
                // Get to next monster
                GetMonsterAtLocation();
            }
            else
            {
                CurrentMonster.UseCurrentWeaponOn(CurrentPlayer);
            }
            
        }

        private void OnCurrentMonsterActionPerformed(object sender, string result)
        {
            RaiseMessage(result);
        }
        private void OnCurrentPlayerKilled (object sender, System.EventArgs e)
        {
                RaiseMessage("");
                RaiseMessage("You have been killed :(");

                CurrentLocation = CurrentWorld.LocationAt(0, -1); // Return Home
            CurrentPlayer.CompletelyHeal();
        }
        private void OnCurrentMonsterKilled (object sender, System.EventArgs e)
        {
            RaiseMessage("");
            RaiseMessage($"You defeated the {CurrentMonster.Name}");

            CurrentPlayer.AddExperience(CurrentMonster.RewardExperiencePoints);
            RaiseMessage($"You receive {CurrentMonster.RewardExperiencePoints} experience points");

            CurrentPlayer.ReceiveGold(CurrentMonster.Gold);
            RaiseMessage($"You receive {CurrentMonster.Gold} gold");

            foreach (GameItem gameItem in CurrentMonster.Inventory)
            {
                CurrentPlayer.AddItemToInventory(gameItem);
                RaiseMessage($"You receive one {gameItem.Name}");
            }
        }
        private void OnCurrentPlayerLeveledUp(object sender, System.EventArgs e)
        {
            RaiseMessage($"You are now level {CurrentPlayer.Level}");
        }
        private void OnCurrentPlayerActionPerformed(object sender, string result)
        {
            RaiseMessage(result);
        }
        private void RaiseMessage(string message)
        {
            OnMessageRaised?.Invoke(this, new GameMessageEventArgs(message));
        }
    }
}
