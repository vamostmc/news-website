using Web1.Helps;
using Web1.Models;
using Web1.Service.Account;
using Web1.Service.RabbitMq.Connection;

namespace Web1.Service.RabbitMq.Consumer
{
    public class ForgotPasswordConsumer : BaseRabbitMqConsumer<AppUser>
    {
        public ForgotPasswordConsumer(RabbitMqConnection rabbitMqService, IServiceScopeFactory serviceScopeFactory) :
            base(rabbitMqService, serviceScopeFactory)
        {
        }

        protected override string QueueName => KeyRabbit.FORGOT_PASSWORD_QUEUE;

        protected override async Task ProcessMessageAsync(AppUser message, IServiceScope scope)
        {
            var forgotPasswordService = scope.ServiceProvider.GetRequiredService<IForgotPasswordService>();
            await forgotPasswordService.SendOtpResetPassword(message);
        }

    }
}
