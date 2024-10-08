using Newtonsoft.Json;
using Notification.Infrastructure.RabbitMQClient.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Notification.Infrastructure.RabbitMQClient.Base
{
    public partial class QueueOperation : IQueueOperation
    {
        public static readonly TimeSpan _maxWait = TimeSpan.FromMinutes(5);
        private readonly IConnection _connection;
        private AutoResetEvent _messageReceived;

        uint msgCount = 0;

        public QueueOperation(IConnection connection)
        {
            _connection = connection;
            _messageReceived = new AutoResetEvent(false);
        }

        public bool ConfigureExchange(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments = null)
        {
            bool exchangeResult = false;

            using (var channel = _connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange, type, durable, autoDelete, arguments);

                exchangeResult = true;
                return exchangeResult;
            }
        }

        public bool ConfigureQueue(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments = null)
        {
            bool exchangeResult = false;

            using (var channel = _connection.CreateModel())
            {
                channel.QueueDeclare(queue, durable, exclusive, autoDelete, arguments);

                exchangeResult = true;
                return exchangeResult;
            }
        }

        public bool BindQueueToExchange(string queue, string exchange, string routingKey, IDictionary<string, object> arguments = null)
        {
            bool exchangeResult = false;

            using (var channel = _connection.CreateModel())
            {
                channel.QueueBind(queue, exchange, routingKey, arguments);

                exchangeResult = true;
                return exchangeResult;
            }
        }

        public void ConsumeQueue(string queue, string exchange, string exchangeType, string routingKey, ushort prefetchCount, EventHandler<BasicDeliverEventArgs> receivedEventHandler, long messageTtl = 0)
        {
            var channel = _connection.CreateModel();

            channel.BasicQos(prefetchSize: 0, prefetchCount: prefetchCount, global: false);

            if (!string.IsNullOrEmpty(exchange))
            {
                channel.ExchangeDeclare(exchange: exchange, type: exchangeType, durable: true, autoDelete: false);
                channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(queue: queue, exchange: exchange, routingKey: routingKey);
            }

            var consumer = new EventingBasicConsumer(channel);

            // Received olayını dinamik olarak ayarlıyoruz
            consumer.Received += receivedEventHandler;

            // Kuyruğu dinamik olarak tüketmeye başlıyoruz
            channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
        }



        public void PublishMessage<TMessage>(TMessage message, string queue, string exchange, string routingKey, long messageTtl = 0) where TMessage : class
        {
            using (var channel = _connection.CreateModel())
            {
                //Get Channel Properties
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                string serializedObj = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(serializedObj);


                channel.BasicPublish(exchange: exchange,
                                     routingKey: routingKey,
                                     basicProperties: properties,
                                     body: body);
            }
        }
    }
}
