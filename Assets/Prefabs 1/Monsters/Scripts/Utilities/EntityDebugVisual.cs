using UnityEngine;

public class EntityDebugVisual : MonoBehaviour
{
    private Entity entity;

    private void Start()
    {
        entity = GetComponent<Entity>();
    }

    private void OnDrawGizmos()
    {
        if (entity == null) return;

        // Рисуем линию направления движения
        Vector3 worldPos = transform.position;
        Vector2 polar = entity.PolarPosition;
        Vector3 direction = new Vector3(Mathf.Cos(polar.y), Mathf.Sin(polar.y), 0);

        Gizmos.color = Color.white;
        Gizmos.DrawRay(worldPos, direction * 0.5f);

        // Подпись с углом
#if UNITY_EDITOR
        UnityEditor.Handles.Label(worldPos + Vector3.up * 0.3f, $"{polar.y * Mathf.Rad2Deg:F1}°");
#endif
    }
}