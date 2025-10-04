using UnityEngine;
using UnityEngine.UI;
using System;

public class SimpleMusicTester : MonoBehaviour
{
    [Header("--- 在这里输入您想测试的条件 ---")]

    [Tooltip("格式: YYYY-MM-DD, 例如: 2025-01-01")]
    public string testDate = "2025-01-01"; // 元旦

    [Range(0, 23)]
    public int testHour = 8; // 早上8点

    [Range(0, 59)]
    public int testMinute = 0;

    public WeatherType testWeather = WeatherType.Sunny;

    [Header("--- UI ---")]
    public Button triggerTestButton; // 拖入一个用于触发测试的按钮

    void Start()
    {
        // 只需要绑定按钮事件
        if (triggerTestButton != null)
        {
            triggerTestButton.onClick.AddListener(TriggerTestAlarm);
        }
        else
        {
            Debug.LogError("测试按钮尚未在Inspector中设置！");
        }
    }

    /// <summary>
    /// 点击按钮时，执行此方法
    /// </summary>
    public void TriggerTestAlarm()
    {
        if (!DateTime.TryParse(testDate, out DateTime parsedDate))
        {
            Debug.LogError("测试失败：日期格式不正确！请输入 yyyy-MM-dd");
            return;
        }
        DateTime testAlarmTime = new DateTime(parsedDate.Year, parsedDate.Month, parsedDate.Day, testHour, testMinute, 0);

        Alarm testAlarm = new Alarm(testHour, testMinute, $"测试: {testDate} {testWeather}");
        testAlarm.specificDate = parsedDate.ToString("yyyy-MM-dd");

        string testWeatherString = "";
        switch (testWeather)
        {
            case WeatherType.Sunny: testWeatherString = "晴"; break;
            case WeatherType.Rainy: testWeatherString = "雨"; break;
            case WeatherType.Snowy: testWeatherString = "雪"; break;
            default: testWeatherString = "多云"; break;
        }
        WeatherData testWeatherData = new WeatherData();
        testWeatherData.forecasts.Add(new DailyForecast { dayweather = testWeatherString });

        Debug.Log($"--- 开始测试：时间={testAlarmTime}, 天气={testWeatherString} ---");

        // 【核心修改】我们不再直接调用MusicManager，而是模拟完整的响铃流程
        // 这样可以确保所有组件（包括新修改的）都参与测试
        if (UIManager.Instance != null && UIManager.Instance.alarmRingingPanel != null)
        {
            // 直接命令响铃面板显示，并让它自己去获取音乐
            // 这会触发我们刚刚修改的所有新逻辑
            UIManager.Instance.alarmRingingPanel.Show(testAlarm);
        }
    }
}