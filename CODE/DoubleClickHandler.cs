using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickHandler : MonoBehaviour, IPointerClickHandler
{
    // 【修改点】我们不再需要直接引用 AlarmSetupScreen
    // public AlarmSetupScreen targetSetupScreen; 

    public void OnPointerClick(PointerEventData eventData)
    {
        // 检查是否是双击
        if (eventData.clickCount == 2)
        {
            Debug.Log("双击成功，正在请求 UIManager 显示面板...");

            // 【核心修复】不再直接调用 targetSetupScreen.Show()
            // 而是通过唯一的“UI总管”来显示面板
            if (UIManager.Instance != null)
            {
                // 传递 null 表示我们正在新建一个闹钟
                UIManager.Instance.ShowAlarmSetupScreen(null);
            }
            else
            {
                Debug.LogError("找不到 UIManager 的实例！请确保场景中存在 UIManager。");
            }
        }
    }
}