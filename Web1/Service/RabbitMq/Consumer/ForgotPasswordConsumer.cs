using Web1.Helps;
using Web1.Models;
using Web1.Repository;
using Web1.Service.Account;
using Web1.Service.RabbitMq.Connection;
using Web1.Service.RabbitMq.Producer;

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
            //Gửi mã OTP quên mật khẩu cho User
            var forgotPasswordService = scope.ServiceProvider.GetRequiredService<IForgotPasswordService>();
            await forgotPasswordService.SendOtpResetPassword(message);

            //Tạo thông báo quên mật khẩu
            var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
            var dataNotify = await notificationRepository.CreatForgotPasswordNotify(message.Id);

            //Thông báo yêu cầu đổi mật khẩu cho user
            var producerService = scope.ServiceProvider.GetRequiredService<IRabbitMqProducer>();
            await producerService.PublishEvent(KeyRabbit.USER_NOTIFICATION_ROUTING, dataNotify);
        }
    }
}
