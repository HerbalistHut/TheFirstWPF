using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Actions
{
    public class AttacWithWeapon : IAction
    {
        private readonly GameItem _weapon;
        private readonly int _minimumDamage;
        private readonly int _maximumDamage;

        public event EventHandler<string> OnActionPerformed;

        public AttacWithWeapon(GameItem weapon, int minimumDamage, int maximumDamage)
        {
            if (weapon.Category != GameItem.ItemCategory.Weapon)
            {
                throw new ArgumentException($"{weapon.Name} is not a weapon");
            }

            if (_minimumDamage < 0)
            {
                throw new ArgumentException($"Minimum damage is smaller than 0");
            }

            if (_minimumDamage > _maximumDamage)
            {
                throw new ArgumentException("Maxumum damage is less than minimum damage");
            }

            _weapon = weapon;
            _minimumDamage = minimumDamage;
            _maximumDamage = maximumDamage;
        }
        public void Execute(LivingEntity actor, LivingEntity target)
        {
            int damage = RandomNumberGenerator.NumberBetween(_minimumDamage, _maximumDamage);

            string actorName = (actor is Player) ? "You" : $"The {actor.Name}";
            string targetName = (target is Player) ? "you" : $"the {target.Name}";
            if (damage == 0)
            {
                ReportResult($"{actorName} missed hit to {targetName}.");
            }
            else
            {
                ReportResult($"{actorName} dealt {damage} damage to {targetName}.");
                target.TakeDamege(damage);
            }
        }
        private void ReportResult(string result)
        {
            OnActionPerformed?.Invoke(this, result);
        }
    }
}
