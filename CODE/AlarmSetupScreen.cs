using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class AlarmSetupScreen : MonoBehaviour
{
    [Header("��ҪUI����")]
    public TMP_Text hourText;
    public TMP_Text minuteText;
    public Button hourUpButton;
    public Button hourDownButton;
    public Button minuteUpButton;
    public Button minuteDownButton;
    public TMP_InputField labelInputField;
    public DayToggleButton[] dayToggles;
    public Button confirmButton;
    public Button cancelButton;
    public Button deleteButton;
    public TMP_Text dateDisplayText;
    public Button prevDayButton;
    public Button nextDayButton;

    // �ڲ�״̬����
    private int currentHour;
    private int currentMinute;
    private DateTime currentSelectedDate;
    private Alarm currentEditingAlarm;

    void Start()
    {
        // �󶨰�ť�¼�
        hourUpButton.onClick.AddListener(() => UpdateHour(1));
        hourDownButton.onClick.AddListener(() => UpdateHour(-1));
        minuteUpButton.onClick.AddListener(() => UpdateMinute(1));
        minuteDownButton.onClick.AddListener(() => UpdateMinute(-1));
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(Hide); // Cancel��ťֱ�ӵ���Hide
        deleteButton.onClick.AddListener(OnDelete);
        prevDayButton.onClick.AddListener(() => UpdateDate(-1));
        nextDayButton.onClick.AddListener(() => UpdateDate(1));

        // Ĭ�������Լ�
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ������Show��������UIManager����
    /// </summary>
    public void Show(Alarm alarmToEdit)
    {
        // ֱ�Ӽ��û�ж���
        gameObject.SetActive(true);

        // ���UI���߼�
        currentEditingAlarm = alarmToEdit;
        if (currentEditingAlarm == null)
        {
            ResetToNewAlarmState();
        }
        else // �༭ģʽ
        {
            currentHour = currentEditingAlarm.hour;
            currentMinute = currentEditingAlarm.minute;
            labelInputField.text = currentEditingAlarm.label;
            if (!DateTime.TryParse(currentEditingAlarm.specificDate, out currentSelectedDate))
            {
                currentSelectedDate = DateTime.Today;
            }
            foreach (var toggle in dayToggles)
            {
                toggle.IsSelected = currentEditingAlarm.repeatDays.Contains(toggle.day);
            }
            deleteButton.gameObject.SetActive(true);
        }
        UpdateVisuals();
    }

    /// <summary>
    /// ������Hide���������ڹر��Լ�
    /// </summary>
    public void Hide()
    {
        // ֱ�����أ�û�ж���
        gameObject.SetActive(false);
    }

    private void OnConfirm()
    {
        var alarmForCheck = new Alarm(currentHour, currentMinute, labelInputField.text);
        alarmForCheck.specificDate = currentSelectedDate.ToString("yyyy-MM-dd");
        foreach (var toggle in dayToggles)
        {
            if (toggle.IsSelected) alarmForCheck.repeatDays.Add(toggle.day);
        }

        bool isDuplicate = false;
        if (currentEditingAlarm != null)
        {
            currentEditingAlarm.isEnabled = false;
            if (AlarmManager.Instance.IsAlarmDuplicate(alarmForCheck))
            {
                isDuplicate = true;
            }
            currentEditingAlarm.isEnabled = true;
        }
        else
        {
            if (AlarmManager.Instance.IsAlarmDuplicate(alarmForCheck))
            {
                isDuplicate = true;
            }
        }

        if (isDuplicate)
        {
            Debug.LogWarning("�����ظ������ӣ�����ֹ���/���棡");
            return;
        }

        if (currentEditingAlarm == null) // �½�
        {
            alarmForCheck.ringtoneMode = RingtoneMode.System;
            alarmForCheck.ringtoneName = "ϵͳ�龳����";
            AlarmManager.Instance.AddAlarm(alarmForCheck);

            // �����󲻹رգ��������ý��棬�����������
            ResetToNewAlarmState();
            UpdateVisuals();
        }
        else // ����
        {
            currentEditingAlarm.hour = currentHour;
            currentEditingAlarm.minute = currentMinute;
            currentEditingAlarm.label = labelInputField.text;
            currentEditingAlarm.specificDate = currentSelectedDate.ToString("yyyy-MM-dd");
            currentEditingAlarm.repeatDays.Clear();
            foreach (var toggle in dayToggles) { if (toggle.IsSelected) currentEditingAlarm.repeatDays.Add(toggle.day); }
            currentEditingAlarm.ringtoneMode = RingtoneMode.System;

            AlarmManager.Instance.NotifyDataChanged();

            // ������ɺ󣬹ر�ҳ��
            Hide();
        }
    }

    private void OnCancel()
    {
        // Cancel��ťҲ����Hide
        Hide();
    }

    private void OnDelete()
    {
        if (currentEditingAlarm != null)
        {
            AlarmManager.Instance.DeleteAlarm(currentEditingAlarm.id);
            currentEditingAlarm = null;
        }
        // ɾ�������ý���
        ResetToNewAlarmState();
        UpdateVisuals();
    }

    private void ResetToNewAlarmState()
    {
        DateTime now = DateTime.Now;
        currentHour = now.Hour;
        currentMinute = now.Minute;
        currentSelectedDate = DateTime.Today;
        labelInputField.text = "";
        foreach (var toggle in dayToggles) { toggle.IsSelected = false; }
        deleteButton.gameObject.SetActive(false);
    }

    private void UpdateHour(int amount)
    {
        currentHour = (currentHour + amount + 24) % 24;
        UpdateVisuals();
    }
    private void UpdateMinute(int amount)
    {
        currentMinute = (currentMinute + amount + 60) % 60;
        UpdateVisuals();
    }
    private void UpdateDate(int daysToAdd)
    {
        currentSelectedDate = currentSelectedDate.AddDays(daysToAdd);
        UpdateVisuals();
    }
    private void UpdateVisuals()
    {
        hourText.text = currentHour.ToString("00");
        minuteText.text = currentMinute.ToString("00");
        dateDisplayText.text = currentSelectedDate.ToString("M��d�� dddd");
    }
}