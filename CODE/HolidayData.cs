using System.Collections.Generic;

[System.Serializable]
public class HolidayApiResponse
{
    public string year;
    public List<HolidayDay> days;
}

[System.Serializable]
public class HolidayDay
{
    public string name;
    public string date; // ∏Ò Ω: "2025-01-01"
    public bool isOffDay;
}