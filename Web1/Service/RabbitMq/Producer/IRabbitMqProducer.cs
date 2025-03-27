namespace Web1.Service.RabbitMq.Producer
{
    public interface IRabbitMqProducer 
    {
        public Task PublishEvent(string routingKey, object message);

    }
}
