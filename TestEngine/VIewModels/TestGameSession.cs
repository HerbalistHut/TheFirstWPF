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
            Assert.AreEqual("Town square", gameSession.CurrentLocation.Name);
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
            gameSession.CurrentWeapon = new GameItem(GameItem.ItemCategory.Weapon,0,"0",0,true,1000,1000);

            Assert.IsTrue(gameSession.CurrentPlayer.Weapons.Any(w => w.Name == "Rusty Sword"));
            for (int i = 0; i < 100; i++) 
            {
                gameSession.AttackCurrentMonster();
            }

            gameSession.MoveSouth();

            Assert.IsTrue(gameSession.CurrentPlayer.Quests.Any(q => q.IsCompleted && q.PlayerQuest.Id == 1));
        }
    }
}
