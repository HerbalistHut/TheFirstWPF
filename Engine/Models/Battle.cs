﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.EventArgs;
using Engine.Services;

namespace Engine.Models
{
    public class Battle : IDisposable
    {
        private readonly MessageBroker _messageBroker = MessageBroker.GetInstance();
        private readonly Player _player;
        private readonly Monster _opponent;

        public event EventHandler<CombatVictoryEventArgs> OnCombatVictory;

        public Battle(Player player, Monster opponent)
        {
            _player = player;
            _opponent = opponent;

            _player.OnActionPerformed += OnCombatantActionPerformed;
            _opponent.OnActionPerformed += OnCombatantActionPerformed;
            _opponent.OnKilled += OnOpponentKilled;

            _messageBroker.RaiseMessage("");
            _messageBroker.RaiseMessage($"You see a {_opponent.Name} here!");
        
            if (CombatService.FirstAttacker(_player, _opponent) == CombatService.Combatant.Opponent)
            {
                AttackPlayer();
            }
        }

        public void AttackOpponent()
        {
            if (_player.CurrentWeapon == null)
            {
                _messageBroker.RaiseMessage("You can't attack without weapon in your hands!");
                return;
            }

            _player.UseCurrentWeaponOn(_opponent);

            if (_opponent.IsAlive)
            {
                AttackPlayer();
            }
        }

        private void OnOpponentKilled(object sender, System.EventArgs e)
        {
            _messageBroker.RaiseMessage("");
            _messageBroker.RaiseMessage($"You defeated the {_opponent.Name}");

            _player.AddExperience(_opponent.RewardExperiencePoints);
            _messageBroker.RaiseMessage($"You receive {_opponent.RewardExperiencePoints} experience points");

            _player.ReceiveGold(_opponent.Gold);
            _messageBroker.RaiseMessage($"You receive {_opponent.Gold} gold");

            foreach (GameItem gameItem in _opponent.Inventory.Items)
            {
                _player.AddItemToInventory(gameItem);
                _messageBroker.RaiseMessage($"You receive one {gameItem.Name}");
            }

            OnCombatVictory?.Invoke(this, new CombatVictoryEventArgs());
        }

        private void AttackPlayer()
        {
            _opponent.UseCurrentWeaponOn(_player);
        }

        private void OnCombatantActionPerformed(object sender, string result)
        {
            _messageBroker.RaiseMessage(result);
        }

        public void Dispose()
        {
            _player.OnActionPerformed -= OnCombatantActionPerformed;
            _opponent.OnActionPerformed -= OnCombatantActionPerformed;
            _opponent.OnKilled -= OnOpponentKilled;
        }
    }
}
