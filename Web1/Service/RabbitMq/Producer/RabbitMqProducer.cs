using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

using System.Threading.Channels;
using Web1.Helps;
using Web1.Service.RabbitMq.Connection;

namespace Web1.Service.RabbitMq.Producer
{
    public class RabbitMqProducer : IRabbitMqProducer
    {
        private readonly RabbitMqConnection _rabbitMqConnection;
        private readonly IModel _channel;
        public RabbitMqProducer(RabbitMqConnection rabbitMqConnection) 
        {
            _rabbitMqConnection = rabbitMqConnection;
            _channel = _rabbitMqConnection.GetChannel();
        }


        public async Task PublishEvent(string routingKey, object message)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            await Task.Run(() =>
                _channel.BasicPublish(exchange: KeyRabbit.nameExchange, routingKey: routingKey, basicProperties: null, body: body)
            );
        }

    }
}
