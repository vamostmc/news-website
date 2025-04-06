using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
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
        private readonly TinTucDbContext _context;

        public NotificationController(INotificationRepository noti,
                                      TinTucDbContext context) { _noti = noti; _context = context; }

        [HttpGet("GetNoti")]
        public async Task<IActionResult> GetAllNotifications()
        {
            var notifications = await _context.Notifications
                .ToListAsync();
            return Ok(notifications);
        }

        [HttpGet("GetNotify/{id}")]
        public async Task<List<NotificationDto>> GetNotifyTinTuc(string id) 
        {
            return await _noti.GetNotifyUser(id);
        }


        [HttpGet("TotalNotify/{id}")]
        public async Task<int> GetTotalNotify(string id)
        {
            return await _noti.TotalNotify(id);
        }

        [HttpDelete("DeleteNotify/{id}")]
        public async Task<Success> DeleteNotification(long id)
        {
            return await _noti.DeleteNotify(id);
        }

        [HttpPut("UpdateStatusReadId/{id}")]
        public Task<Success> UpdateStatusId(long id, [FromBody] bool statusRead)
        {
            return _noti.UpdateReadStatusId(id,statusRead);
        }

        [HttpPut("UpdateStatusAll/{userId}")]
        public Task<Success> UpdateStatusAll(string userId, [FromBody] bool statusRead)
        {
            return _noti.UpdateAllRead(userId,statusRead);
        }

    }
}
