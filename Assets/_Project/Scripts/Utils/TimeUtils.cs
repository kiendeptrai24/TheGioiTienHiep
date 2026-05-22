using Unity.Netcode;
using UnityEngine;

public static class TimeUtils
{
    public static double GetServerTime()
    {
        if (NetworkManager.Singleton == null)
            return 0;
        return NetworkManager.Singleton.ServerTime.Time;
    }
    public static long DateTimeOffset(long seconds = 0)
    {
        return System.DateTimeOffset.UtcNow.AddSeconds(seconds).ToUnixTimeSeconds();
    }
    public static double GetLocalTime()
    {
        if (NetworkManager.Singleton == null)
            return 0;
        return NetworkManager.Singleton.LocalTime.Time;
    }

    public static float GetUnityTime()
    {
        return Time.time;
    }
    // ==================== CÁC HÀM CONVERT THỜI GIAN THÊM VÀO ====================

    /// <summary>
    /// Truyền vào mốc Unix EndTime, tự động tính toán với thời gian thực hiện tại 
    /// và trả về chuỗi định dạng: "X ngày, Y giờ, Z phút, W giây"
    /// </summary>
    public static string FormatRemainingTime(long endTime)
    {
        long now = DateTimeOffset();
        long remainingSeconds = endTime - now;

        if (remainingSeconds <= 0)
        {
            return "0 ngày, 0 giờ, 0 phút, 0 giây";
        }

        return FormatDuration(remainingSeconds);
    }

    /// <summary>
    /// Đổi một số giây bất kỳ (double hoặc long) ra chuỗi hiển thị Ngày/Giờ/Phút/Giây gọn gàng
    /// </summary>
    public static string FormatDuration(double seconds)
    {
        if (seconds <= 0) return "0 ngày, 0 giờ, 0 phút, 0 giây";

        System.TimeSpan time = System.TimeSpan.FromSeconds(seconds);
        
        // Trả về chuỗi đẹp mắt, bạn có thể tự sửa chữ "ngày, giờ..." theo ý muốn
        return $"{time.Days} ngày, {time.Hours} giờ, {time.Minutes} phút, {time.Seconds} giây";
    }
}