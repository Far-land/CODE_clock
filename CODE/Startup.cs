using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    // �������ǲ�����ҪmainSceneName����Ϊ������ThemeManager����
    public string onboardingSceneName = "OnboardingScene";

    void Start()
    {
        // ȷ�����к��Ĺ���������Awake

        if (UserManager.Instance.CurrentUser.hasCompletedOnboarding)
        {
            Debug.Log("�û���������������������ѡ������ⳡ����");

            // �������޸ġ�ֱ������ThemeManagerȥ���ض�Ӧ�ĳ���
            if (ThemeManager.Instance != null)
            {
                ThemeManager.Instance.LoadLastSelectedTheme();
            }
            else
            {
                Debug.LogError("���ش���ThemeManagerʵ�������ڣ��޷��������⡣");
            }
        }
        else
        {
            Debug.Log("�״ν��룬��ʼ�������̡�");
            SceneManager.LoadScene(onboardingSceneName);
        }
    }
}