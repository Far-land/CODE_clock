// �ļ���: AlarmSoundPlayer.cs
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AlarmSoundPlayer : MonoBehaviour
{
    private static AlarmSoundPlayer _instance;
    private AudioSource audioSource;

    void Awake()
    {
        // ��֤���������ȫ��Ψһ����������
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
        // ���������¼�
        if (AlarmManager.Instance != null)
        {
            AlarmManager.Instance.OnAlarmTriggered += PlayAlarmSound;
        }
    }

    void OnDestroy()
    {
        // ȡ������
        if (AlarmManager.Instance != null)
        {
            AlarmManager.Instance.OnAlarmTriggered -= PlayAlarmSound;
        }
    }

    // �յ������źţ��Ͳ�������
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

    // ֹͣ����
    public static void Stop()
    {
        if (_instance != null)
        {
            _instance.audioSource.Stop();
        }
    }
}