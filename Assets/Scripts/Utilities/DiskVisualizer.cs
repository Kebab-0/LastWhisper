using UnityEngine;

public class DiskVisualizer : MonoBehaviour
{
    [Header("Disk Settings")]
    public float diskRadius = 800f;
    public int circleCount = 5;
    public float circleSpacing = 100f;

    private void OnDrawGizmos()
    {
        // Внешняя граница
        Gizmos.color = Color.red;
        DrawCircle(diskRadius);

        // Внутренние окружности
        Gizmos.color = Color.blue;
        for (int i = 1; i <= circleCount; i++)
        {
            DrawCircle(i * circleSpacing);
        }

        // Сектора
        Gizmos.color = Color.green;
        DrawSectors();
    }

    private void DrawCircle(float radius)
    {
        Vector3 center = Vector3.zero;
        int segments = 36;
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * radius;
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * radius;

            Gizmos.DrawLine(point1, point2);
        }
    }

    private void DrawSectors()
    {
        int sectors = 8;
        float angleStep = 360f / sectors;

        for (int i = 0; i < sectors; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            Gizmos.DrawLine(Vector3.zero, direction * diskRadius);
        }
    }
}