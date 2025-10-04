using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

// ����ű�������ҪAnimator���
[RequireComponent(typeof(AudioSource))]
public class AlarmRingingPanel : MonoBehaviour
{
    [Header("UI ����")]
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
    /// UIManager���õı�׼Show����
    /// </summary>
    public void Show(Alarm alarm)
    {
        currentRingingAlarm = alarm;

        if (alarmLabelText != null)
        {
            alarmLabelText.text = currentRingingAlarm.label;
        }

        // ֱ�Ӽ�����壬���ٲ��Ŷ���
        gameObject.SetActive(true);

        PlayRingtone();
    }

    /// <summary>
    /// ����������ר�����ڲ��Ե�Show����������Ҳ������animator
    /// </summary>
    public void ShowWithSpecificClip(Alarm alarm, AudioClip specificClip)
    {
        currentRingingAlarm = alarm;

        if (alarmLabelText != null)
        {
            alarmLabelText.text = currentRingingAlarm.label;
        }

        // ֱ�Ӽ������
        gameObject.SetActive(true);

        // ֱ�Ӳ������Ǵ��ݽ����Ĳ�����Ƶ
        if (specificClip != null && audioSource != null)
        {
            audioSource.clip = specificClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("����ģʽ��δ�ṩ��Ч��AudioClip��");
        }
    }

    /// <summary>
    /// �������ķ���
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
            Debug.LogError("MusicManager �� LocationWeatherManager δ׼��������");
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
            Debug.LogWarning("������������Ҳ�����Ӧ��������");
        }
    }

    void Update()
    {
        if (currentTimeText != null)
        {
            currentTimeText.text = DateTime.Now.ToString("HH:mm");
        }
    }

    // --- ��ť��Ӧ ---
    private void OnDismissClicked()
    {
        Hide();
    }

    private void OnSnoozeClicked()
    {
        if (currentRingingAlarm != null)
        {
            DateTime snoozeTime = AlarmManager.Instance.CurrentTime.AddMinutes(9);
            Alarm snoozeAlarm = new Alarm(snoozeTime.Hour, snoozeTime.Minute, currentRingingAlarm.label + " (�Ժ�����)");
            snoozeAlarm.specificDate = snoozeTime.ToString("yyyy-MM-dd");
            AlarmManager.Instance.AddAlarm(snoozeAlarm);
        }
        Hide();
    }
}