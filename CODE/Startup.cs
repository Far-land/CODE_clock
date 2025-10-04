using UnityEngine;
using UnityEngine.SceneManagement;

public class Startup : MonoBehaviour
{
    // 这里我们不再需要mainSceneName，因为它将由ThemeManager决定
    public string onboardingSceneName = "OnboardingScene";

    void Start()
    {
        // 确保所有核心管理器都已Awake

        if (UserManager.Instance.CurrentUser.hasCompletedOnboarding)
        {
            Debug.Log("用户已完成引导，将加载最后选择的主题场景。");

            // 【核心修改】直接命令ThemeManager去加载对应的场景
            if (ThemeManager.Instance != null)
            {
                ThemeManager.Instance.LoadLastSelectedTheme();
            }
            else
            {
                Debug.LogError("严重错误：ThemeManager实例不存在！无法加载主题。");
            }
        }
        else
        {
            Debug.Log("首次进入，开始引导流程。");
            SceneManager.LoadScene(onboardingSceneName);
        }
    }
}