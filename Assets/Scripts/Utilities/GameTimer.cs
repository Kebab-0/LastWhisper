using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private Text timerText;

    // Реальные секунды (не зависят от Time.timeScale)
    public float ElapsedSeconds { get; private set; }

    private void Update()
    {
        ElapsedSeconds += Time.unscaledDeltaTime;

        if (timerText != null)
            timerText.text = GetTimeString();
    }

    public string GetTimeString()
    {
        int minutes = (int)(ElapsedSeconds / 60f);
        float sec = ElapsedSeconds - minutes * 60f;

        // mm:ss.ff
        return $"{minutes:00}:{sec:00.00}";
    }

    public void ResetTimer()
    {
        ElapsedSeconds = 0f;
    }
}
