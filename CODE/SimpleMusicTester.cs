using UnityEngine;
using UnityEngine.UI;
using System;

public class SimpleMusicTester : MonoBehaviour
{
    [Header("--- ����������������Ե����� ---")]

    [Tooltip("��ʽ: YYYY-MM-DD, ����: 2025-01-01")]
    public string testDate = "2025-01-01"; // Ԫ��

    [Range(0, 23)]
    public int testHour = 8; // ����8��

    [Range(0, 59)]
    public int testMinute = 0;

    public WeatherType testWeather = WeatherType.Sunny;

    [Header("--- UI ---")]
    public Button triggerTestButton; // ����һ�����ڴ������Եİ�ť

    void Start()
    {
        // ֻ��Ҫ�󶨰�ť�¼�
        if (triggerTestButton != null)
        {
            triggerTestButton.onClick.AddListener(TriggerTestAlarm);
        }
        else
        {
            Debug.LogError("���԰�ť��δ��Inspector�����ã�");
        }
    }

    /// <summary>
    /// �����ťʱ��ִ�д˷���
    /// </summary>
    public void TriggerTestAlarm()
    {
        if (!DateTime.TryParse(testDate, out DateTime parsedDate))
        {
            Debug.LogError("����ʧ�ܣ����ڸ�ʽ����ȷ�������� yyyy-MM-dd");
            return;
        }
        DateTime testAlarmTime = new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, testHour, testMinute, 0);

        Alarm testAlarm = new Alarm(testHour, testMinute, $"����: {testDate} {testWeather}");
        testAlarm.specificDate = parsedDate.ToString("yyyy-MM-dd");

        string testWeatherString = "";
        switch (testWeather)
        {
            case WeatherType.Sunny: testWeatherString = "��"; break;
            case WeatherType.Rainy: testWeatherString = "��"; break;
            case WeatherType.Snowy: testWeatherString = "ѩ"; break;
            default: testWeatherString = "����"; break;
        }
        WeatherData testWeatherData = new WeatherData();
        testWeatherData.forecasts.Add(new DailyForecast { dayweather = testWeatherString });

        Debug.Log($"--- ��ʼ���ԣ�ʱ��={testAlarmTime}, ����={testWeatherString} ---");

        // �������޸ġ����ǲ���ֱ�ӵ���MusicManager������ģ����������������
        // ��������ȷ������������������޸ĵģ����������
        if (UIManager.Instance != null && UIManager.Instance.alarmRingingPanel != null)
        {
            // ֱ���������������ʾ���������Լ�ȥ��ȡ����
            // ��ᴥ�����Ǹո��޸ĵ��������߼�
            UIManager.Instance.alarmRingingPanel.Show(testAlarm);
        }
    }
}