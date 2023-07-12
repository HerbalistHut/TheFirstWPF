using Engine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Actions
{
    public class Heal : IAction
    {
        private readonly GameItem _item;
        private readonly int _hitPointsToHeal;

        public event EventHandler<string> OnActionPerformed;
        
        public Heal(GameItem item, int hitPointsToHeal)
        {
            if (item.Category != GameItem.ItemCategory.Consumable)
            {
                throw new ArgumentException($"{item} is no consumable");
            }

            _item = item;
            _hitPointsToHeal = hitPointsToHeal;
        }
        public void Execute(LivingEntity actor, LivingEntity target)
        {
            string actorName = (actor is Player) ? "You" : $"The {actor.Name}";
            string targetName = (target is Player) ? "you" : $"the {target.Name}";

            ReportResult($"{actorName} heal {targetName} for {_hitPointsToHeal} hit points");
            actor.Heal(_hitPointsToHeal);
        }
        private void ReportResult(string result)
        {
            OnActionPerformed?.Invoke(this, result);
        }
    }
}
