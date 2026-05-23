using System;
using System.Collections.Generic;
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
    /// Lấy ngày giờ hiện tại với format tùy chỉnh.
    /// Mặc định: dd/MM/yyyy HH:mm:ss
    /// </summary>
    public static string GetCurrentDateTime(bool includeHours = true, bool includeMinutes = true, bool includeSeconds = true)
    {
        string format = "dd/MM/yyyy";
        
        if (includeHours)
            format += " HH";
        if (includeMinutes)
            format += ":mm";
        if (includeSeconds)
            format += ":ss";
            
        return System.DateTime.Now.ToString(format);
    }
    public static string FormatRemainingTime(long endTime)
    {
        // Sửa lại cú pháp lấy Unix Timestamp (giây) hiện tại chuẩn C#
        long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long remainingSeconds = endTime - now;

        if (remainingSeconds <= 0)
        {
            return "0 giây"; // Hoặc "Đã hoàn thành" tùy bạn muốn
        }

        return FormatDuration(remainingSeconds);
    }

    /// <summary>
    /// Đổi một số giây bất kỳ ra chuỗi Ngày/Giờ/Phút/Giây, tự động ẩn đơn vị nếu bằng 0
    /// </summary>
    public static string FormatDuration(double seconds)
    {
        if (seconds <= 0) return "0 giây";

        TimeSpan time = TimeSpan.FromSeconds(seconds);
        List<string> parts = new List<string>();

        // Kiểm tra từng thành phần, cái nào lớn hơn 0 thì mới thêm vào danh sách
        if (time.Days > 0) parts.Add($"{time.Days} ngày");
        if (time.Hours > 0) parts.Add($"{time.Hours} giờ");
        if (time.Minutes > 0) parts.Add($"{time.Minutes} phút");
        if (time.Seconds > 0) parts.Add($"{time.Seconds} giây");

        // Nếu tất cả đều bằng 0 (ví dụ: 0.4 giây làm tròn xuống), trả về "0 giây"
        if (parts.Count == 0) return "0 giây";

        // Nối các phần tử lại với nhau bằng dấu phẩy và khoảng trắng ", "
        return string.Join(", ", parts);
    }
}