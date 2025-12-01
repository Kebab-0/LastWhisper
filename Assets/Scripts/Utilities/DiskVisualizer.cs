using UnityEngine;

public class SectorVisualizer : MonoBehaviour
{
    [Header("Радиусы")]
    [SerializeField] private bool showWorkRadius = true;
    [SerializeField] private bool showSpawnRadius = true;
    [SerializeField] private bool showDespawnRadius = true;
    [SerializeField] private bool showSectors = true;

    [Header("Цвета")]
    [SerializeField] private Color workRadiusColor = Color.green;
    [SerializeField] private Color spawnRadiusColor = Color.blue;
    [SerializeField] private Color despawnRadiusColor = Color.red;
    [SerializeField] private Color sectorColor = Color.yellow;

    void OnDrawGizmos()
    {
        Vector3 center = Vector3.zero;

        if (showWorkRadius)
        {
            Gizmos.color = workRadiusColor;
            Gizmos.DrawWireSphere(center, Entity.WORK_RADIUS);
        }

        if (showSpawnRadius)
        {
            Gizmos.color = spawnRadiusColor;
            Gizmos.DrawWireSphere(center, Entity.SPAWN_RADIUS);
        }

        if (showDespawnRadius)
        {
            Gizmos.color = despawnRadiusColor;
            Gizmos.DrawWireSphere(center, Entity.DESPAWN_RADIUS);
        }

        if (showSectors)
        {
            Gizmos.color = sectorColor;
            int sectors = 8;
            float angleStep = (2f * Mathf.PI) / sectors;

            for (int i = 0; i < sectors; i++)
            {
                float angle = i * angleStep;
                Vector3 point = new Vector3(
                    Mathf.Cos(angle) * Entity.DESPAWN_RADIUS,
                    Mathf.Sin(angle) * Entity.DESPAWN_RADIUS,
                    0
                );
                Gizmos.DrawLine(center, point);
            }
        }
    }
}