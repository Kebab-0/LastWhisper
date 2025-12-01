using UnityEngine;

public class Repressor : Entity
{
    private SectorManager sectorManager;
    private Scanner targetScanner;
    private Vector3 patrolPoint;
    private float patrolTimer;
    private float patrolChangeTime = 4f;

    protected override bool UsePolarMovement => false;

    protected override void InitializeMovement()
    {
        entityColor = Color.green;
        sectorManager = SectorManager.Instance;
        patrolPoint = GetRandomPatrolPoint();
    }

    protected override void Move()
    {
        // 1. ѕровер€ем последний уничтоженный сканер
        if (sectorManager != null)
        {
            targetScanner = sectorManager.GetLastDestroyedScanner();

            if (targetScanner != null)
            {
                // ≈дем к месту последнего уничтоженного сканера
                MoveTowardsWorld(targetScanner.transform.position);
                return;
            }
        }

        // 2. ≈сли нет целей - патрулируем
        patrolTimer += Time.deltaTime;

        MoveTowardsWorld(patrolPoint);

        if (patrolTimer >= patrolChangeTime || Vector3.Distance(transform.position, patrolPoint) < 5f)
        {
            patrolPoint = GetRandomPatrolPoint();
            patrolTimer = 0f;
        }
    }

    private Vector3 GetRandomPatrolPoint()
    {
        // —лучайна€ точка в пределах рабочей зоны
        Vector2 randomPoint = Random.insideUnitCircle * WORK_RADIUS;
        return new Vector3(randomPoint.x, randomPoint.y, 0);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 20f);
    }
}