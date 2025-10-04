using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    public List<ThemeData> availableThemes;
    public ThemeData currentTheme { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ApplyTheme(ThemeData theme)
    {
        if (theme == null || string.IsNullOrEmpty(theme.sceneToLoad)) return;
        currentTheme = theme;
        if (UserManager.Instance != null)
        {
            UserManager.Instance.CurrentUser.selectedTheme = theme.themeName;
            UserManager.Instance.SaveUserData();
        }
        SceneManager.LoadScene(theme.sceneToLoad);
    }

    public void LoadLastSelectedTheme()
    {
        string lastThemeName = "Ä¬ÈÏÖ÷Ìâ";
        if (UserManager.Instance != null)
        {
            lastThemeName = UserManager.Instance.CurrentUser.selectedTheme;
        }
        ThemeData lastTheme = availableThemes.FirstOrDefault(t => t.themeName == lastThemeName);
        if (lastTheme != null)
        {
            ApplyTheme(lastTheme);
        }
        else if (availableThemes.Count > 0)
        {
            ApplyTheme(availableThemes[0]);
        }
    }
}