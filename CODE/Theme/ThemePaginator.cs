using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.EventSystems; // 引入事件系统命名空间

// 【修改点】实现拖拽事件相关的接口
[RequireComponent(typeof(ScrollRect))]
public class ThemePaginator : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("翻页设置")]
    public float pageSwitchSpeed = 0.5f;

    // 【已移除】不再需要按钮引用
    // public Button leftButton;
    // public Button rightButton;

    // 当页面切换时触发的事件
    public Action<int> OnPageChanged;

    private ScrollRect scrollRect;
    private RectTransform contentPanel;
    private int pageCount = 0;
    private int currentPage = 0;
    private bool isSwitching = false;

    // 用于判断滑动的变量
    private float startDragPositionX;
    private float dragThreshold = 50f; // 滑动超过50像素才算一次有效滑动

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        contentPanel = scrollRect.content;

        // 【已移除】不再需要为按钮绑定事件
        // leftButton.onClick.AddListener(GoToPreviousPage);
        // rightButton.onClick.AddListener(GoToNextPage);
    }

    /// <summary>
    /// 由外部调用，用于初始化总页数
    /// </summary>
    public void Initialize(int count)
    {
        pageCount = count;
        currentPage = 0;
        // 立即将位置设置为第一页
        scrollRect.horizontalNormalizedPosition = 0;
        OnPageChanged?.Invoke(currentPage);
    }

    // --- 【新增】拖拽事件处理 ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 记录开始拖拽时的水平位置
        startDragPositionX = contentPanel.anchoredPosition.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 计算拖拽的距离
        float dragDistance = contentPanel.anchoredPosition.x - startDragPositionX;

        if (dragDistance > dragThreshold) // 向右拖拽（显示左边的内容）-> 上一页
        {
            GoToPreviousPage();
        }
        else if (dragDistance < -dragThreshold) // 向左拖拽（显示右边的内容）-> 下一页
        {
            GoToNextPage();
        }
        else // 如果拖拽距离不够，则弹回当前页面
        {
            StartCoroutine(SwitchToPage(currentPage));
        }
    }

    // --- 页面切换逻辑 (保持不变，但现在由拖拽触发) ---

    public void GoToNextPage()
    {
        if (isSwitching || pageCount <= 1) return;
        currentPage = (currentPage + 1) % pageCount;
        StartCoroutine(SwitchToPage(currentPage));
    }

    public void GoToPreviousPage()
    {
        if (isSwitching || pageCount <= 1) return;
        currentPage = (currentPage - 1 + pageCount) % pageCount;
        StartCoroutine(SwitchToPage(currentPage));
    }

    private IEnumerator SwitchToPage(int pageIndex)
    {
        isSwitching = true;

        // 计算目标位置
        float targetNormalizedPos = (pageCount > 1) ? (float)pageIndex / (float)(pageCount - 1) : 0;
        float startNormalizedPos = scrollRect.horizontalNormalizedPosition;
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime / pageSwitchSpeed;
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(startNormalizedPos, targetNormalizedPos, timer);
            yield return null;
        }

        isSwitching = false;
        OnPageChanged?.Invoke(pageIndex);
    }
}