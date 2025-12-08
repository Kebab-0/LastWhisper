using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Detector : MonoBehaviour
{
    public Text displayText;              // UI Text для вывода
    public int maxRecords = 10;           // Максимум записей в списке

    private Queue<string> recentSensors = new Queue<string>();

    // Вызывается с Sensors
    public void Detection(GameObject sensor)
    {
        if (sensor == null) return;

        string name = sensor.name;

        // Если уже есть в очереди, удаляем, чтобы переместить в конец
        if (recentSensors.Contains(name))
        {
            Queue<string> temp = new Queue<string>();
            foreach (var s in recentSensors)
                if (s != name) temp.Enqueue(s);
            recentSensors = temp;
        }

        // Добавляем новое имя
        recentSensors.Enqueue(name);

        // Если превышаем лимит, удаляем старые
        while (recentSensors.Count > maxRecords)
            recentSensors.Dequeue();

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        displayText.text = "";
        foreach (var s in recentSensors)
        {
            displayText.text += s + "\n";
        }
    }
}
