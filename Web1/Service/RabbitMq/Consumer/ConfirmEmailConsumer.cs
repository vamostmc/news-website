using Web1.Helps;
using Web1.Models;
using Web1.Repository;
using Web1.Service.Mail;
using Web1.Service.RabbitMq.Connection;

namespace Web1.Service.RabbitMq.Consumer
{
    public class ConfirmEmailConsumer : BaseRabbitMqConsumer<AppUser>
    {
        public ConfirmEmailConsumer(RabbitMqConnection rabbitMqService, 
                                    IServiceScopeFactory serviceScopeFactory) : 
            base(rabbitMqService, serviceScopeFactory)
        {
        }

        protected override string QueueName => KeyRabbit.CONFIRM_EMAIL_QUEUE;

        protected override async Task ProcessMessageAsync(AppUser message, IServiceScope scope)
        {
            var confirmEmailService = scope.ServiceProvider.GetRequiredService<IConfirmMailService>();
            await confirmEmailService.SendVerifyEmailAsync(message);
        }
    }
    
}
