using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System;
using System.Linq;

public class HolidayManager : MonoBehaviour
{
    public static HolidayManager Instance { get; private set; }

    private HolidayApiResponse holidayData;
    private string saveFileName = "holidays.json";

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartCoroutine(CheckForHolidayUpdateRoutine());
    }

    private IEnumerator CheckForHolidayUpdateRoutine()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        int currentYear = DateTime.UtcNow.Year;

        bool needsUpdate = true;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            holidayData = JsonUtility.FromJson<HolidayApiResponse>(json);

            // ���������Ч�����ƥ�䣬���������
            if (holidayData != null && holidayData.year == currentYear.ToString())
            {
                needsUpdate = false;
                Debug.Log($"��������Ϊ���� ({currentYear}��)��������¡�");
            }
        }

        if (needsUpdate)
        {
            Debug.Log($"���������ѹ�ʱ�򲻴��ڣ����ڴ������ȡ {currentYear} �������...");
            // ʹ��UnityWebRequest��ȡ������
            string url = $"https://holiday.cyi.me/api/holidays?year={currentYear}";
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                webRequest.certificateHandler = new BypassCertificateHandler();
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string newJson = webRequest.downloadHandler.text;
                    holidayData = JsonUtility.FromJson<HolidayApiResponse>(newJson);
                    File.WriteAllText(path, newJson); // ���浽����
                    Debug.Log($"�ɹ���ȡ�������� {currentYear} ��Ľ������ݡ�");
                }
                else
                {
                    Debug.LogError("��ȡ��������ʧ��: " + webRequest.error);
                }
            }
        }
    }

    /// <summary>
    /// �������Ƿ���һ����֪�Ľ���
    /// </summary>
    /// <returns>����ǽ��գ����ؽ��յ������������򷵻�null</returns>
    public string GetTodaysHolidayName()
    {
        if (holidayData == null || holidayData.days == null) return null;

        string todayDateString = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd");

        HolidayDay todayHoliday = holidayData.days.FirstOrDefault(day => day.date == todayDateString);

        return todayHoliday?.name; // ����ҵ��ˣ�����name�����򷵻�null
    }
    public string GetHolidayNameForDate(DateTime dateToCheck)
    {
        if (holidayData == null || holidayData.days == null) return null;

        // ֻʹ�����ڲ��ֽ��бȽϣ�����ʱ���ʱ��
        string dateString = dateToCheck.ToString("yyyy-MM-dd");

        HolidayDay holiday = holidayData.days.FirstOrDefault(day => day.date == dateString);

        return holiday?.name; // ����ҵ��ˣ�����name�����򷵻�null
    }
}