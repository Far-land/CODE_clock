using UnityEngine;
using System.Collections;

/// <summary>
/// UI总管 (单例)。
/// 整个应用中唯一的UI调度中心，负责所有主UI面板的显示和隐藏。
/// </summary>
public class UIManager : MonoBehaviour
{
    // 创建一个全局唯一的静态实例
    public static UIManager Instance { get; private set; }

    [Header("UI面板脚本的引用")]
    public AlarmSetupScreen alarmSetupScreen;
    public AlarmRingingPanel alarmRingingPanel;
    public ThemeSelectionPanel themeSelectionPanel;

    void Awake()
    {
        // 设置单例模式
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 确保所有面板在启动时都是关闭的
        if (alarmSetupScreen != null) alarmSetupScreen.gameObject.SetActive(false);
        if (alarmRingingPanel != null) alarmRingingPanel.gameObject.SetActive(false);
        if (themeSelectionPanel != null) themeSelectionPanel.gameObject.SetActive(false);
    }

    void Start()
    {
        // 监听“闹钟触发”事件
        if (AlarmManager.Instance != null)
        {
            AlarmManager.Instance.OnAlarmTriggered += OnAlarmTriggered;
        }
    }

    void OnDestroy()
    {
        // 取消事件订阅
        if (AlarmManager.Instance != null)
        {
            AlarmManager.Instance.OnAlarmTriggered -= OnAlarmTriggered;
        }
    }

    /// <summary>
    /// 当闹钟触发时，执行此方法
    /// </summary>
    private void OnAlarmTriggered(Alarm triggeredAlarm)
    {
        if (alarmRingingPanel != null)
        {
            alarmRingingPanel.Show(triggeredAlarm);
        }
    }

    /// <summary>
    /// 这是打开闹钟设置界面的唯一入口
    /// </summary>
    public void ShowAlarmSetupScreen(Alarm alarmToEdit)
    {
        if (alarmSetupScreen != null)
        {
            // 由UIManager自己启动协程
            StartCoroutine(ShowAlarmSetupScreenRoutine(alarmToEdit));
        }
    }

    /// <summary>
    /// 打开主题选择界面
    /// </summary>
    public void ShowThemeSelectionPanel()
    {
        if (themeSelectionPanel != null && ThemeManager.Instance != null)
        {
            themeSelectionPanel.Show(ThemeManager.Instance.availableThemes);
        }
    }

    /// <summary>
    /// 【已修正】处理显示流程的协程，确保时机和参数正确
    /// </summary>
    private IEnumerator ShowAlarmSetupScreenRoutine(Alarm alarmToEdit)
    {
        // 1. UIManager负责“唤醒”面板
        alarmSetupScreen.gameObject.SetActive(true);

        // 2.【关键】等待一帧，给面板上的Animator一点反应时间
        yield return null;

        // 3. 现在面板已完全准备好，再命令它执行Show逻辑，并传递正确的参数
        alarmSetupScreen.Show(alarmToEdit);
    }
}