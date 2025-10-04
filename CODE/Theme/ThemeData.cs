using UnityEngine;
using UnityEngine.SceneManagement; // �Ժ���ܻ��õ�

/// <summary>
/// ���嵥������������������ݡ�
/// ����һ��ScriptableObject����ζ�����ǿ�����Project�����д�����������ʲ��ļ���
/// </summary>
[CreateAssetMenu(fileName = "NewTheme", menuName = "AlarmApp/Theme Data", order = 4)]
public class ThemeData : ScriptableObject
{
    [Header("������Ϣ")]
    public string themeName = "Ĭ������";
    public Sprite themeThumbnail; // ������ѡ�������ʾ������ͼ

    [Header("��ɫ����")]
    public AudioClip characteristicMusic; // �������������ʱ���ŵ���ɫ����

    [Header("Ŀ�곡��")]
    [Tooltip("ѡ����������Ҫ���صĳ����ļ���")]
    public string sceneToLoad;
}