using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Models;
using Engine.Factories;
using System.ComponentModel;
using Engine.EventArgs;

namespace Engine.ViewModels
{
    public class GameSession : BaseNotification
    {
        public event EventHandler<GameMessageEventArgs> OnMessageRised;
        private Monster _currentMonster;
        private Location _currentLocation;
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

                GivePlayerQuestAtLocation();
                GetMonsterAtLocation();
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
                    RaisedMessage("");
                    RaisedMessage($"You see {CurrentMonster.Name} here!");
                }
            }
        }
        public GameSession()
        {
            CurrentPlayer = new Player { Name = "Nikita", CharacterClass = "This method is not inp", ExperiencePoints = 0, Gold = 100, HitPoints = 10, Level = 0};
            
            CurrentWorld = WorldFactory.CreateWorld();

            CurrentLocation = CurrentWorld.LocationAt(0, -1);

        }

        public bool HasLocationToNorth
        {
            get { return CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1) != null; }
        }
        public bool HasLocationToSouth
        {
            get { return CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1) != null; }
        }
        public bool HasLocationToWest
        {
            get { return CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate) != null; }
        }
        public bool HasLocationToEast
        {
            get { return CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate) != null; }
        }
        public void MoveNorth()
        {
            CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate + 1);
        }

        public void MoveSouth() 
        {
            CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate, CurrentLocation.YCoordinate - 1);
        } 
        public void MoveWest() 
        {
            CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate - 1, CurrentLocation.YCoordinate);
        }
        public void MoveEast() 
        {
            CurrentLocation = CurrentWorld.LocationAt(CurrentLocation.XCoordinate + 1, CurrentLocation.YCoordinate);
        }
        public bool HasMonster => CurrentMonster != null;
        private void GivePlayerQuestAtLocation()
        {
            foreach(Quest quest in CurrentLocation.QuestsAvailableHere)
            {
                if (!CurrentPlayer.Quests.Any(q => q.PlayerQuest.Id == quest.Id))
                {
                    CurrentPlayer.Quests.Add(new QuestStatus(quest));
                }
            }
        }
        public void GetMonsterAtLocation()
        {
            CurrentMonster = CurrentLocation.GetMonster();
        }
        private void RaisedMessage(string message)
        {
            OnMessageRised?.Invoke(this, new GameMessageEventArgs(message));
        }
    }
}
