using System;
using System.Web.Http;
using P2P_Netwerken.ChatModels;

namespace P2P_Netwerken.ChatBusiness
{
    public class ChatController : ApiController
    {
        public void Post(MessageReceiver simpleMessage)
        {
            MessageArrived(new Message(simpleMessage));
        }

        public delegate void EventHandler(object sender, MessageEventArgs args);
        public static event EventHandler ThrowMessageArrivedEvent = delegate { };

        public void MessageArrived(Message m)
        {
            ThrowMessageArrivedEvent(this, new MessageEventArgs(m));
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(Message m)
        {
            this.Message = m;
        }
        public Message Message;
    }
}
