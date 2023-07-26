using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.Services;

namespace Engine.Actions
{
    public class AttackWithWeapon : BaseAction, IAction
    {
        private readonly int _minimumDamage;
        private readonly int _maximumDamage;

        public AttackWithWeapon(GameItem itemInUse, int minimumDamage, int maximumDamage)
            :base(itemInUse)
        {
            if (itemInUse.Category != GameItem.ItemCategory.Weapon)
            {
                throw new ArgumentException($"{itemInUse.Name} is not a weapon");
            }

            if (minimumDamage < 0)
            {
                throw new ArgumentException($"Minimum damage is smaller than 0");
            }

            if (maximumDamage < minimumDamage)
            {
                throw new ArgumentException("Maximum damage is less than minimum damage");
            }

            _minimumDamage = minimumDamage;
            _maximumDamage = maximumDamage;
        }
        public void Execute(LivingEntity actor, LivingEntity target)
        {

            string actorName = (actor is Player) ? "You" : $"The {actor.Name}";
            string targetName = (target is Player) ? "you" : $"the {target.Name}";

            if (!CombatService.AttackSucceeded(actor, target))
            {
                ReportResult($"{actorName} missed hit to {targetName}.");
            }
            else
            {
                int damage = RandomNumberGenerator.NumberBetween(_minimumDamage, _maximumDamage);
                ReportResult($"{actorName} dealt {damage} damage to {targetName}.");
                target.TakeDamege(damage);
            }
        }
    }
}
