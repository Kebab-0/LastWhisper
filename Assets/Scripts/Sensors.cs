using UnityEngine;

public class Sensors : MonoBehaviour
{
    public Transform Center;
    public Transform[] sensors1;
    public Transform[] sensors2;
    public Transform[] sensors3;
    public Transform[] sensors4;
    public Transform[] sensors5;
    public float detectionDistance1;
    public float detectionDistance2;
    public float detectionDistance3;
    public float detectionDistance4;
    public float detectionDistance5;
    public Color activeColor;
    public Color passiveColor;
    int chooseIndex = -2;
    int chooseDistance = -2;
    int sensorIndex = -1;
    int nearIndex = -1;
    public Detector detector;
    private SpriteRenderer[] sensorRenderers1;
    private SpriteRenderer[] sensorRenderers2;
    private SpriteRenderer[] sensorRenderers3;
    private SpriteRenderer[] sensorRenderers4;
    private SpriteRenderer[] sensorRenderers5;


    void Start()
    {
        sensorRenderers1 = new SpriteRenderer[sensors1.Length];
        sensorRenderers2 = new SpriteRenderer[sensors2.Length];
        sensorRenderers3 = new SpriteRenderer[sensors3.Length];
        sensorRenderers4 = new SpriteRenderer[sensors4.Length];
        sensorRenderers5 = new SpriteRenderer[sensors5.Length];
        for (int i = 0; i < sensors1.Length; i++)
        {
            sensorRenderers1[i] = sensors1[i].GetComponent<SpriteRenderer>();
            
        }
        for (int i = 0; i < sensors2.Length; i++)
        {
            sensorRenderers2[i] = sensors2[i].GetComponent<SpriteRenderer>();

        }
        for (int i = 0; i < sensors3.Length; i++)
        {
            sensorRenderers3[i] = sensors3[i].GetComponent<SpriteRenderer>();

        }
        for (int i = 0; i < sensors4.Length; i++)
        {
            sensorRenderers4[i] = sensors4[i].GetComponent<SpriteRenderer>();

        }
        for (int i = 0; i < sensors5.Length; i++)
        {
            sensorRenderers5[i] = sensors5[i].GetComponent<SpriteRenderer>();

        }
    }

    void Repaint(int rad, int ind)
    {
        switch (rad)
        {
            case 1:
                sensorRenderers1[ind].color = passiveColor;
                break;
            case 2:
                sensorRenderers2[ind].color = passiveColor;
                break;
            case 3:
                sensorRenderers3[ind].color = passiveColor;
                break;
            case 4:
                sensorRenderers4[ind].color = passiveColor;
                break;
            case 5:
                sensorRenderers5[ind].color = passiveColor;
                break;
        }
    }

    void Update()
    {
        if (nearIndex != -1)
        {
            Repaint(sensorIndex, nearIndex);
        }
        

        float distanceCenter = Vector3.Distance(transform.position, Center.position);
        if (distanceCenter <= detectionDistance1)
        {
            
            float minimal = 999999999;
            int nearestIndex = -1;
            
            for (int i = 0; i < sensors1.Length; i++)
            {
                float dist = Vector3.Distance(sensors1[i].position, transform.position);
                if (dist < minimal)
                {
                    minimal = dist;
                    nearestIndex = i;
                    nearIndex = i;
                    
                }
            }

            if (nearestIndex != -1 && sensorRenderers1[nearestIndex] != null)
            {
                sensorIndex = 1;
                sensorRenderers1[nearestIndex].color = activeColor;
                


                if (chooseIndex != nearestIndex || chooseDistance != 1)
                {
                    chooseIndex = nearestIndex;
                    chooseDistance = 1;
                    detector.Detection();
                }
            }
        }
        if (distanceCenter <= detectionDistance2 && distanceCenter > detectionDistance1)
        {

            float minimal = 999999999;
            int nearestIndex = -1;
            for (int i = 0; i < sensors2.Length; i++)
            {
                float dist = Vector3.Distance(sensors2[i].position, transform.position);
                if (dist < minimal)
                {
                    minimal = dist;
                    nearestIndex = i;
                    nearIndex = i;
                }
            }

            if (nearestIndex != -1 && sensorRenderers2[nearestIndex] != null)
            {
                sensorIndex = 2;
                sensorRenderers2[nearestIndex].color = activeColor;
                
                if (chooseIndex != nearestIndex || chooseDistance != 2)
                {
                    chooseIndex = nearestIndex;
                    chooseDistance = 2;
                    detector.Detection();
                }
            }
        }
        if (distanceCenter <= detectionDistance3 && distanceCenter > detectionDistance2)
        {

            float minimal = 999999999;
            int nearestIndex = -1;
            for (int i = 0; i < sensors3.Length; i++)
            {
                float dist = Vector3.Distance(sensors3[i].position, transform.position);
                if (dist < minimal)
                {
                    minimal = dist;
                    nearestIndex = i;
                    nearIndex = i;
                }
            }

            if (nearestIndex != -1 && sensorRenderers3[nearestIndex] != null)
            {
                sensorIndex = 3;
                sensorRenderers3[nearestIndex].color = activeColor;
                
                if (chooseIndex != nearestIndex || chooseDistance != 3)
                {
                    chooseIndex = nearestIndex;
                    chooseDistance = 3;
                    detector.Detection();
                }
            }
        }
        if (distanceCenter <= detectionDistance4 && distanceCenter > detectionDistance3)
        {

            float minimal = 999999999;
            int nearestIndex = -1;
            for (int i = 0; i < sensors4.Length; i++)
            {
                float dist = Vector3.Distance(sensors4[i].position, transform.position);
                if (dist < minimal)
                {
                    minimal = dist;
                    nearestIndex = i;
                    nearIndex = i;
                }
            }

            if (nearestIndex != -1 && sensorRenderers4[nearestIndex] != null)
            {
                sensorRenderers4[nearestIndex].color = activeColor;
                sensorIndex = 4;

                if (chooseIndex != nearestIndex || chooseDistance != 4)
                {
                    chooseIndex = nearestIndex;
                    chooseDistance = 4;
                    detector.Detection();
                }
            }
        }
        if (distanceCenter <= detectionDistance5 && distanceCenter > detectionDistance4)
        {

            float minimal = 999999999;
            int nearestIndex = -1;
            for (int i = 0; i < sensors5.Length; i++)
            {
                float dist = Vector3.Distance(sensors5[i].position, transform.position);
                if (dist < minimal)
                {
                    minimal = dist;
                    nearestIndex = i;
                    nearIndex = i;
                }
            }

            if (nearestIndex != -1 && sensorRenderers5[nearestIndex] != null)
            {
                sensorRenderers5[nearestIndex].color = activeColor;
                sensorIndex = 5;

                if (chooseIndex != nearestIndex || chooseDistance != 5)
                {
                    chooseIndex = nearestIndex;
                    chooseDistance = 5;
                    detector.Detection();
                }
            }
        }
    }
}