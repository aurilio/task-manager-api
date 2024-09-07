namespace TaskManager.Domain.Interfaces
{
    public interface IMessageBus
    {
        void Publish(string queueName, string message);
    }
}