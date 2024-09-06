using RabbitMQ.Client;
using System.Text;
using Task.Manager.Domain.Interfaces;

namespace Task.Manager.Messaging
{
    public class MessageBus : IMessageBus
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBus(IConnection connection)
        {
            _connection = connection;
            _channel = _connection.CreateModel();
        }

        public void Publish(string queueName, string message)
        {
            _channel.QueueDeclare(queue: queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                  routingKey: queueName,
                                  basicProperties: null,
                                  body: body);
        }
    }
}
