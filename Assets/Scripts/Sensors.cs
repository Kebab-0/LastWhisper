using UnityEngine;
using System.Collections.Generic;

public class Sensors : MonoBehaviour
{
    private Entity ownerEntity;
        ownerEntity = GetComponent<Entity>();
        if (ownerEntity == null)
            ownerEntity = GetComponentInParent<Entity>();

    public Color activeColor = Color.red;      // Öâåò àêòèâíîãî ñåíñîðà
    public Color passiveColor = Color.gray;    // Öâåò ïàññèâíîãî ñåíñîðà
    public Detector detector;                  // Ññûëêà íà Detector (UI)

    public float[] detectionDistances = new float[5] { 2f, 4f, 6f, 8f, 10f }; // Ðàäèóñû çîí

    private Dictionary<int, Transform[]> sensors = new Dictionary<int, Transform[]>();
    private Dictionary<int, SpriteRenderer[]> renderers = new Dictionary<int, SpriteRenderer[]>();

    private int prevRadius = -1;   // Ïîñëåäíèé àêòèâíûé ðàäèóñ
    private int prevIndex = -1;    // Ïîñëåäíèé àêòèâíûé ñåíñîð

    void Start()
    {
        AutoFindDetector();
        AutoDetectSensors();
    }

    // ------------------------------
    // Àâòî-íàõîäèì Detector
    // ------------------------------
    void AutoFindDetector()
    {
        if (detector != null) return;

        detector = FindObjectOfType<Detector>();
        if (detector == null)
        {
            Debug.LogWarning("Sensors: Detector íå íàéäåí! Ïðèñâîéòå âðó÷íóþ â èíñïåêòîðå.");
        }
    }

    // ------------------------------
    // Àâòî-íàõîäèì ñåíñîðû ïî òåãó "Sens"
    // ------------------------------
    void AutoDetectSensors()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Sens"); // âñå ñåíñîðû äîëæíû èìåòü òåã "Sens"
        Dictionary<int, List<Transform>> temp = new Dictionary<int, List<Transform>>();

        int maxRadius = detectionDistances.Length;
        for (int r = 1; r <= maxRadius; r++)
            temp[r] = new List<Transform>();

        foreach (var obj in objs)
        {
            string n = obj.name;
            if (!n.StartsWith("Sensor") || n.Length < 8) continue;

            if (!int.TryParse(n.Substring(6, 1), out int radius)) continue;
            if (radius < 1 || radius > maxRadius) continue;

            temp[radius].Add(obj.transform);
        }

        for (int r = 1; r <= maxRadius; r++)
        {
            temp[r].Sort((a, b) => a.name.CompareTo(b.name));
            sensors[r] = temp[r].ToArray();

            SpriteRenderer[] rs = new SpriteRenderer[sensors[r].Length];
            for (int i = 0; i < sensors[r].Length; i++)
            {
                rs[i] = sensors[r][i] != null ? sensors[r][i].GetComponent<SpriteRenderer>() : null;
                if (rs[i] != null)
                    rs[i].color = passiveColor;
            }
            renderers[r] = rs;
        }
    }

    // ------------------------------
    // Îñíîâíîé Update
    // ------------------------------
    void Update()
    {
        int nearestRadius = -1;
        int nearestIndex = -1;
        float minDist = float.MaxValue;

        int maxRadius = Mathf.Min(5, detectionDistances.Length);

        // Îáõîä ðàäèóñîâ
        for (int r = 1; r <= maxRadius; r++)
        {
            if (!sensors.ContainsKey(r) || sensors[r] == null || sensors[r].Length == 0) continue;

            Transform[] arr = sensors[r];
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == null) continue;

                float d = Vector3.Distance(transform.position, arr[i].position);

                // Ïðîâåðÿåì èíäåêñ ìàññèâà detectionDistances
                if (r - 1 >= 0 && r - 1 < detectionDistances.Length && d <= detectionDistances[r - 1] && d < minDist)
                {
                    minDist = d;
                    nearestRadius = r;
                    nearestIndex = i;
                }
            }
        }

        // Äåàêòèâèðóåì ïðåäûäóùèé ñåíñîð
        if (prevRadius != -1 && prevIndex != -1 &&
            renderers.ContainsKey(prevRadius) &&
            renderers[prevRadius] != null &&
            prevIndex < renderers[prevRadius].Length &&
            renderers[prevRadius][prevIndex] != null)
        {
            renderers[prevRadius][prevIndex].color = passiveColor;
        }

                detector.Detection(sensors[nearestRadius][nearestIndex].gameObject, ownerEntity);
        if (nearestRadius != -1 && nearestIndex != -1 &&
            sensors.ContainsKey(nearestRadius) &&
            renderers.ContainsKey(nearestRadius) &&
            sensors[nearestRadius] != null &&
            renderers[nearestRadius] != null &&
            nearestIndex < sensors[nearestRadius].Length &&
            nearestIndex < renderers[nearestRadius].Length &&
            sensors[nearestRadius][nearestIndex] != null &&
            renderers[nearestRadius][nearestIndex] != null)
        {
            renderers[nearestRadius][nearestIndex].color = activeColor;

            // Äåòåêòîð ñðàáàòûâàåò òîëüêî ïðè ñìåíå ñåíñîðà
            bool isNewSensor = (nearestRadius != prevRadius || nearestIndex != prevIndex);

            prevRadius = nearestRadius;
            prevIndex = nearestIndex;

            if (detector != null && isNewSensor)
            {
                detector.Detection(sensors[nearestRadius][nearestIndex].gameObject);
            }

        }
        else
        {
            prevRadius = -1;
            prevIndex = -1;
        }
    }
}
