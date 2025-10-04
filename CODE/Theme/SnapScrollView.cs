using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(ScrollRect))]
public class SnapScrollView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("��������")]
    [Tooltip("��Ƭ���������ĵ��ٶ�")]
    public float snapSpeed = 10f;
    [Tooltip("��Ƭ֮��ļ�࣬�����HorizontalLayoutGroup�е�Spacingһ��")]
    public float itemSpacing = 20f;
    [Tooltip("�������ߵĶ���߾࣬�����õ�һ�ź����һ���ܾ���")]
    public float padding = 100f;

    // ����������仯ʱ���������¼������������������
    public Action<int> OnCenterItemChanged;

    private ScrollRect scrollRect;
    private RectTransform contentPanel;
    private List<RectTransform> items = new List<RectTransform>();
    private bool isDragging = false;
    private int currentCenterIndex = -1;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        contentPanel = scrollRect.content;
    }

    /// <summary>
    /// ���ⲿ���ã����ڳ�ʼ���б�
    /// </summary>
    public void Initialize(int itemCount)
    {
        items.Clear();
        for (int i = 0; i < contentPanel.childCount; i++)
        {
            items.Add(contentPanel.GetChild(i) as RectTransform);
        }

        if (items.Count == 0) return;

        // �������ұ߾࣬ȷ����һ�ź����һ���ܾ���
        var layoutGroup = contentPanel.GetComponent<HorizontalLayoutGroup>();
        if (layoutGroup != null)
        {
            float panelWidth = GetComponent<RectTransform>().rect.width;
            float itemWidth = items[0].rect.width;
            padding = (panelWidth - itemWidth) / 2f;
            layoutGroup.padding.left = Mathf.RoundToInt(padding);
            layoutGroup.padding.right = Mathf.RoundToInt(padding);
        }

        // �������һ��������
        FindNewCenterItem();
    }

    void Update()
    {
        // ֻ�����û�û����קʱ���Ž����Զ�����
        if (!isDragging && items.Count > 0)
        {
            float targetX = -items[currentCenterIndex].anchoredPosition.x;
            float newX = Mathf.Lerp(contentPanel.anchoredPosition.x, targetX, Time.deltaTime * snapSpeed);
            contentPanel.anchoredPosition = new Vector2(newX, contentPanel.anchoredPosition.y);
        }

        // --- ����ѭ���߼� ---
        if (items.Count > 1) // ������Ҫ2�����ϲ���ѭ��
        {
            for (int i = 0; i < items.Count; i++)
            {
                float itemPosInViewport = transform.InverseTransformPoint(items[i].position).x;
                float contentWidth = contentPanel.rect.width;

                // �����Ƭ���󻬳���Ұ̫��
                if (itemPosInViewport < -contentWidth / 2f)
                {
                    items[i].SetAsLastSibling();
                }
                // �����Ƭ���һ�����Ұ̫��
                else if (itemPosInViewport > contentWidth / 2f)
                {
                    items[i].SetAsFirstSibling();
                }
            }
        }
    }

    // ����ʼ��קʱ����
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    // ��������קʱ����
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        FindNewCenterItem();
    }

    /// <summary>
    /// ���Ҿ��������������
    /// </summary>
    private void FindNewCenterItem()
    {
        float minDistance = float.MaxValue;
        int newCenterIndex = 0;

        for (int i = 0; i < items.Count; i++)
        {
            // ����ÿ�����������Ļ���ĵľ���
            float distance = Mathf.Abs(transform.InverseTransformPoint(items[i].position).x);
            if (distance < minDistance)
            {
                minDistance = distance;
                newCenterIndex = i;
            }
        }

        if (newCenterIndex != currentCenterIndex)
        {
            currentCenterIndex = newCenterIndex;
            // �����¼���֪ͨ�ⲿ����������ˣ���
            OnCenterItemChanged?.Invoke(currentCenterIndex);
        }
    }
}