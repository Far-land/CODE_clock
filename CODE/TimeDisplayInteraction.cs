using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // ����Image���

public class TimeDisplayInteraction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("UI ����")]
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI dayOfWeekText;

    [Header("��������")]
    public float longPressDuration = 0.8f;

    // �ڲ�״̬����
    private bool isPointerDown = false;
    private bool isLongPress = false;
    private Coroutine longPressCoroutine;
    private DateTime lastDateDisplayed;
    public ThemeSelectionPanel themeSelectionPanel;

    // �����Ƴ���������Ҫ���ر������洢ʱ���ʽ
    // private bool use24HourFormat = true;
    // private const string TimeFormatKey = "TimeFormat24H";

    // ��������12Сʱ������/����ͼ��
    public GameObject amIcon;
    public GameObject pmIcon;


    private bool colonVisible = true; // ����ð����˸״̬

    void Start()
    {
        // �����޸ġ�������Ҫ��PlayerPrefs���أ���Ϊ���ǻ�ʵʱ��UserManager��ȡ
        StartCoroutine(UpdateTimeRoutine());
        if (amIcon != null) amIcon.SetActive(false);
        if (pmIcon != null) pmIcon.SetActive(false);
    }

    // --- �����߼� ---

    public void OnPointerDown(PointerEventData eventData)
    {
        longPressCoroutine = StartCoroutine(LongPressCheckRoutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (longPressCoroutine != null) StopCoroutine(longPressCoroutine);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isLongPress)
        {
            isLongPress = false;
            return;
        }
        HandleShortClick();
    }



    /// <summary>
    /// �������޸ġ��̰��л������ѱ��Ƴ�
    /// </summary>
    private void HandleShortClick()
    {
        // �����������ʲôҲ���������������Է�һ����Ч
        //UIManager.Instance.ShowThemeSelectionPanel();
        Debug.Log("ʱ���ı����������ǰ��Ԥ�蹦�ܡ�");
    }

    // ����������Ȼ��Ϊδ������������ռλ
    private IEnumerator LongPressCheckRoutine()
    {
        //UIManager.Instance.ShowThemeSelectionPanel();
        yield return new WaitForSeconds(longPressDuration);
        HandleLongPress();
    }

    private void HandleLongPress()
    {
        Debug.Log("������ʱ�䡱��壡׼�����������ý���...");
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowThemeSelectionPanel();
        }
    }


    // --- ʱ���������ʾ�߼� ---

    private IEnumerator UpdateTimeRoutine()
    {
        while (AlarmManager.Instance == null || UserManager.Instance == null)
        {
            // �ȴ����й�������׼������
            yield return null;
        }

        lastDateDisplayed = DateTime.MinValue;

        while (true)
        {
            UpdateTimeDisplay();

            DateTime trustedLocalTime = AlarmManager.Instance.CurrentTime.ToLocalTime();
            if (trustedLocalTime.Date != lastDateDisplayed)
            {
                UpdateDateDisplay();
                UpdateDayOfWeekDisplay();
                lastDateDisplayed = trustedLocalTime.Date;
            }

            yield return new WaitForSeconds(1f);
            colonVisible = !colonVisible;

        }
    }

    /// <summary>
    /// �������޸ġ�����ʱ����ʾʱ��ֱ�Ӵ�UserManager��ȡ����
    /// </summary>
    private void UpdateTimeDisplay()
    {
        // ���Ӷ�UserManager�ļ��
        if (timeText == null || AlarmManager.Instance == null || UserManager.Instance == null) return;

        DateTime trustedLocalTime = AlarmManager.Instance.CurrentTime.ToLocalTime();
        string timeFormat;

        // ֱ��ʹ�� UserManager �д洢���û�ƫ�����ж�
        if (UserManager.Instance.CurrentUser.use24HourFormat)
        {
            timeFormat = "HH" + (colonVisible ? ":" : " ") + "mm"; // ����ʾ�룬ð����˸
                                                                   // ����12Сʱ��ͼ��
            if (amIcon != null) amIcon.SetActive(false);
            if (pmIcon != null) pmIcon.SetActive(false);
        }
        else
        {
            timeFormat = "hh" + (colonVisible ? ":" : " ") + "mm"; // ����ʾ�룬ð����˸
                                                                   // ��ʾ��Ӧ��AM/PMͼ��
            if (trustedLocalTime.Hour < 12)
            {
                if (amIcon != null) amIcon.SetActive(true);
                if (pmIcon != null) pmIcon.SetActive(false);
            }
            else
            {
                if (amIcon != null) amIcon.SetActive(false);
                if (pmIcon != null) pmIcon.SetActive(true);
            }
        }
        timeText.text = trustedLocalTime.ToString(timeFormat);
    }

    // ���·������ֲ���
    private void UpdateDateDisplay()
    {
        if (dateText == null || AlarmManager.Instance == null) return;
        DateTime trustedLocalTime = AlarmManager.Instance.CurrentTime.ToLocalTime();
        dateText.text = trustedLocalTime.ToString("D");
    }

    private void UpdateDayOfWeekDisplay()
    {
        if (dayOfWeekText == null || AlarmManager.Instance == null) return;
        DateTime trustedLocalTime = AlarmManager.Instance.CurrentTime.ToLocalTime();
        dayOfWeekText.text = trustedLocalTime.ToString("dddd");
    }
}