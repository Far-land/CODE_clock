using UnityEngine;

public class CenterMusicPlayer : MonoBehaviour
{
    [Header("UI����")]
    public Transform listContainer; // ����ScrollView��Content����
    public RectTransform centerDetector; // ������Ϊ����������Ŀն���

    private AudioSource previewAudioSource;
    private Transform currentlyPlayingItem = null;

    void Awake()
    {
        // Ϊ�Լ����һ��AudioSource������Ԥ������
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

        // ���������б���
        foreach (Transform item in listContainer)
        {
            // ����ÿ���������ļ�����ľ���
            float distance = Vector3.Distance(item.position, centerDetector.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestItem = item;
            }
        }

        // ����ҵ���һ�����������������ϴβ��ŵĲ���ͬһ��
        if (closestItem != null && closestItem != currentlyPlayingItem)
        {
            // �����������Ƿ��㹻�������Ա���Ϊ�ǡ������ġ�
            if (minDistance < 50f) // 50���ص���ֵ�������Ե���
            {
                currentlyPlayingItem = closestItem;

                // ���Դ������Ƭ�ϻ�ȡThemeData����������
                ThemeCard cardScript = closestItem.GetComponent<ThemeCard>();
                if (cardScript != null)
                {
                    AudioClip clipToPlay = cardScript.GetThemeData()?.characteristicMusic;
                    if (clipToPlay != null)
                    {
                        previewAudioSource.clip = clipToPlay;
                        previewAudioSource.Play();
                        Debug.Log($"�������Ϊ: {cardScript.themeNameText.text}, ����Ԥ�����֡�");
                    }
                }
            }
        }
    }
}