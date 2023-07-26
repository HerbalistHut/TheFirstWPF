﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Engine.Factories;
using System.ComponentModel;
using Engine.EventArgs;
using System.Security.Permissions;
using Engine.Services;

namespace Engine.ViewModels
{
    public class GameSession : BaseNotification
    {
        private readonly MessageBroker _messageBroker = MessageBroker.GetInstance();

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

                    _messageBroker.RaiseMessage("");
                    _messageBroker.RaiseMessage($"You see {CurrentMonster.Name} here!");
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
            
            if (!CurrentPlayer.Inventory.Weapons.Any())
            {
                CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(1001));
            }

            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(2001));
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(2001));
            CurrentPlayer.LearnRecipe(RecipeFactory.RecipeById(1));
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(3001));
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(3001));
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(3002));
            CurrentPlayer.AddItemToInventory(ItemFactory.CreateGameItem(3003));

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
        {
            if (HasLocationToNorth)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1);
            }
        }

        public void MoveSouth()
        {
            if (HasLocationToSouth)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1);
            }
        }
        public void MoveWest()
        {
            if (HasLocationToWest)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate);
            }
        }
        public void MoveEast()
        {
            if (HasLocationToEast)
            {
                CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate);
            }
        }
        public bool HasMonster => CurrentMonster != null;
        
        public bool HasTrader => CurrentTrader != null;
        
        private void GivePlayerQuestAtLocation()
        {
            foreach(Quest quest in CurrentLocation.QuestsAvailableHere)
            {
                if (!CurrentPlayer.Quests.Any(q => q.PlayerQuest.Id == quest.Id))
                {
                    CurrentPlayer.Quests.Add(new QuestStatus(quest));

                    _messageBroker.RaiseMessage("");
                    _messageBroker.RaiseMessage($"You recieve the '{quest.Name}' quest");
                    _messageBroker.RaiseMessage(quest.Description);
                    _messageBroker.RaiseMessage("Return with:");
                    foreach (var i in quest.ItemsToComplete)
                    {
                        _messageBroker.RaiseMessage($"    {i.Quantity} {ItemFactory.CreateGameItem(i.Id).Name}");
                    }
                    _messageBroker.RaiseMessage("And you will receive:");
                    _messageBroker.RaiseMessage($"{quest.RewardExperiencePoints} EXP");
                    _messageBroker.RaiseMessage($"{quest.RewardGold} gold");
                    foreach(var r in quest.RewardItems)
                    {
                        _messageBroker.RaiseMessage($"{r.Quantity} {ItemFactory.CreateGameItem(r.Id).Name}");
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
                    if (CurrentPlayer.Inventory.HasAllTheseItems(quest.ItemsToComplete))
                    {
                        CurrentPlayer.RemoveItemFromInventory(quest.ItemsToComplete);

                        _messageBroker.RaiseMessage("");
                        _messageBroker.RaiseMessage($"You completed {quest.Name} quest");

                        _messageBroker.RaiseMessage($"You receive {quest.RewardExperiencePoints} EXP");
                        CurrentPlayer.AddExperience(quest.RewardExperiencePoints);

                        _messageBroker.RaiseMessage($"You receive {quest.RewardGold} gold");
                        CurrentPlayer.ReceiveGold(quest.RewardGold);

                        foreach (var i in quest.RewardItems)
                        {
                            GameItem reward = ItemFactory.CreateGameItem(i.Id);
                            _messageBroker.RaiseMessage($"You receive {reward.Name}");
                            CurrentPlayer.AddItemToInventory(reward);
                        }

                        questToComplete.IsCompleted = true;

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
            if (CurrentMonster == null)
            {
                _messageBroker.RaiseMessage("Why are you angry? Stop beating an air");
                return;
            }
            if (CurrentPlayer.CurrentWeapon == null)
            {
                _messageBroker.RaiseMessage("You must select a weapon first, dumbass!");
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

        public void CraftItemUsing(Recipe recipe)
        {
            if (CurrentPlayer.Inventory.HasAllTheseItems(recipe.Ingredients))
            {
                CurrentPlayer.RemoveItemFromInventory(recipe.Ingredients);

                foreach (ItemQuantity itemQuantity in recipe.OutputItems)
                {
                    for (int i = 0; i < itemQuantity.Quantity; i++)
                    {
                        GameItem outputItem = ItemFactory.CreateGameItem(itemQuantity.Id);
                        CurrentPlayer.AddItemToInventory(outputItem);
                        _messageBroker.RaiseMessage($"You just crafted one {outputItem.Name}");
                    }
                }
            }
            else
            {
                _messageBroker.RaiseMessage("To craft this you need:");
                foreach(ItemQuantity itemQuantity in recipe.Ingredients)
                {
                    _messageBroker.RaiseMessage($"{itemQuantity.Quantity} {ItemFactory.ItemName(itemQuantity.Id)}");
                }
            }
        }
        
        private void OnCurrentMonsterActionPerformed(object sender, string result)
        {
            _messageBroker.RaiseMessage(result);
        }
        
        private void OnCurrentPlayerKilled (object sender, System.EventArgs e)
        {
                _messageBroker.RaiseMessage("");
                _messageBroker.RaiseMessage("You have been killed :(");

                CurrentLocation = CurrentWorld.LocationAt(0, -1); // Return Home
            CurrentPlayer.CompletelyHeal();
        }
        
        private void OnCurrentMonsterKilled (object sender, System.EventArgs e)
        {
            _messageBroker.RaiseMessage("");
            _messageBroker.RaiseMessage($"You defeated the {CurrentMonster.Name}");

            CurrentPlayer.AddExperience(CurrentMonster.RewardExperiencePoints);
            _messageBroker.RaiseMessage($"You receive {CurrentMonster.RewardExperiencePoints} experience points");

            CurrentPlayer.ReceiveGold(CurrentMonster.Gold);
            _messageBroker.RaiseMessage($"You receive {CurrentMonster.Gold} gold");

            foreach (GameItem gameItem in CurrentMonster.Inventory.Items)
            {
                CurrentPlayer.AddItemToInventory(gameItem);
                _messageBroker.RaiseMessage($"You receive one {gameItem.Name}");
            }
        }
        
        private void OnCurrentPlayerLeveledUp(object sender, System.EventArgs e)
        {
            _messageBroker.RaiseMessage($"You are now level {CurrentPlayer.Level}");
        }
        
        private void OnCurrentPlayerActionPerformed(object sender, string result)
        {
            _messageBroker.RaiseMessage(result);
        }
    }
}
