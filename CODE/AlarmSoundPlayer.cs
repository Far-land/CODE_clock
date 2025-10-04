// 文件名: AlarmSoundPlayer.cs
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AlarmSoundPlayer : MonoBehaviour
{
    private static AlarmSoundPlayer _instance;
    private AudioSource audioSource;

    void Awake()
    {
        // 保证这个播放器全局唯一且永不销毁
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // 订阅闹钟事件
        if (AlarmManager.Instance != null)
        {
            AlarmManager.Instance.OnAlarmTriggered += PlayAlarmSound;
        }
    }

    void OnDestroy()
    {
        // 取消订阅
        if (AlarmManager.Instance != null)
        {
            AlarmManager.Instance.OnAlarmTriggered -= PlayAlarmSound;
        }
    }

    // 收到闹钟信号，就播放声音
    private void PlayAlarmSound(Alarm alarm)
    {
        if (MusicManager.Instance == null || LocationWeatherManager.Instance == null) return;

        var weatherData = LocationWeatherManager.Instance.LoadedWeatherData;
        AudioClip clipToPlay = MusicManager.Instance.GetContextualRingtone(alarm.GetTriggerTime(), weatherData);

        if (clipToPlay != null)
        {
            audioSource.clip = clipToPlay;
            audioSource.Play();
        }
    }

    // 停止声音
    public static void Stop()
    {
        if (_instance != null)
        {
            _instance.audioSource.Stop();
        }
    }
}