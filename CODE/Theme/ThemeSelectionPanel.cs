using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ThemeSelectionPanel : MonoBehaviour
{
    [Header("UI����")]
    public GameObject themeCardPrefab;
    public Transform listContainer;
    public Button closeButton;

    // ��ȡ��Content�ϵ�InfiniteScroll�ű�������
    private InfiniteScroll infiniteScroller;

    void Awake()
    {
        if (closeButton != null) closeButton.onClick.AddListener(Hide);
        infiniteScroller = listContainer.GetComponent<InfiniteScroll>();
    }

    public void Show(List<ThemeData> themes)
    {
        // ������б�
        foreach (Transform child in listContainer) { Destroy(child.gameObject); }

        // ������б�
        foreach (var theme in themes)
        {
            GameObject cardObject = Instantiate(themeCardPrefab, listContainer);
            ThemeCard cardScript = cardObject.GetComponent<ThemeCard>();
            if (cardScript != null)
            {
                // ��ThemeData���ݸ���Ƭ�Լ�
                cardScript.Setup(theme);
            }
        }

        gameObject.SetActive(true);

        // ʹ��Э��ȷ�����ָ��º��ٳ�ʼ������
        StartCoroutine(InitializeScroller());
    }

    private IEnumerator InitializeScroller()
    {
        // �ȴ�һ֡����UI����ϵͳ��ɼ���
        yield return null;
        if (infiniteScroller != null)
        {
            infiniteScroller.Initialize();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}