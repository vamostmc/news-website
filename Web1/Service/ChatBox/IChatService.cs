using Web1.Data;
using Web1.Models;

namespace Web1.Service.ChatBox
{
    public interface IChatService
    {
        public Task SendMessageAsync(MessageDto message);
    }
}
