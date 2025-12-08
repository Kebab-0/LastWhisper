using UnityEngine;

public class CameraZoomToMonitor : MonoBehaviour
{
    public GameObject wall;                 // Фон сцены
    public Camera cam;
    public Transform[] monitors;
    public Vector3 offset = Vector3.zero;
    public float zoomedSize = 3f;

    public float moveSmoothSpeed = 0.125f;
    public float zoomSmoothSpeed = 0.1f;

    [Range(0f, 1f)]
    public float wallTransparency = 0.3f;   // Прозрачность при зуме

    private Vector3 initialPosition;
    private float initialSize;

    private Vector3 targetPosition;
    private float targetSize;

    private bool isZoomed = false;
    private Transform currentMonitor = null;

    private SpriteRenderer wallRenderer;
    private Color wallInitialColor;

    void Start()
    {
        initialPosition = transform.position;
        initialSize = cam.orthographicSize;
        targetPosition = initialPosition;
        targetSize = initialSize;

        if (wall != null)
        {
            wallRenderer = wall.GetComponent<SpriteRenderer>();
            if (wallRenderer != null)
                wallInitialColor = wallRenderer.color;
        }
    }

    void Update()
    {
        // Проверка клика по монитору
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
            worldPos.z = 0;

            foreach (Transform monitor in monitors)
            {
                Collider2D col = monitor.GetComponent<Collider2D>();
                if (col != null && col.OverlapPoint(worldPos))
                {
                    currentMonitor = monitor;
                    targetPosition = monitor.position + offset;
                    targetSize = zoomedSize;
                    isZoomed = true;

                    // Делаем фон полупрозрачным
                    if (wallRenderer != null)
                    {
                        Color c = wallInitialColor;
                        c.a = wallTransparency;
                        wallRenderer.color = c;
                    }

                    break;
                }
            }
        }

        // Выход с зума (TAB)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentMonitor = null;
            targetPosition = initialPosition;
            targetSize = initialSize;
            isZoomed = false;

            // Восстанавливаем исходную прозрачность фона
            if (wallRenderer != null)
                wallRenderer.color = wallInitialColor;
        }

        // Плавное движение и зум камеры
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSmoothSpeed);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSmoothSpeed);
    }
}
