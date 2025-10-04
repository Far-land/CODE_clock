using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class InfiniteScroll : MonoBehaviour
{
    private ScrollRect scrollRect;
    private RectTransform contentPanel;
    private List<RectTransform> items = new List<RectTransform>();
    private float itemWidthPlusSpacing;
    private float halfContentWidth;

    void Start()
    {
        scrollRect = GetComponentInParent<ScrollRect>();
        contentPanel = GetComponent<RectTransform>();

        // ���������¼�
        scrollRect.onValueChanged.AddListener(OnScroll);
    }

    /// <summary>
    /// ���ⲿ���ã��������б�������г�ʼ��
    /// </summary>
    public void Initialize()
    {
        items.Clear();
        foreach (Transform child in contentPanel)
        {
            items.Add(child as RectTransform);
        }

        if (items.Count < 2) return; // ������Ҫ2������ѭ��

        var layoutGroup = contentPanel.GetComponent<HorizontalLayoutGroup>();
        itemWidthPlusSpacing = items[0].rect.width + (layoutGroup != null ? layoutGroup.spacing : 0);
        halfContentWidth = (items.Count * itemWidthPlusSpacing) / 2f;
    }

    private void OnScroll(Vector2 position)
    {
        foreach (RectTransform item in items)
        {
            // ���б���ı���λ��ת��Ϊ�����Scroll View���ĵ�λ��
            float itemPosInViewport = contentPanel.anchoredPosition.x + item.anchoredPosition.x;

            // �����Ƭ���󻬳���Ұ̫Զ (����һ���ܳ���)
            if (itemPosInViewport < -halfContentWidth)
            {
                // ������λ���ƶ����б�����ұ�
                item.anchoredPosition += new Vector2(items.Count * itemWidthPlusSpacing, 0);
            }
            // �����Ƭ���һ�����Ұ̫Զ
            else if (itemPosInViewport > halfContentWidth)
            {
                // ������λ���ƶ����б�������
                item.anchoredPosition -= new Vector2(items.Count * itemWidthPlusSpacing, 0);
            }
        }
    }
}