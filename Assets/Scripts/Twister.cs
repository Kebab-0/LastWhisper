using UnityEngine;
using System.Collections;

public class Twister : MonoBehaviour
{
    public GameObject detector;
    public GameObject clock;
    public GameObject radio;
    public GameObject shadow;


    private bool isRotating = false;
    public int rotationCount = 1;
    private int maxCount = 5;

    void Start()
    {
        detector.SetActive(true);
        clock.SetActive(true);
    }
    public void RotateOrReset()
    {
        if (!isRotating)
        {
            if (rotationCount < maxCount)
            {
                StartCoroutine(RotateCoroutine(-30f, 200f));
                rotationCount++;
            }
            else
            {
                StartCoroutine(RotateCoroutine(120f, 600f));
                rotationCount = 1;
            }

            switch (rotationCount)
            {
                case 1:
                    shadow.SetActive(true);
                    detector.SetActive(true);
                    clock.SetActive(true);
                    break;
                case 2:
                    detector.SetActive(false);
                    clock.SetActive(false);
                    radio.SetActive(true);
                    break;
                case 3:
                    radio.SetActive(false);
                    break;
                case 4:
                    shadow.SetActive(false);
                    break;
                case 5:

                    break;
            }
                
        }
    }

    private IEnumerator RotateCoroutine(float angle, float spd)
    {
        isRotating = true;

        float rotated = 0f;
        float targetRotation = Mathf.Abs(angle);
        float direction = Mathf.Sign(angle);

        while (rotated < targetRotation)
        {
            float step = spd * Time.deltaTime;
            if (rotated + step > targetRotation)
                step = targetRotation - rotated;

            transform.Rotate(0, 0, step * direction);
            rotated += step;
            yield return null;
        }

        isRotating = false;
    }
}