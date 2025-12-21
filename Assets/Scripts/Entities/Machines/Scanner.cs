using UnityEngine;

public class Scanner : Entity
{
    private SectorManager sectorManager;
    private float rotationSpeed = 90f;
    private float scanRadius = 50f;
    private float rotationAngle;

    protected override bool UsePolarMovement => false;

    protected override void InitializeMovement()
    {
        entityColor = Color.blue;
        sectorManager = SectorManager.Instance;
        EnsureMoveSpeed(30f);

        if (sectorManager != null)
            sectorManager.RegisterScanner(this);
    }

    protected override void Move()
    {
        // Вращение
        rotationAngle += rotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, rotationAngle);

        // Медленное движение к центру
        Vector3 directionToCenter = -transform.position.normalized;
        transform.position += directionToCenter * (moveSpeed * 0.3f * Time.deltaTime);

        // Сканирование
        ScanForTargets();
    }

    private void ScanForTargets()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, scanRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player") || hit.GetComponent<Entity>() != null)
            {
                Debug.Log($"Scanner обнаружен: {hit.name}");
                // Можно активировать тревогу или другую логику
            }
        }
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);

        if (currentHealth <= 0 && sectorManager != null)
        {
            sectorManager.UnregisterScanner(this);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, scanRadius);
    }
}
