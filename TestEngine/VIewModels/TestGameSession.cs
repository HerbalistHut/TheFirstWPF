using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Engine.ViewModels;
using Engine.Models;
using System.Linq;
using Engine.Factories;
using Engine.EventArgs;

namespace TestEngine.VIewModels
{
    [TestClass]
    public class TestGameSession
    {
        [TestMethod]
        public void TestCreateGameSession()
        {
            GameSession gameSession = new GameSession();

            Assert.IsNotNull(gameSession.CurrentPlayer);
            Assert.AreEqual("Town Square", gameSession.CurrentLocation.Name);
        }
        [TestMethod]
        public void TestPlayerMovesHomeAndCompletelyHealedOnKilled() 
        {
            GameSession gameSession = new GameSession();

            gameSession.CurrentPlayer.TakeDamege(99999);

            Assert.AreEqual("Home", gameSession.CurrentLocation.Name);
            Assert.AreEqual(gameSession.CurrentPlayer.Level * 10, gameSession.CurrentPlayer.CurrentHitPoints);
        }

        //This test showed that you get a new monster ONLY if you kill it manually, the new monster does not come with OnMonsterKilled event. MUST BE FIXED (a bit later :) )
        [TestMethod]
        public void TestCompleteHerbalistQuest()
        {
            GameSession gameSession = new GameSession();

            gameSession.MoveNorth();
            gameSession.MoveNorth();
            gameSession.CurrentPlayer.CurrentWeapon = ItemFactory.CreateGameItem(0);

            for (int i = 0; i < 100; i++) 
            {
                gameSession.AttackCurrentMonster();
            }

            gameSession.MoveSouth();

            Assert.IsTrue(gameSession.CurrentPlayer.Quests.Any(q => q.IsCompleted && q.PlayerQuest.Id == 1));
            Assert.IsTrue(gameSession.CurrentPlayer.Inventory.Weapons.Any(w => w.Name == "Rusty sword"));
        }
    }
}
