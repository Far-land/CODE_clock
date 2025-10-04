using UnityEngine;
using System;

/// <summary>
/// ���ScriptableObject����ֻ�洢һ��ͨ�õĽ������֡�
/// ֻҪ�ǽ��գ��ͻ�����龳�����������֡�
/// </summary>
[CreateAssetMenu(fileName = "HolidayMusicLibrary", menuName = "AlarmApp/Holiday Music Library", order = 3)]
public class HolidayMusicLibrary : ScriptableObject
{
    [Header("����ͨ������")]
    public AudioClip sunnyDay;
    public AudioClip sunnyNight;
    public AudioClip rainyDay;
    public AudioClip rainyNight;
    public AudioClip snowyDay;
    public AudioClip snowyNight;
}