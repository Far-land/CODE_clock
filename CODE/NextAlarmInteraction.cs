using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class NextAlarmInteraction : MonoBehaviour, IPointerClickHandler
{
    [Header("UI ����")]
    public TextMeshProUGUI nextAlarmText;

    // --- ���ǲ�����Ҫ�κ����ڼ��˫�����ڲ����� ---
    // private float lastClickTime;
    // private int clickCount;

    // --- ��Start/OnDestroy�ж��ĺ�ȡ�������¼� (���ֲ���) ---
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
    /// �������޸������û�������UIԪ��ʱ���˷���������
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // ֱ��ʹ��UnityΪ����ͳ�ƺõ������������
        // ���ֵ������������Կɿ���û�г�ʼ������
        if (eventData.clickCount == 2)
        {
            Debug.Log("Unity EventSystem ��⵽˫��������UI�ܹܴ����ý��档");

            // ִ�д�ҳ��Ĳ���
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowAlarmSetupScreen(null);
            }
        }
    }

    // --- ��ʾ�߼� (��ȫ����) ---
    private void UpdateNextAlarmDisplay()
    {
        if (nextAlarmText == null || AlarmManager.Instance == null || UserManager.Instance == null) return;

        var nextAlarmInfo = AlarmManager.Instance.GetNextUpcomingAlarm();

        if (nextAlarmInfo == null)
        {
            nextAlarmText.text = "��δ��������";
        }
        else
        {
            DateTime triggerTime = nextAlarmInfo.Value.triggerTime.ToLocalTime();
            DateTime today = AlarmManager.Instance.CurrentTime.ToLocalTime().Date;
            DateTime tomorrow = today.AddDays(1);

            string prefix = "";
            if (triggerTime.Date == today) prefix = "����";
            else if (triggerTime.Date == tomorrow) prefix = "����";
            else prefix = triggerTime.ToString("M��d��");

            string timeString;
            if (UserManager.Instance.CurrentUser.use24HourFormat)
            {
                timeString = triggerTime.ToString("HH:mm");
            }
            else
            {
                timeString = triggerTime.ToString("hh:mm tt");
            }

            nextAlarmText.text = $"��һ������: {prefix} {timeString}";
        }
    }
}