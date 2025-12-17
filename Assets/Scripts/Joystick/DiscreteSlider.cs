using UnityEngine;

public class VerticalDiscreteSlider2D : MonoBehaviour
{
    public Transform handle;
    public AudioSource audioSource;
    public AudioClip tickSound;

    public int minValue = 1;
    public int maxValue = 5;

    public int CurrentValue { get; private set; }

    float minY, maxY;
    Camera cam;
    bool dragging;
    int lastValue;

    Collider2D handleCollider;

    void Start()
    {
        cam = Camera.main;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        minY = sr.bounds.min.y;
        maxY = sr.bounds.max.y;

        handleCollider = handle.GetComponent<Collider2D>();

        SetValue(minValue);
        lastValue = CurrentValue;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouse = cam.ScreenToWorldPoint(Input.mousePosition);

            if (handleCollider != null && handleCollider.OverlapPoint(mouse))
                dragging = true;
        }

        if (Input.GetMouseButtonUp(0))
            dragging = false;

        if (dragging)
            Drag();
    }

    void Drag()
    {
        float y = cam.ScreenToWorldPoint(Input.mousePosition).y;
        y = Mathf.Clamp(y, minY, maxY);

        float t = Mathf.InverseLerp(minY, maxY, y);
        int newValue = Mathf.RoundToInt(
            Mathf.Lerp(minValue, maxValue, t)
        );

        if (newValue != CurrentValue)
        {
            CurrentValue = newValue;
            PlayTick();
            SetValue(CurrentValue);
        }
    }

    void SetValue(int value)
    {
        float t = Mathf.InverseLerp(minValue, maxValue, value);
        float y = Mathf.Lerp(minY, maxY, t);

        handle.position = new Vector3(
            handle.position.x,
            y,
            handle.position.z
        );
    }

    void PlayTick()
    {
        if (audioSource && tickSound)
            audioSource.PlayOneShot(tickSound);
    }
}
