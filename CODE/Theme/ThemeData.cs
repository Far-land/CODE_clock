using UnityEngine;
using UnityEngine.SceneManagement; // 以后可能会用到

/// <summary>
/// 定义单个主题包含的所有数据。
/// 这是一个ScriptableObject，意味着我们可以在Project窗口中创建多个主题资产文件。
/// </summary>
[CreateAssetMenu(fileName = "NewTheme", menuName = "AlarmApp/Theme Data", order = 4)]
public class ThemeData : ScriptableObject
{
    [Header("主题信息")]
    public string themeName = "默认主题";
    public Sprite themeThumbnail; // 用于在选择界面显示的缩略图

    [Header("特色音乐")]
    public AudioClip characteristicMusic; // 滚动到这个主题时播放的特色音乐

    [Header("目标场景")]
    [Tooltip("选择这个主题后要加载的场景文件名")]
    public string sceneToLoad;
}