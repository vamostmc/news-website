using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Threading.Channels;
using Web1.Helps;

namespace Web1.Service.RabbitMq.Connection 
{
    public class RabbitMqConnection : IDisposable
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public RabbitMqConnection(IConfiguration configuration)
        {
            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"],
                Port = int.Parse(configuration["RabbitMQ:Port"]),
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Khai báo Exchange
            _channel.ExchangeDeclare(exchange: KeyRabbit.nameExchange, type: ExchangeType.Direct);

            // Khai báo Queue
            _channel.QueueDeclare(KeyRabbit.FORGOT_PASSWORD_QUEUE, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueDeclare(KeyRabbit.CONFIRM_EMAIL_QUEUE, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueDeclare(KeyRabbit.ADMIN_NOTIFICATION_QUEUE, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueDeclare(KeyRabbit.USER_NOTIFICATION_QUEUE, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueDeclare(KeyRabbit.CHAT_BOX_QUEUE, durable: true, exclusive: false, autoDelete: false);

            // Bind Queue với Exchange
            _channel.QueueBind(KeyRabbit.FORGOT_PASSWORD_QUEUE, KeyRabbit.nameExchange, KeyRabbit.FORGOT_PASSWORD_ROUTING);
            _channel.QueueBind(KeyRabbit.CONFIRM_EMAIL_QUEUE, KeyRabbit.nameExchange, KeyRabbit.CONFIRM_EMAIL_ROUTING);
            _channel.QueueBind(KeyRabbit.USER_NOTIFICATION_QUEUE, KeyRabbit.nameExchange, KeyRabbit.USER_NOTIFICATION_ROUTING);
            _channel.QueueBind(KeyRabbit.ADMIN_NOTIFICATION_QUEUE, KeyRabbit.nameExchange, KeyRabbit.ADMIN_NOTIFICATION_ROUTING);
            _channel.QueueBind(KeyRabbit.CHAT_BOX_QUEUE, KeyRabbit.nameExchange, KeyRabbit.CHAT_BOX_ROUTING);
        }

        public IModel GetChannel() => _channel;

        public IModel CreateChannel()
        {
            return _connection.CreateModel();  // Tạo mới mỗi khi gọi
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
