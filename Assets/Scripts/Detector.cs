using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Detector : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Text displayText;
    [SerializeField] private GameTime gameTime;
    public int maxRecords = 10;           // Максимум записей в списке

    private readonly Dictionary<string, AudioClip> sensorSounds = new Dictionary<string, AudioClip>();
    private Queue<string> recentSensors = new Queue<string>();

    private void Awake()
    {
        EnsureAudioSource();
    }

    public void Detection(GameObject sensor, Entity activator = null)
    {
        if (sensor == null) return;

        string sensorName = sensor.name;
        int sensorCode = ParseSensorCode(sensorName);

        if (activator != null && activator.EmittedSound != null && sensorCode > 0)
        {
            sensorSounds[$"Sensor{sensorCode}"] = activator.EmittedSound;
        }

        EnqueueDetectionMessage(sensorCode);
    }

    public bool TryPlaySensorSound(int sensorCode)
    {
        string key = $"Sensor{sensorCode}";
        if (!sensorSounds.TryGetValue(key, out AudioClip clip) || clip == null)
            return false;

        EnsureAudioSource();
        audioSource.PlayOneShot(clip);
        return true;
    }

    private void EnsureAudioSource()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void EnqueueDetectionMessage(int sensorCode)
    {
        if (sensorCode <= 0 || displayText == null) return;

        string message = BuildMessage(sensorCode);

        if (recentSensors.Contains(message))
        {
            Queue<string> temp = new Queue<string>();
            foreach (var s in recentSensors)
                if (s != message) temp.Enqueue(s);
            recentSensors = temp;
        }

        recentSensors.Enqueue(message);

        while (recentSensors.Count > maxRecords)
            recentSensors.Dequeue();

        UpdateDisplay();
    }

    private string BuildMessage(int sensorCode)
    {
        int radius = sensorCode / 10;
        int sensorNumber = sensorCode % 10;

        string romanRadius = ToRoman(radius);
        string timeStamp = ResolveGameTime();

        return $"Сенсор {sensorNumber}-ый, радиус {romanRadius}: Обнаружена сущность в {timeStamp}";
    }

    private string ResolveGameTime()
    {
        if (gameTime == null)
            gameTime = FindObjectOfType<GameTime>();

        if (gameTime != null)
            return gameTime.GetFormattedTime();

        float seconds = Time.time;
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:00}:{secs:00}";
    }

    private string ToRoman(int value)
    {
        // Поддерживаем значения радиуса (1-5) и fallback в виде числа
        if (value <= 0) return value.ToString();

        int[] numbers = { 10, 9, 5, 4, 1 };
        string[] romans = { "X", "IX", "V", "IV", "I" };

        int remaining = Mathf.Min(value, 5); // радиусы 1..5 отображаются римскими
        string result = "";

        for (int i = 0; i < numbers.Length && remaining > 0; i++)
        {
            while (remaining >= numbers[i])
            {
                result += romans[i];
                remaining -= numbers[i];
            }
        }

        return string.IsNullOrEmpty(result) ? value.ToString() : result;
    }

    private int ParseSensorCode(string sensorName)
    {
        if (string.IsNullOrEmpty(sensorName)) return -1;
        if (!sensorName.StartsWith("Sensor")) return -1;

        if (int.TryParse(sensorName.Substring(6, sensorName.Length - 6), out int code))
            return code;

        return -1;
    }

    private void UpdateDisplay()
    {
        if (displayText == null) return;

        displayText.text = "";
        foreach (var s in recentSensors)
        {
            displayText.text += s + "\n";
        }
    }
}
