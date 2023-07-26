using System;
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
                OnPropertyChanged();
            }
        }
        public int ExperiencePoints
        {
            get { return _experiencePoints; }
            private set
            {
                _experiencePoints = value;

                OnPropertyChanged();

                SetLevelAndMaximumHitPoints();
            }
        }
        public ObservableCollection<QuestStatus> Quests { get; }
        public ObservableCollection<Recipe> Recipes { get; }
        public event EventHandler OnLeveledUp;
        public Player(string name, int currentHitPoints, int maximumHitPoints, int gold, int experiencePoints, int dexterity) :
            base(name, currentHitPoints, maximumHitPoints, gold, dexterity)
        {
            ExperiencePoints = experiencePoints;
            Quests = new ObservableCollection<QuestStatus>();
            Recipes = new ObservableCollection<Recipe>();
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

        public void LearnRecipe(Recipe recipe)
        {
            if (!Recipes.Any(x => x.ID == recipe.ID))
            {
                Recipes.Add(recipe);
            }
        }
    }
}
