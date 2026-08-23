using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>
{
    public TimeSpan serverTimeModifier = TimeSpan.FromHours(8);
    public long GetCurrentUnixtimestamp()
    {
        DateTime utcTime = DateTime.UtcNow.Add(serverTimeModifier);
        return ((DateTimeOffset)utcTime).ToUnixTimeSeconds();
    }

    public DateTime GetCurrentDatetime()
    {
        return DateTime.UtcNow.Add(serverTimeModifier);
    }

    public DateTime UnixTimeStamp2DateTime(long unixTimeStamp)
    {

        return DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp).UtcDateTime;
    }

    public int DiffDayWithToday(long timestamp)
    {

        return UnixTimeStamp2DateTime(timestamp).Subtract(DateTime.MinValue).Days - DateTime.Now.Subtract(DateTime.MinValue).Days;
    }

    public bool IsToday(long timestamp)
    {
        return DiffDayWithToday(timestamp) == 0;
    }

    public bool IsPass(long timestamp)
    {
        return DiffDayWithToday(timestamp) < 0;
    }

    public bool IsFuture(long timestamp)
    {
        return DiffDayWithToday(timestamp) > 0;
    }

    public bool IsWithinTimeRange(long startTime, long endTime)
    {
        return GetCurrentUnixtimestamp() >= startTime && GetCurrentUnixtimestamp() <= endTime;
    }
}