using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Actions
{
    public class Heal : BaseAction, IAction
    {
        private readonly int _hitPointsToHeal;

        
        public Heal(GameItem itemInUse, int hitPointsToHeal) : base(itemInUse)
        {
            if (itemInUse.Category != GameItem.ItemCategory.Consumable)
            {
                throw new ArgumentException($"{itemInUse} is no consumable");
            }

            _hitPointsToHeal = hitPointsToHeal;
        }
        public void Execute(LivingEntity actor, LivingEntity target)
        {
            string actorName = (actor is Player) ? "You" : $"The {actor.Name}";
            string targetName = (target is Player) ? "you" : $"the {target.Name}";

            ReportResult($"{actorName} heal {targetName} for {_hitPointsToHeal} hit points");
            actor.Heal(_hitPointsToHeal);
        }
    }
}
