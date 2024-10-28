using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq.Expressions;
namespace Web1.Helps
{
    public class LocalTime
    {
        //Lấy giá trị ngày và giờ tại thời điểm hiện tại
        public static DateTime GetLocalTime()
        {
            TimeZoneInfo hanoiBangkokTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            DateTime utcNow = DateTime.UtcNow; // Thời gian UTC hiện tại
            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, hanoiBangkokTimeZone);
            return localTime;
        }
    }
}
