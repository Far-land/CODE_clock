using UnityEngine;
using System;

/// <summary>
/// 这个ScriptableObject现在只存储一套通用的节日音乐。
/// 只要是节日，就会根据情境播放以下音乐。
/// </summary>
[CreateAssetMenu(fileName = "HolidayMusicLibrary", menuName = "AlarmApp/Holiday Music Library", order = 3)]
public class HolidayMusicLibrary : ScriptableObject
{
    [Header("节日通用音乐")]
    public AudioClip sunnyDay;
    public AudioClip sunnyNight;
    public AudioClip rainyDay;
    public AudioClip rainyNight;
    public AudioClip snowyDay;
    public AudioClip snowyNight;
}