using ServiceBroker.Interfaces;


namespace ServiceBroker.Handler
{
    public abstract class MessageHandler : IMessageHandler
    {
        protected MessageHandler()
        {
           // Log = Logger.Logger.GetInstance(GetType());
        }

       // public ILog Log { get; private set; }

        public abstract void HandleMessage(string inputDataXml);
    }
}
