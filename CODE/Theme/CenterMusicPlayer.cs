using UnityEngine;

public class CenterMusicPlayer : MonoBehaviour
{
    [Header("UI引用")]
    public Transform listContainer; // 拖入ScrollView的Content对象
    public RectTransform centerDetector; // 拖入作为“检测器”的空对象

    private AudioSource previewAudioSource;
    private Transform currentlyPlayingItem = null;

    void Awake()
    {
        // 为自己添加一个AudioSource来播放预览音乐
        previewAudioSource = gameObject.AddComponent<AudioSource>();
        previewAudioSource.playOnAwake = false;
        previewAudioSource.loop = false;
    }

    void Update()
    {
        FindAndPlayCenterItemMusic();
    }

    void FindAndPlayCenterItemMusic()
    {
        Transform closestItem = null;
        float minDistance = float.MaxValue;

        // 遍历所有列表项
        foreach (Transform item in listContainer)
        {
            // 计算每个项与中心检测器的距离
            float distance = Vector3.Distance(item.position, centerDetector.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestItem = item;
            }
        }

        // 如果找到了一个最近的项，并且它和上次播放的不是同一个
        if (closestItem != null && closestItem != currentlyPlayingItem)
        {
            // 检查这个距离是否足够近，可以被认为是“在中心”
            if (minDistance < 50f) // 50像素的阈值，您可以调整
            {
                currentlyPlayingItem = closestItem;

                // 尝试从这个卡片上获取ThemeData来播放音乐
                ThemeCard cardScript = closestItem.GetComponent<ThemeCard>();
                if (cardScript != null)
                {
                    AudioClip clipToPlay = cardScript.GetThemeData()?.characteristicMusic;
                    if (clipToPlay != null)
                    {
                        previewAudioSource.clip = clipToPlay;
                        previewAudioSource.Play();
                        Debug.Log($"中心项变为: {cardScript.themeNameText.text}, 播放预览音乐。");
                    }
                }
            }
        }
    }
}