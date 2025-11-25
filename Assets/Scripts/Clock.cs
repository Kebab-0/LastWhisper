using UnityEngine;
using UnityEngine.UI;

public class GameTime : MonoBehaviour
{
    public Text timeText;
    public float timeScale = 1f;

    private float gameTime = 0f;

    void Update()
    {
        
        gameTime += Time.deltaTime * timeScale;

       
        if (gameTime >= 24f)
            gameTime -= 24f;

       
        int hours = Mathf.FloorToInt(gameTime);
        int minutes = Mathf.FloorToInt((gameTime - hours) * 60);

        
        string timeString = string.Format("{0:00}:{1:00}", hours, minutes);

        
        if (timeText != null)
            timeText.text = timeString;
    }
}