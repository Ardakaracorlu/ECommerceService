using RabbitMQ.Client.Events;

namespace Notification.RabbitMQ.RabbitMQClient.Interface
{
    public interface IQueueOperation
    {
        /// <summary>
        /// Exchange Oluşturma
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="type"></param>
        /// <param name="durable"></param>
        /// <param name="autoDelete"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public bool ConfigureExchange(string exchange, string type, bool durable, bool autoDelete, IDictionary<string, object> arguments = null);
        /// <summary>
        /// Queue Yaratılır
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="durable"></param>
        /// <param name="exclusive"></param>
        /// <param name="autoDelete"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public bool ConfigureQueue(string queue, bool durable, bool exclusive, bool autoDelete, IDictionary<string, object> arguments = null);
        /// <summary>
        /// Queue Exchange birbirine bağlanır
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="exchange"></param>
        /// <param name="routingKey"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public bool BindQueueToExchange(string queue, string exchange, string routingKey, IDictionary<string, object> arguments = null);
        void PublishMessage<TMessage>(TMessage message, string queue, string exchange, string routingKey, long messageTtl = 0) where TMessage : class;
        void ConsumeQueue(string queue, string exchange, string exchangeType, string routingKey, ushort prefetchCount, EventHandler<BasicDeliverEventArgs> receivedEventHandler, long messageTtl = 0);
    }
}
