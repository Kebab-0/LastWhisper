using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Detector : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    private readonly Dictionary<string, AudioClip> sensorSounds = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        EnsureAudioSource();
    }
    public void Detection(GameObject sensor, Entity activator)
        if (activator != null && activator.EmittedSound != null)
        {
            sensorSounds[name] = activator.EmittedSound;
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

        if (displayText == null) return;

    public int maxRecords = 10;           // Ìàêñèìóì çàïèñåé â ñïèñêå

    private Queue<string> recentSensors = new Queue<string>();

    // Âûçûâàåòñÿ ñ Sensors
    public void Detection(GameObject sensor)
    {
        if (sensor == null) return;

        string name = sensor.name;

        // Åñëè óæå åñòü â î÷åðåäè, óäàëÿåì, ÷òîáû ïåðåìåñòèòü â êîíåö
        if (recentSensors.Contains(name))
        {
            Queue<string> temp = new Queue<string>();
            foreach (var s in recentSensors)
                if (s != name) temp.Enqueue(s);
            recentSensors = temp;
        }

        // Äîáàâëÿåì íîâîå èìÿ
        recentSensors.Enqueue(name);

        // Åñëè ïðåâûøàåì ëèìèò, óäàëÿåì ñòàðûå
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
