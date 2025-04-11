using Web1.Data;
using Web1.Models;

namespace Web1.Repository
{
    public interface IMessageRepository
    {
        public Task<MessageDto> SaveMessageAsync(MessageDto message);

        public Task<List<MessageDto>> GetMessageAsync(string userId, int sizeMessage, DateTime? before = null);

        public Task<ConversationDto> CreateConversation(ConversationDto conversation);

        public Task<Success> GetConversationAsync(string id);

        public Task DeleteConversationAsync(string id);

        public Task<List<ConversationDto>> GetAllConverAsync();
    }
}
