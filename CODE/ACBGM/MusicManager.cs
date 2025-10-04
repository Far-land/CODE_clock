using UnityEngine;
using System;
using System.Linq;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("音乐库资产")]
    public ContextualMusicLibrary contextualMusicLibrary;
    public HolidayMusicLibrary holidayMusicLibrary;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 根据闹钟时间和天气，获取最终应该播放的铃声
    /// </summary>
    public AudioClip GetContextualRingtone(DateTime alarmTime, WeatherData weatherData)
    {
        // 1. 优先检查闹钟当天是不是一个已知的特殊节日
        string holidayName = HolidayManager.Instance?.GetHolidayNameForDate(alarmTime);

        if (!string.IsNullOrEmpty(holidayName))
        {
            Debug.Log($"闹钟日期是 {holidayName}！将尝试播放通用的节日音乐。");

            // 【核心修复】调用GetHolidayRingtone时，不再需要传递holidayName参数
            AudioClip holidayClip = GetHolidayRingtone(alarmTime, weatherData);

            if (holidayClip != null) return holidayClip;
        }

        // 2. 如果不是节日，或者没找到节日音乐，则执行常规逻辑
        return GetNormalRingtone(alarmTime, weatherData);
    }

    /// <summary>
    /// 私有辅助方法：获取通用的节日音乐
    /// </summary>
    private AudioClip GetHolidayRingtone(DateTime alarmTime, WeatherData weatherData)
    {
        if (holidayMusicLibrary == null) return null;

        WeatherType weather = ParseWeather(weatherData);
        bool isDayTime = alarmTime.Hour >= 6 && alarmTime.Hour < 18;

        switch (weather)
        {
            case WeatherType.Sunny:
                return isDayTime ? holidayMusicLibrary.sunnyDay : holidayMusicLibrary.sunnyNight;
            case WeatherType.Rainy:
                return isDayTime ? holidayMusicLibrary.rainyDay : holidayMusicLibrary.rainyNight;
            case WeatherType.Snowy:
                return isDayTime ? holidayMusicLibrary.snowyDay : holidayMusicLibrary.snowyNight;
            default:
                return isDayTime ? holidayMusicLibrary.sunnyDay : holidayMusicLibrary.sunnyNight;
        }
    }

    /// <summary>
    /// 私有辅助方法：获取常规的情境音乐 (此方法保持不变)
    /// </summary>
    private AudioClip GetNormalRingtone(DateTime alarmTime, WeatherData weatherData)
    {
        if (contextualMusicLibrary == null)
        {
            Debug.LogError("ContextualMusicLibrary未在MusicManager的Inspector中设置！");
            return null;
        }

        WeatherType currentActivity = ParseWeather(weatherData);
        TimeOfDay timeOfDay;
        int hour12 = alarmTime.Hour;

        if (alarmTime.Hour == 12 && alarmTime.Minute < 30)
        {
            timeOfDay = TimeOfDay.Noon;
        }
        else if (alarmTime.Hour == 0 && alarmTime.Minute < 30)
        {
            timeOfDay = TimeOfDay.Midnight;
        }
        else if (alarmTime.Hour >= 12)
        {
            timeOfDay = TimeOfDay.PM;
            if (hour12 > 12) hour12 -= 12;
        }
        else
        {
            timeOfDay = TimeOfDay.AM;
            if (hour12 == 0) hour12 = 12;
        }

        ContextualMusicTrack bestMatch = contextualMusicLibrary.contextualTracks
            .FirstOrDefault(track => track.weather == currentActivity &&
                                      track.timeOfDay == timeOfDay &&
                                      (timeOfDay == TimeOfDay.AM || timeOfDay == TimeOfDay.PM ? track.hour == hour12 : true));

        if (bestMatch == null)
        {
            bestMatch = contextualMusicLibrary.contextualTracks
                .FirstOrDefault(track => track.weather == WeatherType.Default &&
                                          track.timeOfDay == timeOfDay &&
                                          (timeOfDay == TimeOfDay.AM || timeOfDay == TimeOfDay.PM ? track.hour == hour12 : true));
        }

        if (bestMatch != null && bestMatch.audioClip != null)
        {
            return bestMatch.audioClip;
        }
        else
        {
            return contextualMusicLibrary.defaultRingtone;
        }
    }

    /// <summary>
    /// 私有辅助方法：将天气字符串解析为我们的枚举 (此方法保持不变)
    /// </summary>
    private WeatherType ParseWeather(WeatherData data)
    {
        if (data == null || data.forecasts == null || data.forecasts.Count == 0) return WeatherType.Default;
        string weatherString = data.forecasts[0].dayweather;
        if (weatherString.Contains("雪")) return WeatherType.Snowy;
        if (weatherString.Contains("雨") || weatherString.Contains("雷") || weatherString.Contains("雹")) return WeatherType.Rainy;
        if (weatherString.Contains("晴") || weatherString.Contains("云") || weatherString.Contains("阴") || weatherString.Contains("风")) return WeatherType.Sunny;
        return WeatherType.Default;
    }
}