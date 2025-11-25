using UnityEngine;
using UnityEngine.UI;

public class Detector : MonoBehaviour
{
    public Text DetectorText;
    public Text timeText;

    void Update()
    {
        if (timeText != null)
        {
            string currentTime = timeText.text;
           
        }
    }

    public void Detection()
    {
        string finaltext = timeText.text + ": The enemy has been detected\n";
        DetectorText.text += finaltext;
    }
}
