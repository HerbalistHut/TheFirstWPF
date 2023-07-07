using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Engine.ViewModels;

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
    }
}
