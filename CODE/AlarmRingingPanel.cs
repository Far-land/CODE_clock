using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

// 这个脚本不再需要Animator组件
[RequireComponent(typeof(AudioSource))]
public class AlarmRingingPanel : MonoBehaviour
{
    [Header("UI 引用")]
    public TextMeshProUGUI alarmLabelText;
    public TextMeshProUGUI currentTimeText;
    public Button dismissButton;
    public Button snoozeButton;

    private AudioSource audioSource;
    private Alarm currentRingingAlarm;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        dismissButton.onClick.AddListener(OnDismissClicked);
        snoozeButton.onClick.AddListener(OnSnoozeClicked);
    }

    /// <summary>
    /// UIManager调用的标准Show方法
    /// </summary>
    public void Show(Alarm alarm)
    {
        currentRingingAlarm = alarm;

        if (alarmLabelText != null)
        {
            alarmLabelText.text = currentRingingAlarm.label;
        }

        // 直接激活面板，不再播放动画
        gameObject.SetActive(true);

        PlayRingtone();
    }

    /// <summary>
    /// 【已修正】专门用于测试的Show方法，现在也不再有animator
    /// </summary>
    public void ShowWithSpecificClip(Alarm alarm, AudioClip specificClip)
    {
        currentRingingAlarm = alarm;

        if (alarmLabelText != null)
        {
            alarmLabelText.text = currentRingingAlarm.label;
        }

        // 直接激活面板
        gameObject.SetActive(true);

        // 直接播放我们传递进来的测试音频
        if (specificClip != null && audioSource != null)
        {
            audioSource.clip = specificClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("测试模式下未提供有效的AudioClip。");
        }
    }

    /// <summary>
    /// 隐藏面板的方法
    /// </summary>
    private void Hide()
    {
        if (audioSource != null) audioSource.Stop();
        gameObject.SetActive(false);
    }

    private void PlayRingtone()
    {
        if (MusicManager.Instance == null || LocationWeatherManager.Instance == null)
        {
            Debug.LogError("MusicManager 或 LocationWeatherManager 未准备就绪！");
            return;
        }

        var weatherData = LocationWeatherManager.Instance.LoadedWeatherData;
        AudioClip clipToPlay = MusicManager.Instance.GetContextualRingtone(currentRingingAlarm.GetTriggerTime(), weatherData);

        if (clipToPlay != null && audioSource != null)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("在响铃面板中找不到对应的铃声。");
        }
    }

    void Update()
    {
        if (currentTimeText != null)
        {
            currentTimeText.text = DateTime.Now.ToString("HH:mm");
        }
    }

    // --- 按钮响应 ---
    private void OnDismissClicked()
    {
        Hide();
    }

    private void OnSnoozeClicked()
    {
        if (currentRingingAlarm != null)
        {
            DateTime snoozeTime = AlarmManager.Instance.CurrentTime.AddMinutes(9);
            Alarm snoozeAlarm = new Alarm(snoozeTime.Hour, snoozeTime.Minute, currentRingingAlarm.label + " (稍后提醒)");
            snoozeAlarm.specificDate = snoozeTime.ToString("yyyy-MM-dd");
            AlarmManager.Instance.AddAlarm(snoozeAlarm);
        }
        Hide();
    }
}