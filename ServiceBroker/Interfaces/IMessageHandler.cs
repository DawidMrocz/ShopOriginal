namespace ServiceBroker.Interfaces
{
    public interface IMessageHandler
    {
        void HandleMessage(string inputDataXml);
    }
}
