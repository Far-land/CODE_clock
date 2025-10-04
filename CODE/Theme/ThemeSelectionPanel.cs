using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ThemeSelectionPanel : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject themeCardPrefab;
    public Transform listContainer;
    public Button closeButton;

    // 获取对Content上的InfiniteScroll脚本的引用
    private InfiniteScroll infiniteScroller;

    void Awake()
    {
        if (closeButton != null) closeButton.onClick.AddListener(Hide);
        infiniteScroller = listContainer.GetComponent<InfiniteScroll>();
    }

    public void Show(List<ThemeData> themes)
    {
        // 清理旧列表
        foreach (Transform child in listContainer) { Destroy(child.gameObject); }

        // 填充新列表
        foreach (var theme in themes)
        {
            GameObject cardObject = Instantiate(themeCardPrefab, listContainer);
            ThemeCard cardScript = cardObject.GetComponent<ThemeCard>();
            if (cardScript != null)
            {
                // 将ThemeData传递给卡片自己
                cardScript.Setup(theme);
            }
        }

        gameObject.SetActive(true);

        // 使用协程确保布局更新后再初始化滚动
        StartCoroutine(InitializeScroller());
    }

    private IEnumerator InitializeScroller()
    {
        // 等待一帧，让UI布局系统完成计算
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