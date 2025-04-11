using Web1.Data;
using Web1.Helps;
using Web1.Models;
using Web1.Repository;
using Web1.Service.RabbitMq.Producer;

namespace Web1.Service.ChatBox
{

    public class ChatService : IChatService
    {
        private readonly IMessageRepository _messageRepo;
        private readonly IRabbitMqProducer _rabbitMqProducer;

        public ChatService(IMessageRepository messageRepo, 
                           IRabbitMqProducer rabbitMqProducer) 
        {
            _messageRepo = messageRepo;
            _rabbitMqProducer = rabbitMqProducer;
        }
        public async Task SendMessageAsync(MessageDto message)
        {
            await _messageRepo.SaveMessageAsync(message);
            await _rabbitMqProducer.PublishEvent(KeyRabbit.CHAT_BOX_ROUTING, message);
        }
    }
}
