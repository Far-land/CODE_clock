using UnityEngine;
using System.Collections;

/// <summary>
/// UI�ܹ� (����)��
/// ����Ӧ����Ψһ��UI�������ģ�����������UI������ʾ�����ء�
/// </summary>
public class UIManager : MonoBehaviour
{
    // ����һ��ȫ��Ψһ�ľ�̬ʵ��
    public static UIManager Instance { get; private set; }

    [Header("UI���ű�������")]
    public AlarmSetupScreen alarmSetupScreen;
    public AlarmRingingPanel alarmRingingPanel;
    public ThemeSelectionPanel themeSelectionPanel;

    void Awake()
    {
        // ���õ���ģʽ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // ȷ���������������ʱ���ǹرյ�
        if (alarmSetupScreen != null) alarmSetupScreen.gameObject.SetActive(false);
        if (alarmRingingPanel != null) alarmRingingPanel.gameObject.SetActive(false);
        if (themeSelectionPanel != null) themeSelectionPanel.gameObject.SetActive(false);
    }

    void Start()
    {
        // ���������Ӵ������¼�
        if (AlarmManager.Instance != null)
        {
            AlarmManager.Instance.OnAlarmTriggered += OnAlarmTriggered;
        }
    }

    void OnDestroy()
    {
        // ȡ���¼�����
        if (AlarmManager.Instance != null)
        {
            AlarmManager.Instance.OnAlarmTriggered -= OnAlarmTriggered;
        }
    }

    /// <summary>
    /// �����Ӵ���ʱ��ִ�д˷���
    /// </summary>
    private void OnAlarmTriggered(Alarm triggeredAlarm)
    {
        if (alarmRingingPanel != null)
        {
            alarmRingingPanel.Show(triggeredAlarm);
        }
    }

    /// <summary>
    /// ���Ǵ��������ý����Ψһ���
    /// </summary>
    public void ShowAlarmSetupScreen(Alarm alarmToEdit)
    {
        if (alarmSetupScreen != null)
        {
            // ��UIManager�Լ�����Э��
            StartCoroutine(ShowAlarmSetupScreenRoutine(alarmToEdit));
        }
    }

    /// <summary>
    /// ������ѡ�����
    /// </summary>
    public void ShowThemeSelectionPanel()
    {
        if (themeSelectionPanel != null && ThemeManager.Instance != null)
        {
            themeSelectionPanel.Show(ThemeManager.Instance.availableThemes);
        }
    }

    /// <summary>
    /// ����������������ʾ���̵�Э�̣�ȷ��ʱ���Ͳ�����ȷ
    /// </summary>
    private IEnumerator ShowAlarmSetupScreenRoutine(Alarm alarmToEdit)
    {
        // 1. UIManager���𡰻��ѡ����
        alarmSetupScreen.gameObject.SetActive(true);

        // 2.���ؼ����ȴ�һ֡��������ϵ�Animatorһ�㷴Ӧʱ��
        yield return null;

        // 3. �����������ȫ׼���ã���������ִ��Show�߼�����������ȷ�Ĳ���
        alarmSetupScreen.Show(alarmToEdit);
    }
}