namespace ServiceBroker.Repositories
{
    internal interface IServiceBrokerRepository
    {
        void InsertErrorMessage(string message);
    }
}
