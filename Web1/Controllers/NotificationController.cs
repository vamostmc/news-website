using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web1.Data;
using Web1.Models;
using Web1.Repository;

namespace Web1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _noti;

        public NotificationController(INotificationRepository noti ) { _noti = noti; }

        [HttpGet("GetNotify/{idNotify}")]
        public async Task<List<NotificationDto>> GetNotifyTinTuc(int idNotify) 
        {
           return await _noti.GetNotify(idNotify);
        }

        [HttpGet("GetNotification/{id}")]
        public async Task<Notification> GetNotification(int id)
        {
            return await _noti.GetNotificationRepo(id);
        }

        [HttpPost("AddNotificationTinTuc")]
        public async Task<NotificationDto> AddNotificationTinTuc([FromBody] NotificationDto newNoti)
        {
            return await _noti.AddNotifyTinTucRepo(newNoti);
        }

        [HttpPost("AddNotificationBinhLuan")]
        public async Task<NotifyBinhLuan> AddNotificationBinhLuan([FromBody] NotifyBinhLuan notifyBinhLuan)
        {
            return await _noti.AddNotifyBinhLuanRepo(notifyBinhLuan);
        }

        [HttpDelete("RemoveNotification")]
        public async Task<IActionResult> DeleteNotification(int id) 
        {
            await _noti.RemoveNotificationRepo(id);
            return Ok("OK");
        }

        //Update Notify
        [HttpPut("EditNoti")]
        public async Task<IActionResult> UpdateNotification(int id) 
        {
            await _noti.UpdateNotificationRepo(id);
            return Ok("OK");
        }

    }
}
