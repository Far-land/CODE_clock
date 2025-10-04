using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.EventSystems; // �����¼�ϵͳ�����ռ�

// ���޸ĵ㡿ʵ����ק�¼���صĽӿ�
[RequireComponent(typeof(ScrollRect))]
public class ThemePaginator : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("��ҳ����")]
    public float pageSwitchSpeed = 0.5f;

    // �����Ƴ���������Ҫ��ť����
    // public Button leftButton;
    // public Button rightButton;

    // ��ҳ���л�ʱ�������¼�
    public Action<int> OnPageChanged;

    private ScrollRect scrollRect;
    private RectTransform contentPanel;
    private int pageCount = 0;
    private int currentPage = 0;
    private bool isSwitching = false;

    // �����жϻ����ı���
    private float startDragPositionX;
    private float dragThreshold = 50f; // ��������50���ز���һ����Ч����

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        contentPanel = scrollRect.content;

        // �����Ƴ���������ҪΪ��ť���¼�
        // leftButton.onClick.AddListener(GoToPreviousPage);
        // rightButton.onClick.AddListener(GoToNextPage);
    }

    /// <summary>
    /// ���ⲿ���ã����ڳ�ʼ����ҳ��
    /// </summary>
    public void Initialize(int count)
    {
        pageCount = count;
        currentPage = 0;
        // ������λ������Ϊ��һҳ
        scrollRect.horizontalNormalizedPosition = 0;
        OnPageChanged?.Invoke(currentPage);
    }

    // --- ����������ק�¼����� ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        // ��¼��ʼ��קʱ��ˮƽλ��
        startDragPositionX = contentPanel.anchoredPosition.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // ������ק�ľ���
        float dragDistance = contentPanel.anchoredPosition.x - startDragPositionX;

        if (dragDistance > dragThreshold) // ������ק����ʾ��ߵ����ݣ�-> ��һҳ
        {
            GoToPreviousPage();
        }
        else if (dragDistance < -dragThreshold) // ������ק����ʾ�ұߵ����ݣ�-> ��һҳ
        {
            GoToNextPage();
        }
        else // �����ק���벻�����򵯻ص�ǰҳ��
        {
            StartCoroutine(SwitchToPage(currentPage));
        }
    }

    // --- ҳ���л��߼� (���ֲ��䣬����������ק����) ---

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

        // ����Ŀ��λ��
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