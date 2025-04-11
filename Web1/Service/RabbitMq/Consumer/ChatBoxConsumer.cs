using Web1.Data;
using Web1.Helps;
using Web1.Models;
using Web1.Service.RabbitMq.Connection;
using Web1.Service.SignalR.SignalRNotification;

namespace Web1.Service.RabbitMq.Consumer
{
    public class ChatBoxConsumer : BaseRabbitMqConsumer<MessageDto>
    {
        public ChatBoxConsumer(RabbitMqConnection rabbitMqService, IServiceScopeFactory serviceScopeFactory) : base(rabbitMqService, serviceScopeFactory)
        {
        }

        protected override string QueueName => KeyRabbit.CHAT_BOX_QUEUE;

        protected async override Task ProcessMessageAsync(MessageDto message, IServiceScope scope)
        {
            var signalRService = scope.ServiceProvider.GetService<ISignalRNotificationService>();
            await signalRService.SendMessageChat(message.ReceiverId, message);
        }
    }
}
