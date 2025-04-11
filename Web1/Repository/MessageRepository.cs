using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web1.Data;
using Web1.Exceptions;
using Web1.Models;

namespace Web1.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly TinTucDbContext _messContext;
        private readonly UserManager<AppUser> _userManager;

        public MessageRepository(TinTucDbContext messContext, UserManager<AppUser> userManager) 
        {
            _messContext = messContext;
            _userManager = userManager;
        }

        public async Task<MessageDto> SaveMessageAsync(MessageDto message)
        {
            var data = await _messContext.Conversations.FirstOrDefaultAsync(t => t.IsActive == true && t.Id == message.ConversationId);
            if (data == null)
            {
                throw new RepositoryException("Lỗi không tìm thấy hộp thoại tin nhắn hoặc hộp thoại không còn hoạt động");
            }
            var messageNew = new Message
            {
                Text = message.Text,
                ConversationId = message.ConversationId,
                Id = message.Id,
                IsRead = message.IsRead,
                ReceiverId = message.ReceiverId,
                SenderId = message.SenderId,
                SentAt = DateTime.Now,
                Status = "active",
            };

            _messContext.Messages.Add(messageNew);
            await _messContext.SaveChangesAsync();

            return message;
        }

        public async Task<ConversationDto> CreateConversation(ConversationDto conversation)
        {
            var dataNew = new Conversation
            {
                Id = conversation.Id,
                IsActive = conversation.IsActive,
                UserId = conversation.UserId,
            };
            await _messContext.AddAsync(dataNew);
            await _messContext.SaveChangesAsync();
            return new ConversationDto
            {
                Id = dataNew.Id, 
                IsActive = dataNew.IsActive,
                UserId = dataNew.UserId
            };
        }

        public async Task DeleteConversationAsync(string id)
        {
            var user = await _messContext.Conversations.FirstOrDefaultAsync(t => t.UserId == id);
            if (user == null)
            {
                throw new RepositoryException("Lỗi không tìm thấy hộp thoại cần xóa của user");
            }
            _messContext.Remove(user);
            await _messContext.SaveChangesAsync();
        }

        public async Task<Success> GetConversationAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return new Success
                {
                    success = false,
                    message = "Thiếu userId"
                };
            }

            var user = await _messContext.Conversations.FirstOrDefaultAsync(t => t.UserId == id);
            if (user == null)
            {
                return new Success { success = false, message = "Chưa có tin nhắn của người dùng này" };  
            }
            return new Success { success = true, message = user.Id.ToString() };
        }

        public async Task<List<MessageDto>> GetMessageAsync(string userId, int sizeMessage, DateTime? before = null)
        {
            var dataConversation = await _messContext.Conversations
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (dataConversation == null)
            {
                throw new RepositoryException($"Không tìm thấy hộp thoại của {userId}");
            }

            var query = _messContext.Messages
                .Where(m => m.ConversationId == dataConversation.Id);

            if (before.HasValue)
            {
                // Lọc những tin nhắn cũ hơn thời điểm "before"
                query = query.Where(m => m.SentAt < before.Value);
            }

            var messageUser = await query
                .OrderByDescending(m => m.SentAt) // Mới nhất trước
                .Take(sizeMessage)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    ConversationId = m.ConversationId,
                    SenderId = m.SenderId,
                    ReceiverId = m.ReceiverId,
                    Text = m.Text,
                    SentAt = m.SentAt,
                    IsRead = m.IsRead,
                    Status = m.Status
                })
                .ToListAsync();

            return messageUser;
        }


        public async Task<List<ConversationDto>> GetAllConverAsync()
        {
            // Bước 1: Lấy danh sách conversation
            var conversations = await _messContext.Conversations
                .Select(convo => new ConversationDto
                {
                    Id = convo.Id,
                    UserId = convo.UserId,
                    IsActive = convo.IsActive,
                    LastMessage = _messContext.Messages
                        .Where(m => m.ConversationId == convo.Id)
                        .OrderByDescending(m => m.SentAt)
                        .Select(m => m.Text)
                        .FirstOrDefault()
                })
                .ToListAsync();

            // Bước 2: Lấy danh sách UserId duy nhất
            var userIds = conversations.Select(c => c.UserId).Distinct().ToList();

            // Bước 3: Dùng UserManager để lấy danh sách user
            var users = new List<AppUser>();
            foreach (var userId in userIds)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                    users.Add(user);
            }

            // Bước 4: Gán UserName vào từng conversation
            foreach (var convo in conversations)
            {
                var user = users.FirstOrDefault(u => u.Id == convo.UserId);
                convo.UserName = user?.UserName;
            }

            return conversations;
        }


    }
}
