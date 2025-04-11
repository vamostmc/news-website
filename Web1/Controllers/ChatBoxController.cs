using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web1.Data;
using Web1.Models;
using Web1.Repository;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatBoxController : ControllerBase
    {
        private readonly IMessageRepository _messageRepo;
        public ChatBoxController(IMessageRepository messageRepo)
        {
            _messageRepo = messageRepo;
        }

        [HttpGet("MessageAll/{id}")]
        public async Task<List<MessageDto>> GetMessagesToUser(string id, [FromQuery] int limit, [FromQuery] DateTime? beforeTime = null) 
        {
            return await _messageRepo.GetMessageAsync(id, limit, beforeTime);
        }

        [HttpPost("CreateConver")] 
        public async Task<ConversationDto> CreateConversation(ConversationDto conver)
        {
            var data = await _messageRepo.CreateConversation(conver);
            return data;
        }

        [HttpDelete("DeleteConver")]
        public async Task DeleteConversation(string UserId)
        {
            await _messageRepo.DeleteConversationAsync(UserId);
        }

        [HttpGet("CheckCover/{UserId}")]
        public async Task<Success> GetConversation(string UserId)
        {
            return await _messageRepo.GetConversationAsync(UserId);
        }

        [HttpGet("GetAllConversation")]
        public async Task<List<ConversationDto>> GetAllConversationToAdmin() 
        {
            return await _messageRepo.GetAllConverAsync();
        }

        [HttpPost("AddMessage")]
        public async Task<MessageDto> AddMessage(MessageDto message) 
        {
            return await _messageRepo.SaveMessageAsync(message);
        }

    }
}
