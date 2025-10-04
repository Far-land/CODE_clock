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

            // 如果数据有效且年份匹配，则无需更新
            if (holidayData != null && holidayData.year == currentYear.ToString())
            {
                needsUpdate = false;
                Debug.Log($"节日数据为最新 ({currentYear}年)，无需更新。");
            }
        }

        if (needsUpdate)
        {
            Debug.Log($"节日数据已过时或不存在，正在从网络获取 {currentYear} 年的数据...");
            // 使用UnityWebRequest获取新数据
            string url = $"https://holiday.cyi.me/api/holidays?year={currentYear}";
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                webRequest.certificateHandler = new BypassCertificateHandler();
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string newJson = webRequest.downloadHandler.text;
                    holidayData = JsonUtility.FromJson<HolidayApiResponse>(newJson);
                    File.WriteAllText(path, newJson); // 保存到本地
                    Debug.Log($"成功获取并保存了 {currentYear} 年的节日数据。");
                }
                else
                {
                    Debug.LogError("获取节日数据失败: " + webRequest.error);
                }
            }
        }
    }

    /// <summary>
    /// 检查今天是否是一个已知的节日
    /// </summary>
    /// <returns>如果是节日，返回节日的中文名；否则返回null</returns>
    public string GetTodaysHolidayName()
    {
        if (holidayData == null || holidayData.days == null) return null;

        string todayDateString = DateTime.UtcNow.ToLocalTime().ToString("yyyy-MM-dd");

        HolidayDay todayHoliday = holidayData.days.FirstOrDefault(day => day.date == todayDateString);

        return todayHoliday?.name; // 如果找到了，返回name，否则返回null
    }
    public string GetHolidayNameForDate(DateTime dateToCheck)
    {
        if (holidayData == null || holidayData.days == null) return null;

        // 只使用日期部分进行比较，忽略时间和时区
        string dateString = dateToCheck.ToString("yyyy-MM-dd");

        HolidayDay holiday = holidayData.days.FirstOrDefault(day => day.date == dateString);

        return holiday?.name; // 如果找到了，返回name，否则返回null
    }
}