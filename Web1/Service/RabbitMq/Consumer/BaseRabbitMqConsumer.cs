
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Web1.Service.RabbitMq.Connection;

namespace Web1.Service.RabbitMq.Consumer
{
    public abstract class BaseRabbitMqConsumer<T> : BackgroundService
    {
        private readonly IModel _channel;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        protected BaseRabbitMqConsumer(RabbitMqConnection rabbitMqService, 
                                       IServiceScopeFactory serviceScopeFactory)
        {
            _channel = rabbitMqService.GetChannel();
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected abstract Task ProcessMessageAsync(T message, IServiceScope scope);

        protected abstract string QueueName { get; }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += async (model, ea) =>
                {
                    if (stoppingToken.IsCancellationRequested) return;

                    var body = ea.Body.ToArray();
                    var messageString = Encoding.UTF8.GetString(body);

                    using var scope = _serviceScopeFactory.CreateScope();
                    var data = DeserializeMessage(messageString);
                    if (data != null)
                    {
                        await ProcessMessageAsync(data, scope);
                    }
                };

                _channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
            }, stoppingToken);
        }

        protected virtual T DeserializeMessage(string message)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(message);
            }
            catch
            {
                return default;
            }
        }
    }
}
