using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.EventArgs;
namespace Engine.Services
{
    public class MessageBroker
    {
        private static readonly MessageBroker s_messageBroker = new MessageBroker();

        private MessageBroker() { }

        public static MessageBroker GetInstance() => s_messageBroker;

        public event EventHandler<GameMessageEventArgs> OnMessageRaised;

        internal void RaiseMessage (string message)
        {
            OnMessageRaised?.Invoke (this, new GameMessageEventArgs (message));
        }
    }
}
