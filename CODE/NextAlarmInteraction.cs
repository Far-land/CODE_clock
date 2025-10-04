using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class NextAlarmInteraction : MonoBehaviour, IPointerClickHandler
{
    [Header("UI 引用")]
    public TextMeshProUGUI nextAlarmText;

    // --- 我们不再需要任何用于检测双击的内部变量 ---
    // private float lastClickTime;
    // private int clickCount;

    // --- 在Start/OnDestroy中订阅和取消订阅事件 (保持不变) ---
    void Start()
    {
        if (AlarmManager.Instance != null)
        {
            AlarmManager.Instance.OnAlarmListChanged += UpdateNextAlarmDisplay;
        }
        UpdateNextAlarmDisplay();
    }
    void OnDestroy()
    {
        if (AlarmManager.Instance != null)
        {
            AlarmManager.Instance.OnAlarmListChanged -= UpdateNextAlarmDisplay;
        }
    }

    /// <summary>
    /// 【核心修复】当用户点击这个UI元素时，此方法被调用
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 直接使用Unity为我们统计好的连续点击次数
        // 这个值由引擎管理，绝对可靠，没有初始化问题
        if (eventData.clickCount == 2)
        {
            Debug.Log("Unity EventSystem 检测到双击！命令UI总管打开设置界面。");

            // 执行打开页面的操作
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowAlarmSetupScreen(null);
            }
        }
    }

    // --- 显示逻辑 (完全不变) ---
    private void UpdateNextAlarmDisplay()
    {
        if (nextAlarmText == null || AlarmManager.Instance == null || UserManager.Instance == null) return;

        var nextAlarmInfo = AlarmManager.Instance.GetNextUpcomingAlarm();

        if (nextAlarmInfo == null)
        {
            nextAlarmText.text = "暂未设置闹钟";
        }
        else
        {
            DateTime triggerTime = nextAlarmInfo.Value.triggerTime.ToLocalTime();
            DateTime today = AlarmManager.Instance.CurrentTime.ToLocalTime().Date;
            DateTime tomorrow = today.AddDays(1);

            string prefix = "";
            if (triggerTime.Date == today) prefix = "今天";
            else if (triggerTime.Date == tomorrow) prefix = "明天";
            else prefix = triggerTime.ToString("M月d日");

            string timeString;
            if (UserManager.Instance.CurrentUser.use24HourFormat)
            {
                timeString = triggerTime.ToString("HH:mm");
            }
            else
            {
                timeString = triggerTime.ToString("hh:mm tt");
            }

            nextAlarmText.text = $"下一个闹钟: {prefix} {timeString}";
        }
    }
}