﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Models
{
    public class Player : LivingEntity
    {
        private string _characterClass;
        private int _experiencePoints;
        public string CharacterClass
        {
            get { return _characterClass; }
            set
            {
                _characterClass = value;
                OnPropertyChanged(nameof(CharacterClass));
            }
        }
        public int ExperiencePoints
        {
            get { return _experiencePoints; }
            private set
            {
                _experiencePoints = value;

                OnPropertyChanged(nameof(ExperiencePoints));

                SetLevelAndMaximumHitPoints();
            }
        }
        public ObservableCollection<QuestStatus> Quests { get; set; }
        public event EventHandler OnLeveledUp;
        public Player(string name, int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints) :
            base(name, currentHitPoints, maximumHitPoints, gold)
        {
            ExperiencePoints = experiencePoints;
            Quests = new ObservableCollection<QuestStatus>();
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

        public void AddExperience(int experiencePoints)
        {
            ExperiencePoints += experiencePoints;
        }

        private void SetLevelAndMaximumHitPoints()
        {
            int originalLevel = Level;

            Level = (ExperiencePoints / 100) + 1;

            if (originalLevel < Level)
            {
                MaximumHitPoints = Level * 10;
                
                OnLeveledUp?.Invoke(this, System.EventArgs.Empty);
            }
        }
    }
}
