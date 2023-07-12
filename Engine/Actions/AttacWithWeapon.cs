using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Actions
{
    public class AttacWithWeapon : BaseAction, IAction
    {
        private readonly int _minimumDamage;
        private readonly int _maximumDamage;

        public AttacWithWeapon(GameItem itemInUse, int minimumDamage, int maximumDamage)
            :base(itemInUse)
        {
            if (itemInUse.Category != GameItem.ItemCategory.Weapon)
            {
                throw new ArgumentException($"{itemInUse.Name} is not a weapon");
            }

            if (_minimumDamage < 0)
            {
                throw new ArgumentException($"Minimum damage is smaller than 0");
            }

            if (_minimumDamage > _maximumDamage)
            {
                throw new ArgumentException("Maxumum damage is less than minimum damage");
            }

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
    }
}
