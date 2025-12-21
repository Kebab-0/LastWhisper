using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Гарантирует сброс Time.timeScale при загрузке любой сцены,
/// чтобы таймеры и анимации продолжали работать после паузы.
/// </summary>
public static class TimeScaleResetter
{
    private static bool _registered;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Register()
    {
        if (_registered) return;

        SceneManager.sceneLoaded += OnSceneLoaded;
        _registered = true;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;
    }
}
