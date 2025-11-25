using UnityEngine;

public class CameraZoomToMonitor : MonoBehaviour
{
    public Camera cam;
    public Transform[] monitors;
    public Vector3 offset = new Vector3(0, 0, 0);
    public float zoomedSize = 3f;


    public float moveSmoothSpeed = 0.125f;
    public float zoomSmoothSpeed = 0.1f;

    private Vector3 initialPosition;
    private float initialSize;

    private Vector3 targetPosition;
    private float targetSize;

    private bool isZoomed = false;
    private Transform currentMonitor = null;

    void Start()
    {
        
        initialPosition = transform.position;
        initialSize = cam.orthographicSize;
        targetPosition = initialPosition;
        targetSize = initialSize;
    }

    void Update()
    {
        
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);

            
            foreach (Transform monitor in monitors)
            {
                Collider2D col = monitor.GetComponent<Collider2D>();
                if (col != null)
                {
                    if (col.OverlapPoint(worldPos))
                    {
                        
                        currentMonitor = monitor;
                        targetPosition = monitor.position + offset;
                        targetSize = zoomedSize;
                        isZoomed = true;
                        break;
                    }
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentMonitor = null;
            targetPosition = initialPosition;
            targetSize = initialSize;
            isZoomed = false;
        }


        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSmoothSpeed);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSmoothSpeed);
    }
}