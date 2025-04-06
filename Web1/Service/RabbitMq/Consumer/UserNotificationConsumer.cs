using Web1.Data;
using Web1.Helps;
using Web1.Models;
using Web1.Repository;
using Web1.Service.RabbitMq.Connection;
using Web1.Service.SignalR.SignalRNotification;

namespace Web1.Service.RabbitMq.Consumer
{
    public class UserNotificationConsumer : BaseRabbitMqConsumer<Notification>
    {

        public UserNotificationConsumer(RabbitMqConnection rabbitMqService, 
                                    IServiceScopeFactory serviceScopeFactory
                                   ) : 
            base(rabbitMqService, serviceScopeFactory)
        {
            
        }

        protected override string QueueName => KeyRabbit.USER_NOTIFICATION_QUEUE;

        protected override async Task ProcessMessageAsync(Notification message, IServiceScope scope)
        {
            //Gửi thông báo sang SignalR để trả về cho user
            var signalRService = scope.ServiceProvider.GetService<ISignalRNotificationService>();
            await signalRService.SendNotificationToUser(message.TargetId, message.Message);
        }
    }
}
