using UnityEngine;
using System;
using System.Linq;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("���ֿ��ʲ�")]
    public ContextualMusicLibrary contextualMusicLibrary;
    public HolidayMusicLibrary holidayMusicLibrary;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// ��������ʱ�����������ȡ����Ӧ�ò��ŵ�����
    /// </summary>
    public AudioClip GetContextualRingtone(DateTime alarmTime, WeatherData weatherData)
    {
        // 1. ���ȼ�����ӵ����ǲ���һ����֪���������
        string holidayName = HolidayManager.Instance?.GetHolidayNameForDate(alarmTime);

        if (!string.IsNullOrEmpty(holidayName))
        {
            Debug.Log($"���������� {holidayName}�������Բ���ͨ�õĽ������֡�");

            // �������޸�������GetHolidayRingtoneʱ��������Ҫ����holidayName����
            AudioClip holidayClip = GetHolidayRingtone(alarmTime, weatherData);

            if (holidayClip != null) return holidayClip;
        }

        // 2. ������ǽ��գ�����û�ҵ��������֣���ִ�г����߼�
        return GetNormalRingtone(alarmTime, weatherData);
    }

    /// <summary>
    /// ˽�и�����������ȡͨ�õĽ�������
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
    /// ˽�и�����������ȡ������龳���� (�˷������ֲ���)
    /// </summary>
    private AudioClip GetNormalRingtone(DateTime alarmTime, WeatherData weatherData)
    {
        if (contextualMusicLibrary == null)
        {
            Debug.LogError("ContextualMusicLibraryδ��MusicManager��Inspector�����ã�");
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
    /// ˽�и����������������ַ�������Ϊ���ǵ�ö�� (�˷������ֲ���)
    /// </summary>
    private WeatherType ParseWeather(WeatherData data)
    {
        if (data == null || data.forecasts == null || data.forecasts.Count == 0) return WeatherType.Default;
        string weatherString = data.forecasts[0].dayweather;
        if (weatherString.Contains("ѩ")) return WeatherType.Snowy;
        if (weatherString.Contains("��") || weatherString.Contains("��") || weatherString.Contains("��")) return WeatherType.Rainy;
        if (weatherString.Contains("��") || weatherString.Contains("��") || weatherString.Contains("��") || weatherString.Contains("��")) return WeatherType.Sunny;
        return WeatherType.Default;
    }
}