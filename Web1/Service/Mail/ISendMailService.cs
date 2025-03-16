using Web1.Models;

namespace Web1.Service.Mail
{
    public interface ISendMailService
    {
        public Task<Success> SetUpSendMail(string to, string subject, string body);


    }


}
