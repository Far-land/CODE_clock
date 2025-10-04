using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickHandler : MonoBehaviour, IPointerClickHandler
{
    // ���޸ĵ㡿���ǲ�����Ҫֱ������ AlarmSetupScreen
    // public AlarmSetupScreen targetSetupScreen; 

    public void OnPointerClick(PointerEventData eventData)
    {
        // ����Ƿ���˫��
        if (eventData.clickCount == 2)
        {
            Debug.Log("˫���ɹ����������� UIManager ��ʾ���...");

            // �������޸�������ֱ�ӵ��� targetSetupScreen.Show()
            // ����ͨ��Ψһ�ġ�UI�ܹܡ�����ʾ���
            if (UIManager.Instance != null)
            {
                // ���� null ��ʾ���������½�һ������
                UIManager.Instance.ShowAlarmSetupScreen(null);
            }
            else
            {
                Debug.LogError("�Ҳ��� UIManager ��ʵ������ȷ�������д��� UIManager��");
            }
        }
    }
}