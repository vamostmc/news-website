using Web1.Models;

namespace Web1.Service
{
    public interface ISendMailService 
    {
        public Task<Success> SetUpSendMail(string to, string subject, string body);

        
    }


}
